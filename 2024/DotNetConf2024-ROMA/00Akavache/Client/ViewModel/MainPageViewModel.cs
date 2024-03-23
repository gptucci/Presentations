using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace MauiAppAkavache.Client;
public partial class MainPageViewModel : ObservableObject
{
    private readonly IClientRepository _remoteDBService;
    [ObservableProperty]
    ObservableCollection<Expense> expenses = new();

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    Expense spesaInEditing = new() { Data = DateTime.Now.Date };


    public MainPageViewModel(IClientRepository remoteDBService)
    {
        _remoteDBService = remoteDBService;
    }
    public async Task InitializeAsync()
    {
        IsBusy = true;
        await _remoteDBService.InitializeAsync();


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
            Expenses = new ObservableCollection<Expense>(await _remoteDBService.Get_AllItemsAsync<Expense>(x => true));
        }
        
        SpesaInEditing = new Expense() { Data = DateTime.Now.Date };
        IsBusy = false;
    }

}
