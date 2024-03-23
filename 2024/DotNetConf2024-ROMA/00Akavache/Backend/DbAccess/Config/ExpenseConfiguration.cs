using GenoeseExpenseMng.Backend.DbAccess;
using GenoeseExpenseMng.Backend.Simple.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestioneOrdini.DatabaseContext.ConfigSqlServer;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable(nameof(ApplicationDbContext.Expenses));
        builder.Property(x => x.Importo).HasPrecision(8);

        builder.HasQueryFilter(x => x.Deleted == false);
        builder.Property(x => x.Deleted).HasDefaultValue(false);
    }
}
