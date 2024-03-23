
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Datasync.Client;

namespace AzureMobSvc.Client;

public partial class MainPageViewModel : ObservableObject
{
    private readonly IClientDbContext expenseService;

    public MainPageViewModel(IClientDbContext expenseService)
    {
        this.expenseService = expenseService;
    }

    public async Task InitializzaVmAsync()
    {
        await expenseService.RefreshItemsAsync();
        var lista = await expenseService.GetItemsAsync();
        Expenses.ReplaceAll(lista);
    }


    [ObservableProperty]
    ConcurrentObservableCollection<Expense> expenses = new(); //fornita da DataSync - Una ObservableCollection che può essere modificata da più thread contemporaneamente

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    Expense spesaInEditing = new() { Data = DateTime.Now.Date };

    [RelayCommand]
    public async Task ModificaPrimaSpesaAsync()
    {
        var spesa = Expenses.FirstOrDefault();
        if (spesa == null)
            return;
        Random rnd = new Random();
        spesa.Importo = rnd.Next(100);
        await expenseService.UpdateItemAsync(spesa);
        await expenseService.RefreshItemsAsync();
        var lista = await expenseService.GetItemsAsync();
    }


    [RelayCommand]
    public async Task SaveExpenseAsync()
    {
        IsBusy = true;
        if (string.IsNullOrEmpty(spesaInEditing.Id))
        {

            await expenseService.InsertItemAsync(SpesaInEditing);

        }

        else
        {
            await expenseService.UpdateItemAsync(SpesaInEditing);
        }

        await expenseService.RefreshItemsAsync();
        var lista = await expenseService.GetItemsAsync();
        Expenses.ReplaceAll(lista);
        SpesaInEditing = new Expense() { Data = DateTime.Now.Date };
        IsBusy = false;
    }
}
