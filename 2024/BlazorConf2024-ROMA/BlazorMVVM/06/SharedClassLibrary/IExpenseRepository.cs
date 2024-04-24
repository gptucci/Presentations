using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedClassLibrary
{
    public interface IExpenseRepository
    {
        public Task<List<ExpenseItem>> GetAllExpensesAsync();
    }
}
