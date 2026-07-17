using CommunityToolkit.Maui.Views;

namespace MauiHighFidelityDashboard.Views;

/// <summary>
/// Styled replacement for the old "Order Summary" OS alert: totals as hero
/// tiles plus a per-status breakdown with proportional share bars.
/// </summary>
public partial class OrderSummaryPopup : Popup
{
    public OrderSummaryPopup(int total, int open, int process, int onHold,
        decimal totalValue, string? searchFilter, int allOrdersCount)
    {
        InitializeComponent();

        SubtitleLabel.Text = string.IsNullOrWhiteSpace(searchFilter)
            ? "Overview of the latest month"
            : $"Filtered by “{searchFilter}” — {total} of {allOrdersCount} orders";

        TotalOrdersLabel.Text = total.ToString();
        TotalValueLabel.Text = $"${totalValue:N2}";

        OpenCountLabel.Text = open.ToString();
        ProcessCountLabel.Text = process.ToString();
        OnHoldCountLabel.Text = onHold.ToString();

        OpenBar.Progress = Share(open, total);
        ProcessBar.Progress = Share(process, total);
        OnHoldBar.Progress = Share(onHold, total);
    }

    private static double Share(int count, int total)
        => total == 0 ? 0 : (double)count / total;

    private async void OnCloseClicked(object? sender, EventArgs e)
        => await CloseAsync();
}
