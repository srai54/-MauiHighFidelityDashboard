namespace MauiHighFidelityDashboard.Domain.Models;

public class DashboardCard
{
    public string Title { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Icon { get; init; } = string.Empty;
    public Color ThemeColor { get; init; } = Colors.Grey;
}
