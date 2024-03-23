using GenoeseExpenseMng.Backend.DbAccess;
using GenoeseExpenseMng.Backend.Simple.Entity;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
namespace GenoeseExpenseMng.Backend.Simple.ServiceExtension;

public static class ServiceExtensions
{
    public static void ConfigureOData(this IServiceCollection services)
    {
        services.AddControllers().AddOData(opt => opt.AddRouteComponents("api/odata", GetEdmModel())
            .Filter()
            .Expand()
            .Select()
            .OrderBy()
            .Count()
            .SkipToken()
            .SetMaxTop(1000)
            );
    }

    static IEdmModel GetEdmModel()
    {
        var builder = new ODataConventionModelBuilder();


        builder.EntitySet<Expense>("ODataExpense");
        return builder.GetEdmModel();
    }



    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {


        services.AddDbContextFactory<ApplicationDbContext>((sp, options) =>
            options.EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .UseSqlServer(
               configuration.GetConnectionString("DefaultConnection")
              )
              .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole())));



        return services;
    }



}
