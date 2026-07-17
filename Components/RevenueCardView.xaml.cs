using CommunityToolkit.Maui.Views;
using MauiHighFidelityDashboard.Charts;
using MauiHighFidelityDashboard.Charts.Drawables;
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
        CardValue = ChartData.BounceRateByPeriod[choice].Value;
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
                BarCanvas.Drawable = new MiniBarChartDrawable(AccentColor, ChartData.RevenueBarValues);
                BarCanvas.Invalidate();
                break;
            case "Area":
                AreaCanvas.Drawable = new MiniAreaChartDrawable(AccentColor, ChartData.PageViewAreaValues);
                AreaCanvas.Invalidate();
                break;
            case "Line":
                LineCanvas.Drawable = new MiniLineChartDrawable(AccentColor, ChartData.BounceRateByPeriod[_selectedPeriod].Curve);
                LineCanvas.Invalidate();
                break;
        }
    }
}
