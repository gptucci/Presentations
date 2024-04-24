using SharedClassLibrary;

namespace BlazorWebApp.Repo
{
    public class ExpenseRepositoryLocalDB : IExpenseRepository
    {
        public Task<List<ExpenseItem>> GetAllExpensesAsync()
        {
            List<ExpenseItem> ExpenseItemItems = new();
            ExpenseItemItems.Add(new ExpenseItem { Amount = 100, ExpenseNote = "Accesso diretto DB", ExpenseDate = DateTime.Now });
            return Task.FromResult(ExpenseItemItems);
            
        }

        
    }
}
