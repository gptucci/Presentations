namespace GenoeseExpenseManager.Client
{
    public class Expense : BaseEntityForMobile
    {

        public DateTime Data { get; set; }
        public decimal Importo { get; set; }
        public string? Nota { get; set; }
    }
}
