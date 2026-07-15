namespace MauiHighFidelityDashboard.Components;

public partial class RevenueCardView : ContentView
{
    public static readonly BindableProperty CardTitleProperty =
        BindableProperty.Create(nameof(CardTitle), typeof(string), typeof(RevenueCardView), string.Empty);
    public static readonly BindableProperty CardValueProperty =
        BindableProperty.Create(nameof(CardValue), typeof(string), typeof(RevenueCardView), string.Empty);
    public static readonly BindableProperty CardSubtitleProperty =
        BindableProperty.Create(nameof(CardSubtitle), typeof(string), typeof(RevenueCardView), string.Empty);
    public static readonly BindableProperty DropdownLabelProperty =
        BindableProperty.Create(nameof(DropdownLabel), typeof(string), typeof(RevenueCardView), string.Empty);
    public static readonly BindableProperty CardBackgroundProperty =
        BindableProperty.Create(nameof(CardBackground), typeof(Color), typeof(RevenueCardView), Colors.White);
    public static readonly BindableProperty ChartBackgroundProperty =
        BindableProperty.Create(nameof(ChartBackground), typeof(Color), typeof(RevenueCardView), Colors.LightGray);

    public string CardTitle { get => (string)GetValue(CardTitleProperty); set => SetValue(CardTitleProperty, value); }
    public string CardValue { get => (string)GetValue(CardValueProperty); set => SetValue(CardValueProperty, value); }
    public string CardSubtitle { get => (string)GetValue(CardSubtitleProperty); set => SetValue(CardSubtitleProperty, value); }
    public string DropdownLabel { get => (string)GetValue(DropdownLabelProperty); set => SetValue(DropdownLabelProperty, value); }
    public Color CardBackground { get => (Color)GetValue(CardBackgroundProperty); set => SetValue(CardBackgroundProperty, value); }
    public Color ChartBackground { get => (Color)GetValue(ChartBackgroundProperty); set => SetValue(ChartBackgroundProperty, value); }

    public RevenueCardView()
    {
        InitializeComponent();
    }
}
