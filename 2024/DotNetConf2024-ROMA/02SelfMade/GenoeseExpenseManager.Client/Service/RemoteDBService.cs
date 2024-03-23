using GenoeseExpenseMng.Shared;
using SQLite;
using System.Globalization;
using System.Linq.Expressions;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace GenoeseExpenseManager.Client;
public interface IRemoteDBService
{
    Task Delete_ItemAsync<T>(Guid Id) where T : BaseEntityForMobile, new();

    Task<List<T>> Get_AllItemsAsync<T>(Expression<Func<T, bool>> funct) where T : BaseEntityForMobile, new();
    Task<T> Get_ItemAsync<T>(Guid Id) where T : BaseEntityForMobile, new();
    Task InitializeAsync();
    Task Insert_ItemAsync<T>(T newItem) where T : BaseEntityForMobile;

    Task Update_ItemAsync<T>(T newItem) where T : BaseEntityForMobile;
    Task<Result> SyncAllAsync<T>() where T : BaseEntityForMobile, new();
}
internal class RemoteDBService : IRemoteDBService
{
    private readonly HttpClient _httpClient;
    public RemoteDBService(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient(Constants.httpClient_SyncDati);
    }
    private bool _initialized = false;
    private readonly SemaphoreSlim _asyncLock = new(1, 1);
    private SQLiteAsyncConnection database;
    public async Task InitializeAsync()
    {

        if (_initialized)
        {
            return;
        }
        try
        {
            await _asyncLock.WaitAsync();
            var Path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "expensemanager.db");
            database = new SQLiteAsyncConnection(Path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
            await database.EnableWriteAheadLoggingAsync().ConfigureAwait(false);

            await database.ExecuteAsync("PRAGMA temp_store=MEMORY").ConfigureAwait(false);
            await database.ExecuteAsync("PRAGMA synchronous=OFF").ConfigureAwait(false);

            await database.CreateTableAsync<Expense>().ConfigureAwait(false);
            await database.CreateTableAsync<LatestUpdateAt>().ConfigureAwait(false);

            _initialized = true;

        }
        catch (Exception)
        {

            return;
        }
        finally
        {
            _asyncLock.Release();
        }
    }

    public async Task Delete_ItemAsync<T>(Guid Id) where T : BaseEntityForMobile, new()
    {
        var itemFromDb = await Get_ItemAsync<T>(Id);
        if (itemFromDb == null)
        {
            return;
        }

        itemFromDb.Deleted = true;

        await Update_ItemAsync(itemFromDb);

    }

    public Task<List<T>> Get_AllItemsAsync<T>(Expression<Func<T, bool>> funct) where T : BaseEntityForMobile, new()
    {
        return database.Table<T>()
            .Where(funct)
            .ToListAsync();
    }



    public Task<T> Get_ItemAsync<T>(Guid Id) where T : BaseEntityForMobile, new()
    {
        return database.Table<T>()
                    .FirstOrDefaultAsync(x => x.Id == Id);

    }

    public Task Insert_ItemAsync<T>(T newItem) where T : BaseEntityForMobile
    {
        if (newItem.Id == Guid.Empty)
        {
            newItem.Id = Guid.NewGuid();
        }

        newItem.UpdatedAt = DateTimeOffset.Now;
        return database.InsertAsync(newItem);
    }

    public Task Update_ItemAsync<T>(T newItem) where T : BaseEntityForMobile
    {
        newItem.UpdatedAt = DateTimeOffset.Now;
        return database.UpdateAsync(newItem);
    }

