using CommunityToolkit.Maui;
using MauiHighFidelityDashboard.ViewModels;
using MauiHighFidelityDashboard.Services.Interfaces;
using MauiHighFidelityDashboard.Services;
using MauiHighFidelityDashboard.Views;
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
                fonts.AddFont("fa-solid-900.ttf", "FontAwesome");
            });

#if WINDOWS
        // WinUI paints an accent-blue underline on focused text boxes; the app-level
        // theme overrides in Platforms/Windows/App.xaml don't reach it, so override the
        // resource per control. Must run after Loaded — inserting into a ResourceDictionary
        // before the control joins the visual tree crashes with COMException 0x800F0902.
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("FlatFocusVisual", (handler, view) =>
        {
            var textBox = handler.PlatformView;
            textBox.Loaded += (s, e) =>
            {
                try
                {
                    textBox.Resources["TextControlBorderThemeThicknessFocused"] = new Microsoft.UI.Xaml.Thickness(0);
                    textBox.Resources["TextControlBorderBrushFocused"] =
                        new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
                    textBox.Resources["TextControlBorderBrushPointerOver"] =
                        new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
                }
                catch
                {
                    // Purely cosmetic; never crash the app over a focus visual.
                }
            };
        });

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
        builder.Services.AddSingleton<IPrintService, PrintService>();
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
