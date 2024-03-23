namespace GenoeseExpenseMng.Backend.Simple.Entity
{
    public class Expense : BaseEntity
    {
        public DateTime Data { get; set; }
        public decimal Importo { get; set; }
        public string? Nota { get; set; }
    }
}
