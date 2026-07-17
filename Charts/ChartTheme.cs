namespace MauiHighFidelityDashboard.Charts;

/// <summary>
/// Single source of truth for chart colors and series styling.
/// XAML references these via <c>{x:Static charts:ChartTheme.*}</c> and drawables use
/// them directly, so a palette change lands everywhere at once.
/// </summary>
public static class ChartTheme
{
    // Sales spline series — the legend dots in SalesChartView.xaml use the same values.
    public static Color SalesOnline { get; } = Color.FromArgb("#4FC3F7");
    public static Color SalesStore { get; } = Color.FromArgb("#FFA726");

    // Area fill under each sales series (softer wash than the stroke).
    public const float SalesOnlineFillAlpha = 0.14f;
    public const float SalesStoreFillAlpha = 0.20f;

    // Axis scaffolding shared by in-canvas charts.
    public static Color GridLine { get; } = Color.FromArgb("#EDF0F5");
    public static Color AxisText { get; } = Color.FromArgb("#9AA3B7");
}
