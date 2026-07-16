using CommunityToolkit.Maui.Views;
using MauiHighFidelityDashboard.Models;

namespace MauiHighFidelityDashboard.Views;

public partial class ConfirmDeletePopup : Popup
{
    public ConfirmDeletePopup(OrderModel order)
    {
        InitializeComponent();
        MessageLabel.FormattedText = new FormattedString
        {
            Spans =
            {
                new Span { Text = "Are you sure you want to delete invoice " },
                new Span { Text = $"#{order.Invoice}", FontAttributes = FontAttributes.Bold },
                new Span { Text = " for " },
                new Span { Text = order.Customer, FontAttributes = FontAttributes.Bold },
                new Span { Text = $" ({order.Country}, ${order.Price:F0})?" },
            }
        };
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
        => await CloseAsync(false);

    private async void OnDeleteClicked(object? sender, EventArgs e)
        => await CloseAsync(true);
}
