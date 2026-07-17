using CommunityToolkit.Maui.Views;
using MauiHighFidelityDashboard.Views;

namespace MauiHighFidelityDashboard.Components;

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

    // Dummy bounce-rate data per period: curve + headline value stay in sync,
    // so picking a period updates the whole card, not just the mini chart.
    private static readonly Dictionary<string, (float[] Curve, string Value)> PeriodData = new()
    {
        ["Daily"] = ([0.30f, 0.70f, 0.25f, 0.80f, 0.35f, 0.60f, 0.20f], "$118"),
        ["Weekly"] = ([0.55f, 0.20f, 0.70f, 0.30f, 0.75f, 0.40f, 0.65f], "$267"),
        ["Monthly"] = ([0.40f, 0.78f, 0.22f, 0.58f, 0.18f, 0.48f, 0.08f], "$432"),
        ["Yearly"] = ([0.15f, 0.35f, 0.28f, 0.55f, 0.45f, 0.75f, 0.90f], "$5,184"),
    };

    private string _selectedPeriod = "Monthly";

    public RevenueCardView()
    {
        InitializeComponent();
        UpdateChart();
    }

    private async void OnPeriodChipTapped(object? sender, TappedEventArgs e)
    {
        var page = Shell.Current.CurrentPage;
        if (page is null) return;

        var result = await page.ShowPopupAsync(new PeriodPickerPopup(_selectedPeriod));
        if (result is not string choice || choice == _selectedPeriod) return;

        _selectedPeriod = choice;
        PeriodLabel.Text = choice;
        CardValue = PeriodData[choice].Value;
        ToolTipProperties.SetText(PeriodChip,
            $"Bounce rate for the {choice.ToLowerInvariant()} period — click to change");
        UpdateChart();
    }

    private void UpdateChart()
    {
        BarLayout.IsVisible = ChartType == "Bar";
        AreaLayout.IsVisible = ChartType == "Area";
        LineLayout.IsVisible = ChartType == "Line";

        // Arrows are stroked Paths (Brush-typed), so they take the accent color here.
        var accentBrush = new SolidColorBrush(AccentColor);
        BarArrow.Stroke = accentBrush;
        AreaArrow.Stroke = accentBrush;
        LineArrow.Stroke = accentBrush;

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
                LineCanvas.Drawable = new MiniLineChartDrawable(AccentColor, PeriodData[_selectedPeriod].Curve);
                LineCanvas.Invalidate();
                break;
        }
    }
}

public class MiniBarChartDrawable : IDrawable
{
    // Valley profile from the reference card: tall start, dip to almost nothing,
    // then a climb that ends on the tallest bar.
    private static readonly float[] Values = [0.85f, 0.50f, 0.56f, 0.10f, 0.24f, 0.42f, 1f];
    private readonly Color _color;

    public MiniBarChartDrawable(Color color) => _color = color;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        int n = Values.Length;
        float slot = dirtyRect.Width / n;
        float barWidth = slot * 0.6f;

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
    // Angular peaks-and-valleys profile matching the reference Page View card:
    // stepped foothills, one dominant peak at ~70% width, then a steep drop.
    private static readonly float[] Values =
        [0.08f, 0.36f, 0.22f, 0.44f, 0.28f, 0.52f, 0.40f, 0.80f, 0.62f, 0.18f, 0.05f];
    private readonly Color _color;

    public MiniAreaChartDrawable(Color color) => _color = color;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var points = MiniChartGeometry.ToPoints(Values, dirtyRect, 4);
        var path = MiniChartGeometry.BuildPolyline(points);
        path.LineTo(points[^1].X, dirtyRect.Height);
        path.LineTo(points[0].X, dirtyRect.Height);
        path.Close();

        canvas.FillColor = _color.WithAlpha(0.4f);
        canvas.FillPath(path);

        canvas.StrokeColor = _color;
        canvas.StrokeSize = 2;
        canvas.StrokeLineJoin = LineJoin.Round;
        canvas.DrawPath(MiniChartGeometry.BuildPolyline(points));
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

    public static PathF BuildPolyline(PointF[] points)
    {
        var path = new PathF();
        path.MoveTo(points[0]);
        for (int i = 1; i < points.Length; i++)
            path.LineTo(points[i]);
        return path;
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
