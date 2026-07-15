using MauiHighFidelityDashboard.ViewModels;

namespace MauiHighFidelityDashboard.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
