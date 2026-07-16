using CommunityToolkit.Maui.Views;
using MauiHighFidelityDashboard.Models;

namespace MauiHighFidelityDashboard.Views;

public partial class LastMonthSummaryPopup : Popup
{
    public LastMonthSummaryPopup(IReadOnlyList<SummaryStat> stats)
    {
        InitializeComponent();
        BindableLayout.SetItemsSource(StatsHost, stats);
    }

    private async void OnCloseClicked(object? sender, EventArgs e)
        => await CloseAsync();
}
