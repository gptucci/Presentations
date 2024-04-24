using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace SharedClassLibrary;

public partial class MainPageViewModel : ObservableObject
{

    public MainPageViewModel(IExpenseRepository expenseRepository)
    {
        SalvaEditingCommand = new AsyncRelayCommand( SaveExpense, () => !IsBusy);
        ExpenseItemInEditing = new() { ExpenseDate = DateTime.Now };
        _expenseRepository = expenseRepository;
    }
    public async Task InitializeAsync()
    {
        ExpenseItemItems = new ObservableCollection<ExpenseItem>(await _expenseRepository.GetAllExpensesAsync());
    }

    [ObservableProperty]
    ObservableCollection<ExpenseItem> _expenseItemItems = new();

    [ObservableProperty]
    int _expenseItemItemsCount;

    [ObservableProperty]
    ExpenseItem _expenseItemInEditing = new();


    //Dei metodi per intercettare il cambiamento di una proprietà
    //https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/generators/observableproperty
    partial void OnExpenseItemInEditingChanged(ExpenseItem value)
    {
        ExpenseItemItemsCount = ExpenseItemItems.Count;
    }
    public AsyncRelayCommand SalvaEditingCommand { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SalvaEditingCommand))]
    bool _isBusy;
    private readonly IExpenseRepository _expenseRepository;

    async Task SaveExpense()
    {

        if (ExpenseItemInEditing.Amount <= 0) return;
        IsBusy = true;
        
        ExpenseItemItems.Add(ExpenseItemInEditing);
        ExpenseItemInEditing = new() { ExpenseDate = DateTime.Now };
        await Task.Delay(1000);
        IsBusy = false;

    }

}
public class ExpenseItem
{
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string ExpenseNote { get; set; }
}