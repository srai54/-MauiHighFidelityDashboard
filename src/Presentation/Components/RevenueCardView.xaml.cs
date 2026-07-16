namespace MauiHighFidelityDashboard.Presentation.Components;

public partial class RevenueCardView : ContentView
{
    public static readonly BindableProperty CardTitleProperty =
        BindableProperty.Create(nameof(CardTitle), typeof(string), typeof(RevenueCardView), string.Empty);
    public static readonly BindableProperty CardValueProperty =
        BindableProperty.Create(nameof(CardValue), typeof(string), typeof(RevenueCardView), string.Empty);
    public static readonly BindableProperty CardSubtitleProperty =
        BindableProperty.Create(nameof(CardSubtitle), typeof(string), typeof(RevenueCardView), string.Empty);
    public static readonly BindableProperty CardBackgroundProperty =
        BindableProperty.Create(nameof(CardBackground), typeof(Color), typeof(RevenueCardView), Colors.White);
    public static readonly BindableProperty AccentColorProperty =
        BindableProperty.Create(nameof(AccentColor), typeof(Color), typeof(RevenueCardView), Colors.SteelBlue,
            propertyChanged: (b, o, n) => ((RevenueCardView)b).UpdateChart());
    public static readonly BindableProperty ChartTypeProperty =
        BindableProperty.Create(nameof(ChartType), typeof(string), typeof(RevenueCardView), "Bar",
            propertyChanged: (b, o, n) => ((RevenueCardView)b).UpdateChart());

    public string CardTitle { get => (string)GetValue(CardTitleProperty); set => SetValue(CardTitleProperty, value); }
    public string CardValue { get => (string)GetValue(CardValueProperty); set => SetValue(CardValueProperty, value); }
    public string CardSubtitle { get => (string)GetValue(CardSubtitleProperty); set => SetValue(CardSubtitleProperty, value); }
    public Color CardBackground { get => (Color)GetValue(CardBackgroundProperty); set => SetValue(CardBackgroundProperty, value); }
    public Color AccentColor { get => (Color)GetValue(AccentColorProperty); set => SetValue(AccentColorProperty, value); }
    public string ChartType { get => (string)GetValue(ChartTypeProperty); set => SetValue(ChartTypeProperty, value); }

    // Dummy bounce-rate curve per period, selectable from the chip dropdown.
    private static readonly Dictionary<string, float[]> PeriodLineData = new()
    {
        ["Daily"] = [0.3f, 0.9f, 0.25f, 0.7f, 0.2f, 0.85f, 0.35f, 0.6f],
        ["Weekly"] = [0.6f, 0.4f, 0.75f, 0.3f, 0.8f, 0.5f, 0.65f, 0.35f],
        ["Monthly"] = [0.5f, 0.8f, 0.35f, 0.85f, 0.3f, 0.7f, 0.4f, 0.75f],
        ["Yearly"] = [0.2f, 0.35f, 0.5f, 0.45f, 0.65f, 0.6f, 0.8f, 0.9f],
    };

    private string _selectedPeriod = "Monthly";

    public RevenueCardView()
    {
        InitializeComponent();
        UpdateChart();
    }

    private async void OnPeriodChipTapped(object? sender, TappedEventArgs e)
    {
        var choice = await Shell.Current.DisplayActionSheetAsync(
            "Bounce Rate period", "Cancel", null, "Daily", "Weekly", "Monthly", "Yearly");
        if (choice is null or "Cancel") return;

        _selectedPeriod = choice;
        PeriodLabel.Text = $"{choice} ▾";
        UpdateChart();
    }

    private void UpdateChart()
    {
        BarLayout.IsVisible = ChartType == "Bar";
        AreaLayout.IsVisible = ChartType == "Area";
        LineLayout.IsVisible = ChartType == "Line";

        switch (ChartType)
        {
            case "Bar":
                BarCanvas.Drawable = new MiniBarChartDrawable(AccentColor);
                BarCanvas.Invalidate();
                break;
            case "Area":
                AreaCanvas.Drawable = new MiniAreaChartDrawable(AccentColor);
                AreaCanvas.Invalidate();
                break;
            case "Line":
                LineCanvas.Drawable = new MiniLineChartDrawable(AccentColor, PeriodLineData[_selectedPeriod]);
                LineCanvas.Invalidate();
                break;
        }
    }
}

public class MiniBarChartDrawable : IDrawable
{
    private static readonly float[] Values = [0.55f, 0.9f, 0.4f, 1f, 0.7f, 0.3f, 0.6f];
    private readonly Color _color;

    public MiniBarChartDrawable(Color color) => _color = color;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        int n = Values.Length;
        float slot = dirtyRect.Width / n;
        float barWidth = slot * 0.55f;

        canvas.FillColor = _color;
        for (int i = 0; i < n; i++)
        {
            float h = Values[i] * dirtyRect.Height;
            canvas.FillRoundedRectangle(
                i * slot + (slot - barWidth) / 2,
                dirtyRect.Height - h,
                barWidth, h, 1.5f);
        }
    }
}

public class MiniAreaChartDrawable : IDrawable
{
    private static readonly float[] Values = [0.25f, 0.6f, 0.35f, 0.8f, 0.45f, 0.9f, 0.4f, 0.65f];
    private readonly Color _color;

    public MiniAreaChartDrawable(Color color) => _color = color;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var points = MiniChartGeometry.ToPoints(Values, dirtyRect, 4);
        var path = MiniChartGeometry.BuildSpline(points);
        path.LineTo(points[^1].X, dirtyRect.Height);
        path.LineTo(points[0].X, dirtyRect.Height);
        path.Close();

        canvas.FillColor = _color.WithAlpha(0.35f);
        canvas.FillPath(path);

        canvas.StrokeColor = _color;
        canvas.StrokeSize = 2;
        canvas.DrawPath(MiniChartGeometry.BuildSpline(points));
    }
}

public class MiniLineChartDrawable : IDrawable
{
    private readonly float[] _values;
    private readonly Color _color;

    public MiniLineChartDrawable(Color color, float[] values)
    {
        _color = color;
        _values = values;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var points = MiniChartGeometry.ToPoints(_values, dirtyRect, 6);
        canvas.StrokeColor = _color;
        canvas.StrokeSize = 2;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.DrawPath(MiniChartGeometry.BuildSpline(points));
    }
}

internal static class MiniChartGeometry
{
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

    public static PathF BuildSpline(PointF[] points)
    {
        var path = new PathF();
        path.MoveTo(points[0]);
        for (int i = 0; i < points.Length - 1; i++)
        {
            float x1 = points[i].X, y1 = points[i].Y;
            float x2 = points[i + 1].X, y2 = points[i + 1].Y;
            float dx = (x2 - x1) / 2f;
            path.CurveTo(x1 + dx, y1, x2 - dx, y2, x2, y2);
        }
        return path;
    }
}
