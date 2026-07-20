namespace HighFidelity.Ui.Charts;

/// <summary>One dataset of the sales spline chart: two series plus the x-axis labels.</summary>
public sealed record SalesSeries(float[] Online, float[] Store, string[] XLabels);

/// <summary>A mini-chart snapshot for one period: the curve and the headline value shown beside it.</summary>
public sealed record PeriodSnapshot(float[] Curve, string Value);

/// <summary>
/// Global chart datasets, declared once and consumed by the chart views.
///
/// Adding a chart tomorrow is a data change, not a code change:
///   1. Declare its dataset here, following the shapes below.
///   2. In the view, hand the dataset to one of the generic drawables in Charts/Drawables.
/// Nothing else in the app needs to know about the new chart.
/// </summary>
public static class ChartData
{
    // ---------- Sales chart (SalesChartView) ----------

    /// <summary>Fixed y-axis ceiling of the sales chart (gridlines drawn every 5 units).</summary>
    public const float SalesYAxisMax = 35f;

    /// <summary>Dummy series per period — switching tabs swaps the dataset and x-axis labels.</summary>
    public static readonly IReadOnlyDictionary<string, SalesSeries> SalesByPeriod = new Dictionary<string, SalesSeries>
    {
        ["DAILY"] = new(
            [2, 6, 18, 22, 12, 7, 12, 15, 9, 6, 24, 4],
            [3, 8, 12, 16, 13, 10, 13, 16, 12, 16, 28, 17],
            ["1", "2", "3", "4", "5", "6"]),
        ["WEEKLY"] = new(
            [5, 12, 8, 20, 26, 14, 10, 18, 22, 12, 16, 9],
            [8, 6, 14, 11, 18, 22, 16, 12, 26, 20, 12, 15],
            ["W1", "W2", "W3", "W4", "W5", "W6"]),
        ["MONTHLY"] = new(
            [10, 14, 22, 18, 26, 20, 30, 24, 16, 21, 27, 19],
            [6, 10, 15, 21, 17, 25, 20, 28, 23, 18, 24, 30],
            ["Jan", "Feb", "Mar", "Apr", "May", "Jun"]),
        ["YEARLY"] = new(
            [8, 11, 15, 13, 20, 24, 22, 28, 25, 30, 27, 32],
            [5, 9, 12, 16, 14, 19, 23, 21, 26, 24, 29, 26],
            ["2020", "2021", "2022", "2023", "2024", "2025"]),
    };

    // ---------- Analytics mini charts (RevenueCardView) ----------

    /// <summary>
    /// Bounce-rate data per period: curve + headline value stay in sync,
    /// so picking a period updates the whole card, not just the mini chart.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, PeriodSnapshot> BounceRateByPeriod = new Dictionary<string, PeriodSnapshot>
    {
        ["Daily"] = new([0.30f, 0.70f, 0.25f, 0.80f, 0.35f, 0.60f, 0.20f], "$118"),
        ["Weekly"] = new([0.55f, 0.20f, 0.70f, 0.30f, 0.75f, 0.40f, 0.65f], "$267"),
        ["Monthly"] = new([0.40f, 0.78f, 0.22f, 0.58f, 0.18f, 0.48f, 0.08f], "$432"),
        ["Yearly"] = new([0.15f, 0.35f, 0.28f, 0.55f, 0.45f, 0.75f, 0.90f], "$5,184"),
    };

    /// <summary>
    /// Revenue Status bars — valley profile from the reference card: tall start,
    /// dip to almost nothing, then a climb that ends on the tallest bar.
    /// </summary>
    public static readonly float[] RevenueBarValues = [0.85f, 0.50f, 0.56f, 0.10f, 0.24f, 0.42f, 1f];

    /// <summary>
    /// Page View wave — angular peaks-and-valleys profile matching the reference:
    /// stepped foothills, one dominant peak at ~70% width, then a steep drop.
    /// </summary>
    public static readonly float[] PageViewAreaValues =
        [0.08f, 0.36f, 0.22f, 0.44f, 0.28f, 0.52f, 0.40f, 0.80f, 0.62f, 0.18f, 0.05f];
}
