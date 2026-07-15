namespace MauiHighFidelityDashboard.Models;

public class DashboardCard
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Icon { get; set; } = string.Empty;
    public Color ThemeColor { get; set; } = Colors.Grey;
}
