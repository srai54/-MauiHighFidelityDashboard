using System.Globalization;

namespace MauiHighFidelityDashboard.Core.Converters;

public class StatusColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Process" => Color.FromArgb("#F44336"),
            "Open" => Color.FromArgb("#4CAF50"),
            "On Hold" => Color.FromArgb("#2196F3"),
            _ => Colors.Grey
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
