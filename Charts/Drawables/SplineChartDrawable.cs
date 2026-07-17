namespace MauiHighFidelityDashboard.Charts.Drawables;

/// <summary>One line of a spline chart: its values, stroke color, and area-fill opacity.</summary>
public sealed record SplineSeries(float[] Values, Color Color, float FillAlpha);

/// <summary>
/// Grid-backed smooth line chart with a soft area fill under each series.
/// Fully data-driven: pass the x-axis labels, the y ceiling, and any number of
/// series (drawn in order, so the last one ends up on top).
/// </summary>
public class SplineChartDrawable : IDrawable
{
    // Spline roundness used by the large dashboard chart (see ChartGeometry.BuildSpline).
    private const float Tension = 2.2f;

    private readonly string[] _xLabels;
    private readonly float _yMax;
    private readonly SplineSeries[] _series;

    public SplineChartDrawable(string[] xLabels, float yMax, params SplineSeries[] series)
    {
        _xLabels = xLabels;
        _yMax = yMax;
        _series = series;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float left = 30, right = dirtyRect.Width - 8, top = 8, bottom = dirtyRect.Height - 20;
        float width = right - left, height = bottom - top;
        if (width <= 0 || height <= 0) return;

        canvas.FontSize = 9;
        canvas.FontColor = ChartTheme.AxisText;
        canvas.StrokeColor = ChartTheme.GridLine;
        canvas.StrokeSize = 1;

        // Grid background behind the series: horizontal line per y tick,
        // vertical line per x tick (per design screenshot)
        for (int v = 0; v <= (int)_yMax; v += 5)
        {
            float y = bottom - v / _yMax * height;
            canvas.DrawLine(left, y, right, y);
            canvas.DrawString(v.ToString(), 0, y - 6, 24, 12, HorizontalAlignment.Right, VerticalAlignment.Center);
        }

        for (int i = 0; i < _xLabels.Length; i++)
        {
            float x = left + i / (float)(_xLabels.Length - 1) * width;
            canvas.DrawLine(x, top, x, bottom);
            canvas.DrawString(_xLabels[i], x - 16, bottom + 5, 32, 12, HorizontalAlignment.Center, VerticalAlignment.Top);
        }

        foreach (var series in _series)
            DrawSeries(canvas, series, left, bottom, width, height);
    }

    private void DrawSeries(ICanvas canvas, SplineSeries series,
        float left, float bottom, float width, float height)
    {
        int n = series.Values.Length;
        if (n < 2) return;

        var points = new PointF[n];
        for (int i = 0; i < n; i++)
        {
            points[i] = new PointF(
                left + i * width / (n - 1),
                bottom - series.Values[i] / _yMax * height);
        }

        // Soft area fill under the curve
        var fill = ChartGeometry.BuildSpline(points, Tension);
        fill.LineTo(points[^1].X, bottom);
        fill.LineTo(points[0].X, bottom);
        fill.Close();
        canvas.FillColor = series.Color.WithAlpha(series.FillAlpha);
        canvas.FillPath(fill);

        canvas.StrokeColor = series.Color;
        canvas.StrokeSize = 2;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeLineJoin = LineJoin.Round;
        canvas.DrawPath(ChartGeometry.BuildSpline(points, Tension));
    }
}
