namespace HighFidelity.Ui.Models;

/// <summary>
/// Data model for a revenue card displayed in the dashboard analytics row.
/// Uses hex strings for colors so the model stays UI-framework-agnostic.
/// XAML converts via <c>HexToColorConverter</c> (registered globally in App.xaml).
/// </summary>
public class RevenueCardItem
{
    public string Title { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public string ChartType { get; init; } = "Bar";       // "Bar" | "Area" | "Line"
    public string BackgroundHex { get; init; } = "#FFFFFF";
    public string AccentHex { get; init; } = "#2196F3";
}
