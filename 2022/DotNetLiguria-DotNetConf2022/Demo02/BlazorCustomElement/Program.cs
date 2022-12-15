using BlazorCustomElement.Data;
using BlazorCustomElement.Pages;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddSingleton<DogsService>();
builder.RootComponents.RegisterCustomElement<DogsList>("dogs-list"); //Per registrae il custom element

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();