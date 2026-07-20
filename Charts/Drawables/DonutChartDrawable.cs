namespace HighFidelity.Ui.Charts.Drawables;

/// <summary>One slice of a donut chart.</summary>
public sealed record DonutSegment(float Percentage, Color Color);

/// <summary>
/// Donut (ring) chart. Segments are drawn clockwise starting at 12 o'clock,
/// in list order, each sized by its percentage of the full circle.
/// </summary>
public class DonutChartDrawable : IDrawable
{
    public IReadOnlyList<DonutSegment> Segments { get; init; } = [];

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var center = new PointF(dirtyRect.Width / 2, dirtyRect.Height / 2);
        float radius = Math.Min(dirtyRect.Width, dirtyRect.Height) / 2f - 6f;
        float innerRadius = radius * 0.60f;
        float startAngle = -90;

        foreach (var segment in Segments)
        {
            float sweepAngle = segment.Percentage / 100f * 360f;
            int steps = Math.Max(12, (int)(Math.Abs(sweepAngle) / 2));

            var path = new PathF();
            float startRad = startAngle * MathF.PI / 180f;

            path.MoveTo(
                center.X + innerRadius * MathF.Cos(startRad),
                center.Y + innerRadius * MathF.Sin(startRad));

            path.LineTo(
                center.X + radius * MathF.Cos(startRad),
                center.Y + radius * MathF.Sin(startRad));

            for (int i = 1; i <= steps; i++)
            {
                float angle = (startAngle + sweepAngle * i / steps) * MathF.PI / 180f;
                path.LineTo(
                    center.X + radius * MathF.Cos(angle),
                    center.Y + radius * MathF.Sin(angle));
            }

            float endRad = (startAngle + sweepAngle) * MathF.PI / 180f;
            path.LineTo(
                center.X + innerRadius * MathF.Cos(endRad),
                center.Y + innerRadius * MathF.Sin(endRad));

            for (int i = steps; i >= 0; i--)
            {
                float angle = (startAngle + sweepAngle * i / steps) * MathF.PI / 180f;
                path.LineTo(
                    center.X + innerRadius * MathF.Cos(angle),
                    center.Y + innerRadius * MathF.Sin(angle));
            }

            path.Close();
            canvas.FillColor = segment.Color;
            canvas.FillPath(path);

            startAngle += sweepAngle;
        }
    }
}
