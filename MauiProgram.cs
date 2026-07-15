using CommunityToolkit.Maui;
using MauiHighFidelityDashboard.Core.ViewModels;
using MauiHighFidelityDashboard.Domain.Interfaces;
using MauiHighFidelityDashboard.Infrastructure.Data;
using MauiHighFidelityDashboard.Presentation.Views;
using Microsoft.Extensions.Logging;

namespace MauiHighFidelityDashboard;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Data Services — swap StaticDashboardDataService for ApiDashboardDataService
        // when a backend API is available
        builder.Services.AddSingleton<IDashboardDataService, StaticDashboardDataService>();
        // builder.Services.AddSingleton<IDashboardDataService>(sp =>
        // {
        //     var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };
        //     return new ApiDashboardDataService(client);
        // });

        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
