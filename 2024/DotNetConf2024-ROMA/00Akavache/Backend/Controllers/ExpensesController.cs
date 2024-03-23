using GenoeseExpenseMng.Backend.DbAccess;
using GenoeseExpenseMng.Backend.Simple.Entity;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GenoeseExpenseMng.Backend.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{

    readonly ILogger<ExpensesController> _logger;
    readonly ApplicationDbContext _ApplicationDbContext;
    public ExpensesController(ILogger<ExpensesController> logger, IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _logger = logger;
        _ApplicationDbContext = dbFactory.CreateDbContext();

    }


    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _ApplicationDbContext.Expenses.AsNoTracking().ToListAsync());
    }


    [HttpPost]
    [Route("")]
    public async Task<IActionResult> InserimentoModificaAsync([FromBody] Expense mExpense)
    {
        if (mExpense.Id == Guid.Empty)
        {
            mExpense.Id = Guid.NewGuid();
        }

        var expennseFromDB = await _ApplicationDbContext.Expenses.Where(x => x.Id == mExpense.Id).FirstOrDefaultAsync();
        if (expennseFromDB == null)
        {
            _ApplicationDbContext.Expenses.Add(mExpense);
            await _ApplicationDbContext.SaveChangesAsync();
            return Ok(mExpense);
        }
        expennseFromDB.Adapt(mExpense);
        await _ApplicationDbContext.SaveChangesAsync();

        return Ok(expennseFromDB);
    }

    [HttpDelete]
    [Route("{Id}")]
    public async Task<IActionResult> CancellazioneAsync([FromRoute] Guid Id)
    {
        if (Id == Guid.Empty)
        {
            return NoContent();
        }

        var expennseFromDB = await _ApplicationDbContext.Expenses.Where(x => x.Id == Id).FirstOrDefaultAsync();
        if (expennseFromDB == null)
        {
            return NoContent();

        }
        _ApplicationDbContext.Expenses.Remove(expennseFromDB);
        

        await _ApplicationDbContext.SaveChangesAsync();
        return NoContent();
    }
    
}
