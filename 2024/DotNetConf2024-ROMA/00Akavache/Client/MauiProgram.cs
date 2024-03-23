using CommunityToolkit.Maui;
using MapsterMapper;
using MauiAppAkavache.Shared;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace MauiAppAkavache.Client
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiApp<App>()
#if DEBUG
            .UseMauiCommunityToolkit()
#else
			.UseMauiCommunityToolkit(options =>
			{
				options.SetShouldSuppressExceptionsInConverters(true);
				options.SetShouldSuppressExceptionsInBehaviors(true);
				options.SetShouldSuppressExceptionsInAnimations(true);
			})
#endif
            .UseMauiCommunityToolkitMediaElement()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<IMapper, ServiceMapper>();
            builder.Services.AddTransient<IClientRepository, ClientRepository>();
            var retryPolicy = HttpPolicyExtensions
                   .HandleTransientHttpError() // HttpRequestException, 5XX and 408
                   .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(retryAttempt));

            builder.Services.AddHttpClient(Constants.httpClient_SyncDati, client =>
            {
                client.BaseAddress = new Uri(Constants.ServiceUri);
            }).AddPolicyHandler(retryPolicy);
            return builder.Build();
        }
    }
}
