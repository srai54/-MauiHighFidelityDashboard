using CommunityToolkit.Maui.Views;

namespace HighFidelity.Ui.Views;

/// <summary>
/// Two-step add-order popup: a chooser (Quick Add with generated sample values,
/// or Manual Entry) followed by the form when manual is picked.
/// Returns (customer, country, price, status) either way.
/// </summary>
public partial class AddOrderPopup : Popup
{
    private static readonly Random Rng = new();

    private static readonly string[] SampleCustomers =
    [
        "Aarav Mehta", "Liam Carter", "Sofia Rossi", "Emma Novak",
        "Kenji Sato", "Priya Sharma", "Lucas Meyer", "Olivia Brown",
    ];

    private static readonly string[] SampleCountries =
    [
        "India", "USA", "Italy", "Germany", "Japan", "Brazil", "Korea", "Russia",
    ];

    private static readonly string[] Statuses = ["Open", "Process", "On Hold"];

    public AddOrderPopup()
    {
        InitializeComponent();
        FitToWindow();
    }

    // Cap the popup to the host window so the footer (Cancel / Save) can never be
    // clipped off screen in small or non-maximized windows; the middle section
    // scrolls instead.
    private void FitToWindow()
    {
        var window = Application.Current?.Windows.FirstOrDefault();

        double winH = window?.Height ?? 0;
        if (double.IsNaN(winH) || winH < 400) winH = 700;
        // Header ≈ 72, footer ≈ 66, dividers + popup margins ≈ 90
        ContentScroll.MaximumHeightRequest = Math.Max(220, winH - 230);

        double winW = window?.Width ?? 0;
        if (double.IsNaN(winW) || winW < 300) winW = 800;
        RootBorder.WidthRequest = Math.Min(430, winW - 32);
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
        => await CloseAsync();

    // Quick Add: close immediately with a generated sample order.
    private async void OnQuickAddTapped(object? sender, TappedEventArgs e)
    {
        var customer = SampleCustomers[Rng.Next(SampleCustomers.Length)];
        var country = SampleCountries[Rng.Next(SampleCountries.Length)];
        var price = (decimal)Rng.Next(120, 2401);
        var status = Statuses[Rng.Next(Statuses.Length)];

        await CloseAsync((customer, country, price, status));
    }

    private void OnManualTapped(object? sender, TappedEventArgs e)
        => ShowForm(true);

    private void OnBackClicked(object? sender, EventArgs e)
        => ShowForm(false);

    private void ShowForm(bool form)
    {
        ChoicePanel.IsVisible = !form;
        FormPanel.IsVisible = form;
        BackButton.IsVisible = form;
        AddButton.IsVisible = form;
        HeaderSubtitle.Text = form
            ? "It will be appended at the end of the order list"
            : "Choose how you want to add it";
    }

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
