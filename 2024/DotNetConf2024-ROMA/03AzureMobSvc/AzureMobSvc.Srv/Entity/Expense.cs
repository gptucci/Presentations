using Microsoft.AspNetCore.Datasync.EFCore;

namespace AzureMobSvc.Srv.Entity;

public class Expense : EntityTableData
{
    public DateTime Data { get; set; }
    public decimal Importo { get; set; }
    public string? Nota { get; set; }
}
