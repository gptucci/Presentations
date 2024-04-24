using SharedClassLibrary;
using System.Net.Http.Json;

namespace BlazorWebApp.Repo
{
    public class ExpenseRepositoryRest:IExpenseRepository
    {
        private readonly HttpClient httpClient;

        public ExpenseRepositoryRest(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task<List<ExpenseItem>> GetAllExpensesAsync()
        {
            return httpClient.GetFromJsonAsync<List<ExpenseItem>>("api/expenses");
        }
    }
}
