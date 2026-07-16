using CommunityToolkit.Maui;
using MauiHighFidelityDashboard.Core.ViewModels;
using MauiHighFidelityDashboard.Domain.Interfaces;
using MauiHighFidelityDashboard.Infrastructure.Data;
using MauiHighFidelityDashboard.Presentation.Views;
using Microsoft.Extensions.Logging;
#if WINDOWS
using Microsoft.Maui.LifecycleEvents;
#endif

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

#if WINDOWS
        // Launch maximized so the dashboard opens at its designed proportions.
        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddWindows(windows => windows.OnWindowCreated(window =>
            {
                var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
                if (appWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter)
                    presenter.Maximize();
            }));
        });
#endif

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
        builder.Services.AddTransient<DetailViewModel>();
        builder.Services.AddTransient<DetailPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
