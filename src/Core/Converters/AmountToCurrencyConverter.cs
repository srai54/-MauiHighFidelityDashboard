using System.Globalization;

namespace MauiHighFidelityDashboard.Core.Converters;

public class AmountToCurrencyConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is decimal amount)
            return string.Format(culture, "{0:C}", amount);
        return "$0.00";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
