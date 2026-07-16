using System.Globalization;

namespace MauiHighFidelityDashboard.Core;

public class StatusBackgroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Process" => Color.FromArgb("#F7284A"),
            "Open" => Color.FromArgb("#2BC155"),
            "On Hold" => Color.FromArgb("#6259CE"),
            _ => Colors.Grey
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
