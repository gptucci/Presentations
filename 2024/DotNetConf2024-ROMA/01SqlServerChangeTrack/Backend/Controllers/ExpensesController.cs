using GenoeseExpenseMng.Backend.DbAccess;
using GenoeseExpenseMng.Backend.Simple.Entity;
using GenoeseExpenseMng.Client;
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

    //Prima sincronizzazione (completa)

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _ApplicationDbContext.Expenses.AsNoTracking().ToListAsync());
    }
    //Versione attuale database
    [HttpGet]
    [Route("currversion")]
    public async Task<IActionResult> GetVersioneAttualeAsync()
    {
        FormattableString sql = $"SELECT CHANGE_TRACKING_CURRENT_VERSION() AS Value";
        var versione = await _ApplicationDbContext.Database.SqlQuery<long>(sql).FirstOrDefaultAsync();
        return Ok(versione);
    }

    //Modifiche a partire dalla versione specificata
    [HttpGet]
    [Route("fromversion/{dallaversione:int}")]
    public async Task<IActionResult> GetItemDallaVersione([FromRoute] long dallaversione)
    {
        FormattableString sql = $@"SELECT CT.id,Expenses.Data,Expenses.Importo, Expenses.Nota, CT.SYS_CHANGE_OPERATION AS Operazione 
                                    FROM Expenses 
                                    RIGHT OUTER JOIN
                                    CHANGETABLE(CHANGES Expenses, {dallaversione}) AS CT 
                                    ON
                                    Expenses.id = CT.id";
        var versione = await _ApplicationDbContext.Database.SqlQuery<ModifiedExpense>(sql).ToListAsync();
        return Ok(versione);
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
        expennseFromDB.Deleted = true;
        expennseFromDB.UpdatedAt = DateTimeOffset.Now;

        await _ApplicationDbContext.SaveChangesAsync();
        return NoContent();
    }

}
