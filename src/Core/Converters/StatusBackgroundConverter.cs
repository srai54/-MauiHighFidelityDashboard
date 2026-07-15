using System.Globalization;

namespace MauiHighFidelityDashboard.Core.Converters;

public class StatusBackgroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Process" => Color.FromArgb("#FFF0F0"),
            "Open" => Color.FromArgb("#F0FFF0"),
            "On Hold" => Color.FromArgb("#F0F5FF"),
            _ => Colors.Transparent
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
