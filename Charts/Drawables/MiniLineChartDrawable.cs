namespace MauiHighFidelityDashboard.Charts.Drawables;

/// <summary>Compact smooth line chart for analytics cards, with a circular marker at every point.</summary>
public class MiniLineChartDrawable : IDrawable
{
    private readonly Color _color;
    private readonly float[] _values;

    public MiniLineChartDrawable(Color color, float[] values)
    {
        _color = color;
        _values = values;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var points = ChartGeometry.ToPoints(_values, dirtyRect, 6);
        canvas.StrokeColor = _color;
        canvas.StrokeSize = 2;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.DrawPath(ChartGeometry.BuildSpline(points));

        // Circular markers at every data point, like the reference squiggle
        canvas.FillColor = _color;
        canvas.StrokeColor = Colors.White;
        canvas.StrokeSize = 1.5f;
        foreach (var p in points)
        {
            canvas.FillCircle(p.X, p.Y, 3f);
            canvas.DrawCircle(p.X, p.Y, 3f);
        }
    }
}
