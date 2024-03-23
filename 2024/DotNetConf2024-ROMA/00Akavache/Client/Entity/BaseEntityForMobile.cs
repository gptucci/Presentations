using SQLite;

namespace MauiAppAkavache.Client;

public abstract class BaseEntityForMobile
{
    [PrimaryKey]
    public Guid Id { get; set; }


    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
}
