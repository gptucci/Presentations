using SQLite;

namespace GenoeseExpenseManager.Client;

public abstract class BaseEntityForMobile
{
    [PrimaryKey]
    public Guid Id { get; set; }


    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;

    public bool Deleted { get; set; }
}
