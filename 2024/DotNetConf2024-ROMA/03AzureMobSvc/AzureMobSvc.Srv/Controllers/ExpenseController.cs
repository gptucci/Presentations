using AzureMobSvc.Srv.Database;
using AzureMobSvc.Srv.Entity;
using Microsoft.AspNetCore.Datasync;
using Microsoft.AspNetCore.Datasync.EFCore;
using Microsoft.AspNetCore.Mvc;


namespace AzureMobSvc.Srv.Controllers;

[Route("tables/expense")]
public class ExpenseController : TableController<Expense>
{
    public ExpenseController(AppDbContext context)
        : base(new EntityTableRepository<Expense>(context))
    {
        Options = new TableControllerOptions { EnableSoftDelete = true };
    }
}
