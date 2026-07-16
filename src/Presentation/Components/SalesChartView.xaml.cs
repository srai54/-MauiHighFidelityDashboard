namespace MauiHighFidelityDashboard.Presentation.Components;

public partial class SalesChartView : ContentView
{
    // Dummy series per period — swapping tabs swaps the dataset and x-axis labels.
    private static readonly Dictionary<string, (float[] Online, float[] Store, string[] XLabels)> Series = new()
    {
        ["DAILY"] = (
            [2, 6, 18, 22, 12, 7, 12, 15, 9, 6, 24, 4],
            [3, 8, 12, 16, 13, 10, 13, 16, 12, 16, 28, 17],
            ["1", "2", "3", "4", "5", "6"]),
        ["WEEKLY"] = (
            [5, 12, 8, 20, 26, 14, 10, 18, 22, 12, 16, 9],
            [8, 6, 14, 11, 18, 22, 16, 12, 26, 20, 12, 15],
            ["W1", "W2", "W3", "W4", "W5", "W6"]),
        ["MONTHLY"] = (
            [10, 14, 22, 18, 26, 20, 30, 24, 16, 21, 27, 19],
            [6, 10, 15, 21, 17, 25, 20, 28, 23, 18, 24, 30],
            ["Jan", "Feb", "Mar", "Apr", "May", "Jun"]),
        ["YEARLY"] = (
            [8, 11, 15, 13, 20, 24, 22, 28, 25, 30, 27, 32],
            [5, 9, 12, 16, 14, 19, 23, 21, 26, 24, 29, 26],
            ["2020", "2021", "2022", "2023", "2024", "2025"]),
    };

    public SalesChartView()
    {
        InitializeComponent();
        ApplyPeriod("DAILY");
        SetupTabTaps();
    }

    private void ApplyPeriod(string period)
    {
        var (online, store, labels) = Series[period];
        ChartCanvas.Drawable = new SplineDrawable(online, store, labels);
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
                ApplyPeriod(tab.Label.Text);
            };
            tab.Label.GestureRecognizers.Add(tap);
        }
    }
}

public class SplineDrawable : IDrawable
{
    private readonly float[] _online;
    private readonly float[] _store;
    private readonly string[] _xLabels;

    private const float YMax = 35f;

    private static readonly Color OnlineColor = Color.FromArgb("#4FC3F7");
    private static readonly Color StoreColor = Color.FromArgb("#FFA726");
    private static readonly Color GridColor = Color.FromArgb("#EDF0F5");
    private static readonly Color AxisTextColor = Color.FromArgb("#9AA3B7");

    public SplineDrawable(float[] online, float[] store, string[] xLabels)
    {
        _online = online;
        _store = store;
        _xLabels = xLabels;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float left = 30, right = dirtyRect.Width - 8, top = 8, bottom = dirtyRect.Height - 20;
        float width = right - left, height = bottom - top;
        if (width <= 0 || height <= 0) return;

        canvas.FontSize = 9;
        canvas.FontColor = AxisTextColor;
        canvas.StrokeColor = GridColor;
        canvas.StrokeSize = 1;

        for (int v = 0; v <= (int)YMax; v += 5)
        {
            float y = bottom - v / YMax * height;
            canvas.DrawLine(left, y, right, y);
            canvas.DrawString(v.ToString(), 0, y - 6, 24, 12, HorizontalAlignment.Right, VerticalAlignment.Center);
        }

        for (int i = 0; i < _xLabels.Length; i++)
        {
            float x = left + i / (float)(_xLabels.Length - 1) * width;
            canvas.DrawString(_xLabels[i], x - 16, bottom + 5, 32, 12, HorizontalAlignment.Center, VerticalAlignment.Top);
        }

        DrawSeries(canvas, _store, StoreColor, 0.20f, left, bottom, width, height);
        DrawSeries(canvas, _online, OnlineColor, 0.14f, left, bottom, width, height);
    }

    private static void DrawSeries(ICanvas canvas, float[] values, Color color, float fillAlpha,
        float left, float bottom, float width, float height)
    {
        int n = values.Length;
        if (n < 2) return;

        var points = new PointF[n];
        for (int i = 0; i < n; i++)
        {
            points[i] = new PointF(
                left + i * width / (n - 1),
                bottom - values[i] / YMax * height);
        }

        var curve = BuildSpline(points);

        // Soft area fill under the curve
        var fill = BuildSpline(points);
        fill.LineTo(points[^1].X, bottom);
        fill.LineTo(points[0].X, bottom);
        fill.Close();
        canvas.FillColor = color.WithAlpha(fillAlpha);
        canvas.FillPath(fill);

        canvas.StrokeColor = color;
        canvas.StrokeSize = 2;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeLineJoin = LineJoin.Round;
        canvas.DrawPath(curve);

        canvas.FillColor = Colors.White;
        canvas.StrokeSize = 1.6f;
        foreach (var p in points)
        {
            canvas.FillCircle(p.X, p.Y, 2.6f);
            canvas.DrawCircle(p.X, p.Y, 2.6f);
        }
    }

    private static PathF BuildSpline(PointF[] points)
    {
        var path = new PathF();
        path.MoveTo(points[0]);
        for (int i = 0; i < points.Length - 1; i++)
        {
            float x1 = points[i].X, y1 = points[i].Y;
            float x2 = points[i + 1].X, y2 = points[i + 1].Y;
            float dx = (x2 - x1) / 2.2f;
            path.CurveTo(x1 + dx, y1, x2 - dx, y2, x2, y2);
        }
        return path;
    }
}
