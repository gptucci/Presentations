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

    Task InviaDatiAsync<T>() where T : BaseEntityForMobile, new();

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
        //Se uso quello di seguito mi da errore se non esiste alcun item
        //return database.GetAsync<T>(Id);
    }

    public Task Insert_ItemAsync<T>(T newItem) where T : BaseEntityForMobile
    {
        if (newItem.Id == Guid.Empty)
        {
            newItem.Id = Guid.NewGuid();
        }
        return database.InsertAsync(newItem);
    }

    public Task Update_ItemAsync<T>(T newItem) where T : BaseEntityForMobile
    {
        return database.UpdateAsync(newItem);
    }


    public async Task<Result> SyncAllAsync<T>()
        where T : BaseEntityForMobile, new()
    {

        var latestUpdateAt = await database.Table<LatestUpdateAt>()
                           .Where(i => i.NomeDati == typeof(T).Name)
                           .FirstOrDefaultAsync();

        if (latestUpdateAt == null)
        {
            latestUpdateAt = new LatestUpdateAt();

            latestUpdateAt.NomeDati = typeof(T).Name;
            latestUpdateAt.MaxUpdateAt = DateTimeOffset.Now.AddYears(-1);
            await Insert_ItemAsync<LatestUpdateAt>(latestUpdateAt);
        }
        else
        {
            latestUpdateAt.MaxUpdateAt = latestUpdateAt.MaxUpdateAt;
        }

        string DateTimeFormat = latestUpdateAt.MaxUpdateAt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        //DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)

        //DateTimeFormat += "T";
        ////DateTimeFormat += latestUpdateAt.MaxUpdateAt.ToUniversalTime().Hour.ToString().PadLeft(2, '0') + ":00:00Z";
        //DateTimeFormat += latestUpdateAt.MaxUpdateAt.ToUniversalTime().Hour.ToString().PadLeft(2, '0') + ":"; // Ore
        //DateTimeFormat += latestUpdateAt.MaxUpdateAt.ToUniversalTime().Minute.ToString().PadLeft(2, '0') + ":";
        //DateTimeFormat += latestUpdateAt.MaxUpdateAt.ToUniversalTime().Second.ToString().PadLeft(2, '0') + "Z";
        //replace non va bene per via del case (maiuscolo/minuscolo)
        string NomeOdataEndpoint = Regex.Replace(typeof(T).Name, "dto", String.Empty, RegexOptions.IgnoreCase);

        string url = $"/api/odata/OData{NomeOdataEndpoint}?$filter=UpdatedAt+gt+" + DateTimeFormat + "&orderby=UpdatedAt+asc";

        var risp = new ODataResponse<T>();
        var ListaDaDB = await database.Table<T>().ToListAsync();

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


        } while (risp.value.Count > 0);

        await Update_ItemAsync<LatestUpdateAt>(latestUpdateAt);

        return new Result();
    }

    public async Task InviaDatiAsync<T>() where T : BaseEntityForMobile, new()
    {
        string NomeEndpoint = typeof(T).Name;
        string url = $"/api/{NomeEndpoint}s";

        var itemCancellati = await Get_AllItemsAsync<T>(x => x.Deleted);
        foreach (var item in itemCancellati)
        {
            var resp = await _httpClient.DeleteAsync(url + "/" + item.Id);
            if (resp.IsSuccessStatusCode)
            {
                await Delete_ItemAsync<T>(item.Id);
            }

        }
        var itemInseritiModificati = await Get_AllItemsAsync<T>(x => x.DaInviare);
        foreach (var item in itemInseritiModificati)
        {
            var resp = await _httpClient.PostAsJsonAsync<T>(url, item);
            if (resp.IsSuccessStatusCode)
            {
                item.DaInviare = false;
                await Update_ItemAsync<T>(item);
            }
            else
            {

                var resp1 = await resp.Content.ReadAsStringAsync();
            }

        }
    }
}
