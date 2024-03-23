using Akavache;
using System.Linq.Expressions;
using System.Net.Http.Json;
using System.Reactive.Linq;

namespace MauiAppAkavache.Client;
public interface IClientRepository
{
    Task Delete_ItemAsync<T>(Guid Id) where T : BaseEntityForMobile, new();

    Task<List<T>> Get_AllItemsAsync<T>(Expression<Func<T, bool>> funct) where T : BaseEntityForMobile, new();
    Task<T> Get_ItemAsync<T>(Guid Id) where T : BaseEntityForMobile, new();
    Task InitializeAsync();
    Task Insert_ItemAsync<T>(T newItem) where T : BaseEntityForMobile;

}
internal class ClientRepository : IClientRepository
{
    private readonly HttpClient _httpClient;
    public ClientRepository(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient(AllConstants.httpClient_SyncDati);
    }

    public async Task InitializeAsync()
    {

        Akavache.Registrations.Start("ApplicationName");
    }

    public async Task<List<T>> Get_AllItemsAsync<T>(Expression<Func<T, bool>> funct) where T : BaseEntityForMobile, new()
    {
        return await BlobCache.LocalMachine.GetOrFetchObject<List<T>>(typeof(T).Name, async () => await Get_ItemsFromWebAsync<T>(), DateTimeOffset.Now.AddHours(3));

    }



    public async Task<T> Get_ItemAsync<T>(Guid Id) where T : BaseEntityForMobile, new()
    {

        var listaCompleta = await BlobCache.LocalMachine.GetOrFetchObject<List<T>>(typeof(T).Name, async () => await Get_ItemsFromWebAsync<T>(), DateTimeOffset.Now.AddHours(3));
        return listaCompleta.FirstOrDefault(x => x.Id == Id);

    }
    async Task<List<T>> Get_ItemsFromWebAsync<T>() where T : BaseEntityForMobile, new()
    {

        try
        {
            string s = "/api/" + typeof(T).Name + "s";
            using var httpResponseMessage = await _httpClient.GetAsync("/api/" + typeof(T).Name + "s");
            httpResponseMessage.EnsureSuccessStatusCode();
            var contentResponse =
                       await httpResponseMessage.Content.ReadAsStringAsync();


            if (!string.IsNullOrEmpty(contentResponse)) //Per gestire situazioni limite
            {
                return Serializations.DeserializeJsonString<List<T>>(contentResponse);
            }
            return new List<T>();
        }
        catch (Exception)
        {

            return new List<T>();
        }


    }
    public async Task Insert_ItemAsync<T>(T newItem) where T : BaseEntityForMobile
    {
        if (newItem.Id == Guid.Empty)
        {
            newItem.Id = Guid.NewGuid();
        }

        await InviaDatoAsync<T>(newItem);
    }


    async Task InviaDatoAsync<T>(T item) where T : BaseEntityForMobile
    {
        string NomeEndpoint = typeof(T).Name;
        string url = $"/api/{NomeEndpoint}s";


        var resp = await _httpClient.PostAsJsonAsync<T>(url, item);
        if (resp.IsSuccessStatusCode)
        {
            await BlobCache.LocalMachine.Invalidate(typeof(T).Name);
        }
        else
        {


        }


    }

    public async Task Delete_ItemAsync<T>(Guid Id) where T : BaseEntityForMobile, new()
    {
        var itemFromDb = await Get_ItemAsync<T>(Id);
        if (itemFromDb == null)
        {
            return;
        }

        string NomeEndpoint = typeof(T).Name;
        string url = $"/api/{NomeEndpoint}s/{Id}";


        var resp = await _httpClient.DeleteAsync(url);
        if (resp.IsSuccessStatusCode)
        {
            await BlobCache.LocalMachine.Invalidate(typeof(T).Name);
        }
        else
        {


        }
    }
}
