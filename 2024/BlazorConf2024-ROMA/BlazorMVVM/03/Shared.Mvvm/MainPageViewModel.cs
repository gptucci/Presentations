using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Shared.Mvvm;
//ObservableObject - Indicia che il ViewModel invierà alla View delle notification per valori delle proprietà che variano
//l metodo standard è utilizzare l'evento PropertyChanged
//Il source generator si occuperà di aggiungere il codice boiler-plate per invocare correttamente l'evento
//per questo la classe è partial

public partial class MainPageViewModel : ObservableObject
{


    public MainPageViewModel()
    {
        
    }

    //ObservableProperty - indica che la proprietà deve inviare un evento PropertyChanged quando il suo valore varia
    //Esiste una convenzione da eseguire: qui si specifica come campo e con prima lettera maiiuscolo: verrà tradotto da source generator in una proprietà
    //il stesso nome ma prima lettera maiuscola 
    [ObservableProperty] 
    [NotifyCanExecuteChangedFor(nameof(SalvaEditingCommand))]
    bool isBusy;



    //ObservableCollection emette un evento CollectionChanged
    //Quando alla lista degli elementi
    //- si aggiunge un nuovo elemento
    //-si elimina un unvio elemento
    [ObservableProperty]
    ObservableCollection<ExpenseItem> expenseItemItems = new();

    [ObservableProperty]
    int expenseItemItemsCount;

    [ObservableProperty]
    ExpenseItem expenseItemInEditing = new();

    //Metodo generto da sourcegenerator quando si varia il valore della proprietà ExpenseItemInEditing
    partial void OnExpenseItemInEditingChanged(ExpenseItem value)
    {
        ExpenseItemItemsCount = ExpenseItemItems.Count;
    }

    //Function che restitusce True se il commando può essere eseguito
    bool CanSaveExpense => !IsBusy && ExpenseItemInEditing.Amount>0;


    //Anche se nn belissimo sopratutto quando si usa anche Blazor è più duttile mettere la logica di inizializzazione
    //All'interno di un metodo separato
    //Vedi per Blazor prerendering
    public async Task InitializeAsync()
    {
        IsBusy = true;
       
        ExpenseItemItems.Add(new ExpenseItem { Amount = 100, ExpenseNote = "Purtroppo.......", ExpenseDate = DateTime.Now });
        ExpenseItemInEditing = new() { ExpenseDate = DateTime.Now };

        //_Timer = new System.Timers.Timer(2000);

        //_Timer.Elapsed += OnTimedEvent;
        //_Timer.AutoReset = true;
        //_Timer.Enabled = true;

        IsBusy = false;

    }
    //private void OnTimedEvent(Object source, ElapsedEventArgs e)
    //{
    //    ExpenseItemItems.Add(new ExpenseItem() { Amount = 100, ExpenseNote = "Spesa avvenuta il " + DateTime.Now.ToString(), ExpenseDate = DateTime.Now.Date });
    //}
    

    //private System.Timers.Timer _Timer;
    

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
