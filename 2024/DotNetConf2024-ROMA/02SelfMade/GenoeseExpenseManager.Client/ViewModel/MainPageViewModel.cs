using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace GenoeseExpenseManager.Client;

public partial class MainPageViewModel : ObservableObject
{
    private readonly IRemoteDBService _remoteDBService;
    [ObservableProperty]
    ObservableCollection<Expense> expenses = new();

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    Expense spesaInEditing = new() { Data = DateTime.Now.Date };


    public MainPageViewModel(IRemoteDBService remoteDBService)
    {
        _remoteDBService = remoteDBService;
    }
    public async Task InitializzaVmAsync()
    {
        IsBusy = true;
        await _remoteDBService.InitializeAsync();
        await _remoteDBService.SyncAllAsync<Expense>();

        Expenses = new ObservableCollection<Expense>(await _remoteDBService.Get_AllItemsAsync<Expense>(x => true));
        IsBusy = false;
    }
    [RelayCommand]
    public async Task SaveExpenseAsync()
    {
        IsBusy = true;
        if (SpesaInEditing.Id == Guid.Empty)
        {
            await _remoteDBService.Insert_ItemAsync(SpesaInEditing);

        }

        await _remoteDBService.SyncAllAsync<Expense>();

        SpesaInEditing = new Expense() { Data = DateTime.Now.Date };
        IsBusy = false;
    }

}
