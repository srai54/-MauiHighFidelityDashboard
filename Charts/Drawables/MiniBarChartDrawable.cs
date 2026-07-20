namespace HighFidelity.Ui.Charts.Drawables;

/// <summary>Compact bar chart for analytics cards: normalized values (0..1), one rounded bar each.</summary>
public class MiniBarChartDrawable : IDrawable
{
    private readonly Color _color;
    private readonly float[] _values;

    public MiniBarChartDrawable(Color color, float[] values)
    {
        _color = color;
        _values = values;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        int n = _values.Length;
        float slot = dirtyRect.Width / n;
        float barWidth = slot * 0.6f;

        canvas.FillColor = _color;
        for (int i = 0; i < n; i++)
        {
            float h = _values[i] * dirtyRect.Height;
            canvas.FillRoundedRectangle(
                i * slot + (slot - barWidth) / 2,
                dirtyRect.Height - h,
                barWidth, h, 1.5f);
        }
    }
}
