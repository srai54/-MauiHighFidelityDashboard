using MauiHighFidelityDashboard.Core.ViewModels;

namespace MauiHighFidelityDashboard.Presentation.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
