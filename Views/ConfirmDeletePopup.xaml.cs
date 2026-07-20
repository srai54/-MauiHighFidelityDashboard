using CommunityToolkit.Maui.Views;
using HighFidelity.Ui.Models;

namespace HighFidelity.Ui.Views;

public partial class ConfirmDeletePopup : Popup
{
    // One popup serves single and bulk delete: a lone order keeps the detailed
    // sentence; multiple orders get a count + total summary.
    public ConfirmDeletePopup(IReadOnlyList<OrderModel> orders)
    {
        InitializeComponent();

        if (orders.Count == 1)
        {
            var order = orders[0];
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
        else
        {
            TitleLabel.Text = "Delete Orders";
            DeleteButton.Text = "Delete Orders";
            MessageLabel.FormattedText = new FormattedString
            {
                Spans =
                {
                    new Span { Text = "Are you sure you want to delete " },
                    new Span { Text = $"{orders.Count} selected orders", FontAttributes = FontAttributes.Bold },
                    new Span { Text = $" (total ${orders.Sum(o => o.Price):N0})?" },
                }
            };
        }
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
        => await CloseAsync(false);

    private async void OnDeleteClicked(object? sender, EventArgs e)
        => await CloseAsync(true);
}
