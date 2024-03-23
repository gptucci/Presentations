using GenoeseExpenseMng.Backend.Simple.Entity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace GenoeseExpenseMng.Backend.DbAccess;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option) : base(option)
    {
        this.Database.SetCommandTimeout(90);
    }


    public DbSet<Expense> Expenses { get; set; }




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }


}
