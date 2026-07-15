using System.Globalization;

namespace MauiHighFidelityDashboard.Presentation.Components;

public partial class SalesChartView : ContentView
{
    public SalesChartView()
    {
        InitializeComponent();
    }
}

public class SalesBarHeightConverter : IValueConverter
{
    private const double MaxBarHeight = 130;
    private const double MaxDataValue = 24;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double val)
            return Math.Max(4, val / MaxDataValue * MaxBarHeight);
        return 4;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
