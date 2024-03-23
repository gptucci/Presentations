namespace GenoeseExpenseManager.Client;
//https://github.com/SitholeWB/Files.EntityFrameworkCore.Extensions
public class LatestUpdateAt : BaseEntityForMobile
{

    public string EntityName { get; set; }
    public DateTimeOffset MaxUpdateAt { get; set; }
    public bool Rx { get; set; }
}