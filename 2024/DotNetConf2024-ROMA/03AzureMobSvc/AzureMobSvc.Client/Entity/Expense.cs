using Microsoft.Datasync.Client;

namespace AzureMobSvc.Client;

public class Expense : DatasyncClientData
{
    public DateTime Data { get; set; }
    public decimal Importo { get; set; }
    public string? Nota { get; set; }
}
