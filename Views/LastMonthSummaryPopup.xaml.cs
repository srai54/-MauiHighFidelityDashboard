using CommunityToolkit.Maui.Views;
using HighFidelity.Ui.Models;

namespace HighFidelity.Ui.Views;

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
