namespace MauiHighFidelityDashboard.Presentation.Components;

public partial class TrafficChartView : ContentView
{
    public TrafficChartView()
    {
        InitializeComponent();

        DonutCanvas.Drawable = new DonutDrawable
        {
            Segments =
            [
                new DonutSegment { Percentage = 34, Color = Color.FromArgb("#2196F3") },
                new DonutSegment { Percentage = 55, Color = Color.FromArgb("#FF5722") },
                new DonutSegment { Percentage = 11, Color = Color.FromArgb("#FFC107") },
            ]
        };
    }
}

public class DonutSegment
{
    public float Percentage { get; set; }
    public Color Color { get; set; } = Colors.Grey;
}

public class DonutDrawable : IDrawable
{
    public List<DonutSegment> Segments { get; set; } = [];

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var center = new PointF(dirtyRect.Width / 2, dirtyRect.Height / 2);
        float radius = Math.Min(dirtyRect.Width, dirtyRect.Height) / 2f - 4f;
        float innerRadius = radius * 0.55f;
        float startAngle = -90;

        foreach (var segment in Segments)
        {
            float sweepAngle = segment.Percentage / 100f * 360f;
            int steps = Math.Max(8, (int)(Math.Abs(sweepAngle) / 3));

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
