using MauiHighFidelityDashboard.Domain.Models;

namespace MauiHighFidelityDashboard.Presentation.Components;

public partial class SalesChartView : ContentView
{
    public SalesChartView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        SetupTabTaps();
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        ChartCanvas.HeightRequest = 200;
        ChartCanvas.Drawable = new SplineDrawable(GetSampleData());
        ChartCanvas.Invalidate();
    }

    private static List<SalesDataPoint> GetSampleData() =>
    [
        new(1, 2, 3),
        new(2, 13, 12),
        new(3, 3, 5),
        new(4, 6, 14),
        new(5, 12, 10),
        new(6, 27, 17),
    ];

    private void SetupTabTaps()
    {
        var tabs = new[] { (Label)TabDaily, (Label)TabWeekly, (Label)TabMonthly, (Label)TabYearly };
        foreach (var tab in tabs)
        {
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                foreach (var t in tabs)
                {
                    t.Style = t == tab
                        ? (Style)Application.Current!.Resources["TabActiveStyle"]
                        : (Style)Application.Current!.Resources["TabInactiveStyle"];
                }
            };
            tab.GestureRecognizers.Add(tap);
        }
    }
}

public readonly record struct SalesDataPoint(int Day, double Online, double Store);

public class SplineDrawable : IDrawable
{
    private readonly List<SalesDataPoint> _data;

    public SplineDrawable(List<SalesDataPoint> data) => _data = data;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (_data.Count == 0) return;

        float yLabelWidth = 28;
        float padding = 6;
        float chartLeft = yLabelWidth + padding;
        float chartRight = dirtyRect.Width - padding;
        float chartTop = padding;
        float chartBottom = dirtyRect.Height - padding;
        float chartWidth = chartRight - chartLeft;
        float chartHeight = chartBottom - chartTop;

        float yMax = 35;
        float yMin = 0;

        // Draw horizontal grid lines and Y-axis labels
        canvas.FontColor = Color.FromArgb("#B2BEC3");
        canvas.FontSize = 9;
        canvas.StrokeColor = Color.FromArgb("#E8ECF0");
        canvas.StrokeSize = 0.5f;

        for (int v = 0; v <= 35; v += 5)
        {
            float y = chartBottom - (v - yMin) / (yMax - yMin) * chartHeight;
            canvas.DrawLine(chartLeft, y, chartRight, y);
            canvas.DrawString(v.ToString(), 2, y - 5, yLabelWidth, 12, HorizontalAlignment.Left, VerticalAlignment.Center);
        }

        // Draw spline for Online (Blue)
        DrawSpline(canvas, _data.Select(d => (float)d.Online).ToList(),
                   chartLeft, chartBottom, chartWidth, chartHeight, yMin, yMax,
                   Color.FromArgb("#4FC3F7"));

        // Draw spline for Store (Orange)
        DrawSpline(canvas, _data.Select(d => (float)d.Store).ToList(),
                   chartLeft, chartBottom, chartWidth, chartHeight, yMin, yMax,
                   Color.FromArgb("#FF5B1F"));
    }

    private static void DrawSpline(ICanvas canvas, List<float> values,
        float chartLeft, float chartBottom, float chartWidth, float chartHeight,
        float yMin, float yMax, Color color)
    {
        int n = values.Count;
        if (n < 2) return;

        float stepX = chartWidth / (n - 1);
        var points = new PointF[n];
        for (int i = 0; i < n; i++)
        {
            float x = chartLeft + i * stepX;
            float y = chartBottom - (values[i] - yMin) / (yMax - yMin) * chartHeight;
            points[i] = new PointF(x, y);
        }

        canvas.StrokeColor = color;
        canvas.StrokeSize = 2.5f;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeLineJoin = LineJoin.Round;

        var path = new PathF();
        path.MoveTo(points[0]);

        for (int i = 0; i < n - 1; i++)
        {
            float x1 = points[i].X;
            float y1 = points[i].Y;
            float x2 = points[i + 1].X;
            float y2 = points[i + 1].Y;

            float cpx1 = x1 + (x2 - x1) / 3;
            float cpy1 = y1;
            float cpx2 = x2 - (x2 - x1) / 3;
            float cpy2 = y2;

            path.CurveTo(cpx1, cpy1, cpx2, cpy2, x2, y2);
        }

        canvas.DrawPath(path);

        // Draw dots on data points
        canvas.FillColor = Colors.White;
        canvas.StrokeColor = color;
        canvas.StrokeSize = 2;
        foreach (var p in points)
        {
            canvas.FillCircle(p.X, p.Y, 3.5f);
            canvas.DrawCircle(p.X, p.Y, 3.5f);
        }
    }
}
