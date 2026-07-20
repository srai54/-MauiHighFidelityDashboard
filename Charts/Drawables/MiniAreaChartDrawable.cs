namespace HighFidelity.Ui.Charts.Drawables;

/// <summary>Compact angular area chart for analytics cards: filled wave with a stroked top edge.</summary>
public class MiniAreaChartDrawable : IDrawable
{
    private readonly Color _color;
    private readonly float[] _values;

    public MiniAreaChartDrawable(Color color, float[] values)
    {
        _color = color;
        _values = values;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var points = ChartGeometry.ToPoints(_values, dirtyRect, 4);
        var path = ChartGeometry.BuildPolyline(points);
        path.LineTo(points[^1].X, dirtyRect.Height);
        path.LineTo(points[0].X, dirtyRect.Height);
        path.Close();

        canvas.FillColor = _color.WithAlpha(0.4f);
        canvas.FillPath(path);

        canvas.StrokeColor = _color;
        canvas.StrokeSize = 2;
        canvas.StrokeLineJoin = LineJoin.Round;
        canvas.DrawPath(ChartGeometry.BuildPolyline(points));
    }
}
