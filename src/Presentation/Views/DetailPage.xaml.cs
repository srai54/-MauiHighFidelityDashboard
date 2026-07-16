using MauiHighFidelityDashboard.Core.ViewModels;

namespace MauiHighFidelityDashboard.Presentation.Views;

[QueryProperty(nameof(SectionTitle), "title")]
public partial class DetailPage : ContentPage
{
    private readonly DetailViewModel _viewModel;

    public string SectionTitle
    {
        set => _viewModel.LoadSection(Uri.UnescapeDataString(value));
    }

    public DetailPage(DetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
