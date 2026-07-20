using HighFidelity.Ui.Charts;
using HighFidelity.Ui.Charts.Drawables;

namespace HighFidelity.Ui.Components;

public partial class SalesChartView : ContentView
{
    private bool _showOnline = true;
    private bool _showStore = true;
    private string _currentPeriod = "DAILY";

    public SalesChartView()
    {
        InitializeComponent();
        ApplyPeriod("DAILY");
        SetupTabTaps();
        SetupLegendTaps();
    }

    private void ApplyPeriod(string period)
    {
        _currentPeriod = period;
        var data = ChartData.SalesByPeriod[period];

        // Store is drawn first so Online ends up on top, matching the reference.
        var series = new List<SplineSeries>(2);
        if (_showStore)
            series.Add(new(data.Store, ChartTheme.SalesStore, ChartTheme.SalesStoreFillAlpha));
        if (_showOnline)
            series.Add(new(data.Online, ChartTheme.SalesOnline, ChartTheme.SalesOnlineFillAlpha));

        ChartCanvas.Drawable = new SplineChartDrawable(data.XLabels, ChartData.SalesYAxisMax, [.. series]);
        ChartCanvas.Invalidate();
    }

    private void SetupTabTaps()
    {
        var tabs = new (Label Label, BoxView Underline)[]
        {
            (TabDaily, UnderlineDaily),
            (TabWeekly, UnderlineWeekly),
            (TabMonthly, UnderlineMonthly),
            (TabYearly, UnderlineYearly),
        };

        foreach (var tab in tabs)
        {
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                foreach (var t in tabs)
                {
                    bool active = t.Label == tab.Label;
                    t.Label.Style = (Style)Application.Current!.Resources[active ? "TabActiveStyle" : "TabInactiveStyle"];
                    t.Underline.IsVisible = active;
                }
                // Switching period always returns to the original two-series chart.
                ResetSeriesFilter();
                ApplyPeriod(tab.Label.Text);
            };
            tab.Label.GestureRecognizers.Add(tap);
        }
    }

    private void SetupLegendTaps()
    {
        AddLegendTap(LegendOnlineRow, "Online");
        AddLegendTap(LegendStoreRow, "Store");
    }

    private void ResetSeriesFilter()
    {
        _showOnline = _showStore = true;
        LegendOnlineRow.Opacity = 1;
        LegendStoreRow.Opacity = 1;
    }

    private void AddLegendTap(View row, string series)
    {
        var tap = new TapGestureRecognizer();
        tap.Tapped += (s, e) => ToggleSeries(series);
        row.GestureRecognizers.Add(tap);
    }

    // Tapping a legend dot solos that series (single chart); tapping it again restores both.
    // The choice sticks while switching Daily/Weekly/Monthly/Yearly tabs.
    private void ToggleSeries(string series)
    {
        bool soloOnline = _showOnline && !_showStore;
        bool soloStore = _showStore && !_showOnline;

        if (series == "Online")
        {
            if (soloOnline) _showStore = true;
            else { _showOnline = true; _showStore = false; }
        }
        else
        {
            if (soloStore) _showOnline = true;
            else { _showStore = true; _showOnline = false; }
        }

        LegendOnlineRow.Opacity = _showOnline ? 1 : 0.35;
        LegendStoreRow.Opacity = _showStore ? 1 : 0.35;
        ApplyPeriod(_currentPeriod);
    }
}
