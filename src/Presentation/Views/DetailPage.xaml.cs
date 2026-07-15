namespace MauiHighFidelityDashboard.Presentation.Views;

[QueryProperty(nameof(Title), "title")]
public partial class DetailPage : ContentPage
{
    public DetailPage()
    {
        InitializeComponent();
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
