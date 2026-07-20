namespace HighFidelity.Ui.Charts;

/// <summary>Point and path builders shared by every chart drawable.</summary>
public static class ChartGeometry
{
    /// <summary>
    /// Maps normalized values (0..1, where 1 is the top) onto evenly spaced points
    /// across the full width of <paramref name="rect"/>.
    /// </summary>
    public static PointF[] ToPoints(float[] values, RectF rect, float verticalPadding)
    {
        var points = new PointF[values.Length];
        float usable = rect.Height - verticalPadding * 2;
        for (int i = 0; i < values.Length; i++)
        {
            points[i] = new PointF(
                i * rect.Width / (values.Length - 1),
                verticalPadding + (1 - values[i]) * usable);
        }
        return points;
    }

    /// <summary>Straight segments through every point (angular charts).</summary>
    public static PathF BuildPolyline(PointF[] points)
    {
        var path = new PathF();
        path.MoveTo(points[0]);
        for (int i = 1; i < points.Length; i++)
            path.LineTo(points[i]);
        return path;
    }

    /// <summary>
    /// Smooth cubic curve through every point. <paramref name="tension"/> divides the
    /// horizontal control-point offset: 2 gives round curves, higher values flatten them.
    /// </summary>
    public static PathF BuildSpline(PointF[] points, float tension = 2f)
    {
        var path = new PathF();
        path.MoveTo(points[0]);
        for (int i = 0; i < points.Length - 1; i++)
        {
            float x1 = points[i].X, y1 = points[i].Y;
            float x2 = points[i + 1].X, y2 = points[i + 1].Y;
            float dx = (x2 - x1) / tension;
            path.CurveTo(x1 + dx, y1, x2 - dx, y2, x2, y2);
        }
        return path;
    }
}
