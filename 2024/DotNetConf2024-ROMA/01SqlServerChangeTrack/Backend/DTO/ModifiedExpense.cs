namespace GenoeseExpenseMng.Client
{
    public class ModifiedExpense
    {

        public Guid Id { get; set; }

        public DateTime? Data { get; set; }
        public decimal? Importo { get; set; }
        public string? Nota { get; set; }
        public string Operazione { get; set; } //I= insert, U= update, D= delete
    }
}
