using BlazorWebApp.Repo;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SharedClassLibrary;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped<MainPageViewModel>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepositoryRest>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

await builder.Build().RunAsync();
