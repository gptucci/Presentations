using GenoeseExpenseMng.Backend.DbAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace GenoeseExpenseMng.Backend.Controllers;

public class ODataExpenseController : ControllerBase
{

    readonly ILogger<ODataExpenseController> _logger;
    readonly ApplicationDbContext _ApplicationDbContext;
    public ODataExpenseController(ILogger<ODataExpenseController> logger, IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _logger = logger;
        _ApplicationDbContext = dbFactory.CreateDbContext();

    }


    [EnableQuery]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(_ApplicationDbContext.Expenses.IgnoreQueryFilters());
    }
}
