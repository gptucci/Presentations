using Microsoft.Datasync.Client;
using Microsoft.Datasync.Client.Offline;
using Microsoft.Datasync.Client.Offline.Queue;
using Microsoft.Datasync.Client.SQLiteStore;
using System.Diagnostics;
namespace AzureMobSvc.Client;

public interface IClientDbContext
{

    Task<IEnumerable<Expense>> GetItemsAsync();


    Task RefreshItemsAsync();


    Task RemoveItemAsync(Expense item);

    Task InsertItemAsync(Expense item);
    Task UpdateItemAsync(Expense newItem);
}

internal class ClientDbContextService : IClientDbContext
{
    IOfflineTable<Expense> _table = null;
    public string OfflineDb { get; set; } = FileSystem.CacheDirectory + "/offline.db";

    bool _initialized = false;

    DatasyncClient _client = null;


    async Task InitializeAsync()
    {
        //E' già inizializzato
        if (_client != null)
            return;

        var connectionString = new UriBuilder { Scheme = "file", Path = OfflineDb, Query = "?mode=rwc" }.Uri.ToString();
        var store = new OfflineSQLiteStore(connectionString);
        store.DefineTable<Expense>();

        //var options = new DatasyncClientOptions
        //{
        //    OfflineStore = store,
        //    HttpPipeline = new HttpMessageHandler[] { new LoggingHandler() }
        //};
        var options = new DatasyncClientOptions
        {
            OfflineStore = store
        };
        //// Create the datasync client.
        _client = new DatasyncClient(ExpenseMobileConstants.ServiceUri, options);

        // Initialize the database
        await _client.InitializeOfflineStoreAsync();

        // Get a reference to the offline table.
        _table = _client.GetOfflineTable<Expense>();


    }

    public async Task<IEnumerable<Expense>> GetItemsAsync()
    {
        await InitializeAsync();
        return await _table.GetAsyncItems().ToListAsync();
    }

    public async Task RefreshItemsAsync()
    {
        IReadOnlyCollection<TableOperationError> syncErrors = null;
        await InitializeAsync();

        try
        {
            // First, push all the items in the table.
            await _table.PushItemsAsync();
            // Then, pull all the items in the table.
            await _table.PullItemsAsync();
        }
        catch (PushFailedException exc)
        {
            if (exc.PushResult != null)
            {
                syncErrors = exc.PushResult.Errors;
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(@"SyncItemsAsync ERROR {0}", e.Message);
        }

        if (syncErrors != null)
        {
            foreach (var error in syncErrors)
            {
                if (error.OperationKind == TableOperationKind.Update && error.Result != null)
                {
                    //error.Result - contiene la versione della tupla salvata sul server
                    //Update failed reverting to server's copy.
                    //Maschera da presentare a utilizzatore ??
                    await error.CancelAndUpdateItemAsync(error.Result);
                }
                else
                {
                    //Situazione anomala - si elimina l'aggiornamento
                    await error.CancelAndDiscardItemAsync();
                }

                Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
            }
        }


    }

    public Task RemoveItemAsync(Expense item)
    {
        return _table.DeleteItemAsync(item);
    }

    public Task InsertItemAsync(Expense item)
    {
        return _table.InsertItemAsync(item);
    }

    public Task UpdateItemAsync(Expense item)
    {
        return _table.ReplaceItemAsync(item);
    }
}
