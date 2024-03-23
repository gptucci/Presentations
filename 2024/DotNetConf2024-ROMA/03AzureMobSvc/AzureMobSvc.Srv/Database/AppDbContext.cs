using AzureMobSvc.Srv.Entity;
using Microsoft.EntityFrameworkCore;

namespace AzureMobSvc.Srv.Database;

public class AppDbContext : DbContext
{
    public DbSet<Expense> Expenses => Set<Expense>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}
