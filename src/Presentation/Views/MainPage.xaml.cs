using MauiHighFidelityDashboard.Core.ViewModels;

namespace MauiHighFidelityDashboard.Presentation.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();

        // Keep the dashboard anchored at the top on launch; without this WinUI can
        // scroll the first focusable input (the order search box) into view.
        Dispatcher.Dispatch(() =>
        {
            try { _ = MainScroll.ScrollToAsync(0, 0, false); }
            catch { /* scrolling to origin is best-effort */ }
        });

        // TEMP: automated UI verification hook
        if (Environment.GetEnvironmentVariable("DASH_TEST_DETAIL") == "1")
            await Shell.Current.GoToAsync("detail?title=Widgets");
    }
}
