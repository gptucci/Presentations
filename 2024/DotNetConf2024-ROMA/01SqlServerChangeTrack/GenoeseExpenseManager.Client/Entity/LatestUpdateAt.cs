namespace GenoeseExpenseManager.Client
{
    internal class LatestUpdateAt : BaseEntityForMobile
    {

        public string NomeDati { get; set; }
        public DateTimeOffset MaxUpdateAt { get; set; }
    }
}
