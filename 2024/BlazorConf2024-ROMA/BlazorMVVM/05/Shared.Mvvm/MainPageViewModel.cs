using Blazing.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Timers;

namespace Shared.Mvvm;

//I ViewModel devono derivare da ViewModelBase.
public partial class MainPageViewModel : ViewModelBase
{
    public MainPageViewModel()
    {
        ExpenseItemInEditing = new() { ExpenseDate = DateTime.Now };
    }

    public override async Task OnInitializedAsync()
    {
        IsBusy = true;
        ExpenseItemItems.Add(new ExpenseItem { Amount = 100, ExpenseNote = "Purtroppo.......", ExpenseDate = DateTime.Now });
        IsBusy = false;
        await base.OnInitializedAsync();
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SalvaEditingCommand))]
    bool isBusy;

    [ObservableProperty]
    ObservableCollection<ExpenseItem> expenseItemItems = new();

    [ObservableProperty]
    int _expenseItemItemsCount;

    bool CanSaveExpense => !IsBusy && ExpenseItemInEditing.Amount>0;


    [ObservableProperty]
    ExpenseItem expenseItemInEditing = new();


    partial void OnExpenseItemInEditingChanged(ExpenseItem value)
    {
        ExpenseItemItemsCount = ExpenseItemItems.Count;
    }

    
    [RelayCommand(CanExecute = nameof(CanSaveExpense))]
    async Task SalvaEditingAsync()
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
