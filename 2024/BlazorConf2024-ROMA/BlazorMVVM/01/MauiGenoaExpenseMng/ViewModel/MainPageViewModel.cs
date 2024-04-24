using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
namespace MauiGenoaExpenseMng.ViewModel;

public class MainPageViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public MainPageViewModel()
    {
        SalvaEditingCommand = new Command(async () => await SaveExpense(), () => !IsBusy);
        ExpenseItemItems.Add(new ExpenseItem { Amount = 100, ExpenseNote = "Purtroppo.......", ExpenseDate = DateTime.Now });
    }
    bool _isBusy;
    public bool IsBusy
    {
        get
        {
            return _isBusy;
        }
        set
        {
            if (_isBusy == value) return;
            _isBusy = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBusy)));
            SalvaEditingCommand.ChangeCanExecute();
        }
    }

    public ObservableCollection<ExpenseItem> ExpenseItemItems { get; set; } = new();
    public int ExpenseItemItemsCount => ExpenseItemItems.Count;

    ExpenseItem _ExpenseItemInEditing = new() { ExpenseDate = DateTime.Now };
    public ExpenseItem ExpenseItemInEditing
    {
        get
        {
            return _ExpenseItemInEditing;
        }
        set
        {
            _ExpenseItemInEditing= value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExpenseItemInEditing)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExpenseItemItemsCount)));
        }
    }

    public Command SalvaEditingCommand { get; set; }  //Implementa ICommand

    

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
