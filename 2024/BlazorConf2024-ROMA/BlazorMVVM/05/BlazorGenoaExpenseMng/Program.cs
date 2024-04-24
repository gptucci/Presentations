using Blazing.Mvvm;
using Blazing.Mvvm.Infrastructure;
using BlazorGenoaExpenseMng.Components;
using Shared.Mvvm;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<MainPageViewModel>();
builder.Services.AddScoped<SecondaPaginaViewModel>();


builder.Services.AddMvvmNavigation(options =>
{
    options.HostingModel = BlazorHostingModel.WebApp;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
