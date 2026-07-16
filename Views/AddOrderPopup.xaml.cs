using CommunityToolkit.Maui.Views;

namespace MauiHighFidelityDashboard.Views;

public partial class AddOrderPopup : Popup
{
    public AddOrderPopup()
    {
        InitializeComponent();
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
        => await CloseAsync();

    private async void OnAddClicked(object? sender, EventArgs e)
    {
        var customer = CustomerEntry.Text?.Trim();
        var country = CountryEntry.Text?.Trim();
        var priceText = PriceEntry.Text?.Trim();
        var status = StatusPicker.SelectedItem as string;

        if (string.IsNullOrWhiteSpace(customer) || string.IsNullOrWhiteSpace(country))
        {
            await Shell.Current.DisplayAlertAsync("Missing fields", "Please fill in customer name and country.", "OK");
            return;
        }

        if (!decimal.TryParse(priceText, out var price) || price <= 0)
        {
            await Shell.Current.DisplayAlertAsync("Invalid price", "Please enter a valid positive number.", "OK");
            return;
        }

        if (string.IsNullOrEmpty(status))
        {
            await Shell.Current.DisplayAlertAsync("Missing status", "Please select an order status.", "OK");
            return;
        }

        // Return the order data as a tuple
        await CloseAsync((customer, country, price, status));
    }
}
