namespace GenoeseExpenseManager.Client;

public class ODataResponse<T>
{
    public List<T> value { get; set; }
}
