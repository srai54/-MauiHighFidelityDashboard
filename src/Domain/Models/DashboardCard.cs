namespace MauiHighFidelityDashboard.Domain.Models;

public class DashboardCard
{
    public string Title { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string AmountDisplay { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;
    public string ThemeColorHex { get; init; } = "#808080";
}
