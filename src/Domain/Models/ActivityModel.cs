namespace MauiHighFidelityDashboard.Domain.Models;

public class ActivityModel
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Time { get; init; } = string.Empty;
    public Color DotColor { get; init; } = Colors.Grey;
}
