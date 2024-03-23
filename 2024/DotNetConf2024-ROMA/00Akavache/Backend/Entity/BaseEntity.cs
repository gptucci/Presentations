using System.ComponentModel.DataAnnotations;

namespace GenoeseExpenseMng.Backend.Simple.Entity;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
}