    public async Task<Result> SyncAllAsync<T>()
        where T : BaseEntityForMobile, new()
    {
        //Per prima cosa spedisce gli item modificati localmente
        await PushItemsAsync<T>();


        //ora prosegue con il Pull degli item modificati lato backend
        var latestUpdateAt = await database.Table<LatestUpdateAt>()
                           .Where(i => i.EntityName == typeof(T).Name && i.Rx == true)
                           .FirstOrDefaultAsync();

        if (latestUpdateAt == null)
        {
            latestUpdateAt = new LatestUpdateAt();

            latestUpdateAt.EntityName = typeof(T).Name;
            latestUpdateAt.MaxUpdateAt = DateTimeOffset.Now.AddYears(-1);
            latestUpdateAt.Rx = true;
            await Insert_ItemAsync<LatestUpdateAt>(latestUpdateAt);
        }


        string DateTimeFormat = latestUpdateAt.MaxUpdateAt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        string NomeOdataEndpoint = Regex.Replace(typeof(T).Name, "dto", String.Empty, RegexOptions.IgnoreCase);

        string url = $"/api/odata/OData{NomeOdataEndpoint}?$filter=UpdatedAt+gt+" + DateTimeFormat;

        var risp = new ODataResponse<T>();

        int Counter = 1;

        do
        {
            int skip = (Counter - 1) * 200;
            Counter += 1;

            using var httpResponseMessage = await _httpClient.GetAsync(url + "&top=200&skip=" + skip);
            httpResponseMessage.EnsureSuccessStatusCode();

            var contentResponse =
                    await httpResponseMessage.Content.ReadAsStringAsync();


            if (!string.IsNullOrEmpty(contentResponse)) //Per gestire situazioni limite
            {
                risp = Serializations.DeserializeJsonString<ODataResponse<T>>(contentResponse);
            }
            //Oss.: qui nella realtà è possibile fare il ciclo in multithread per velocizzare il processo
            //e usare operazioni CRUD batch con SqLite 
            foreach (var item in risp.value)
            {

                if (item.UpdatedAt > latestUpdateAt.MaxUpdateAt)
                    latestUpdateAt.MaxUpdateAt = item.UpdatedAt;

                if (item.Deleted)
                {
                    await Delete_ItemAsync<T>(item.Id);
                    continue;
                }


                var itemInDB = await Get_ItemAsync<T>(item.Id);
                if (itemInDB == null)
                {
                    await Insert_ItemAsync<T>(item);
                }
                else
                {
                    await Update_ItemAsync<T>(item);
                }
            }


        } while (risp.value.Count == 200);

        await Update_ItemAsync<LatestUpdateAt>(latestUpdateAt);

        return new Result();
    }

    async Task PushItemsAsync<T>() where T : BaseEntityForMobile, new()
    {


        string NomeEndpoint = typeof(T).Name;
        string url = $"/api/{NomeEndpoint}s";

        //Prima invia gli item cancellati
        var itemCancellati = await Get_AllItemsAsync<T>(x => x.Deleted);
        foreach (var item in itemCancellati)
        {
            var resp = await _httpClient.DeleteAsync(url + "/" + item.Id);
            if (resp.IsSuccessStatusCode)
            {
                await Delete_ItemAsync<T>(item.Id);
            }

        }

        var latestUpdateAtTx = await database.Table<LatestUpdateAt>()
                          .Where(i => i.EntityName == typeof(T).Name)
                          .FirstOrDefaultAsync();

        if (latestUpdateAtTx == null)
        {
            latestUpdateAtTx = new LatestUpdateAt();

            latestUpdateAtTx.EntityName = typeof(T).Name;
            latestUpdateAtTx.MaxUpdateAt = DateTimeOffset.Now.AddYears(-1);
            latestUpdateAtTx.Rx = false;
            await Insert_ItemAsync<LatestUpdateAt>(latestUpdateAtTx);
        }


        var itemInseritiModificati = await Get_AllItemsAsync<T>(x => x.UpdatedAt > latestUpdateAtTx.MaxUpdateAt);
        if (itemInseritiModificati.Count() <= 0)
            return;

        foreach (var item in itemInseritiModificati)
        {
            var resp = await _httpClient.PostAsJsonAsync<T>(url, item);
            if (resp.IsSuccessStatusCode)
            {

            }
            else
            {

                var resp1 = await resp.Content.ReadAsStringAsync();
            }

        }
        latestUpdateAtTx.MaxUpdateAt = itemInseritiModificati.Max(x => x.UpdatedAt);
        await Update_ItemAsync<LatestUpdateAt>(latestUpdateAtTx);
    }
}
