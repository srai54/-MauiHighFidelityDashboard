namespace MauiHighFidelityDashboard.Domain.Models;

public class TrafficModel
{
    public string Source { get; init; } = string.Empty;
    public double Percentage { get; init; }
    public Color SegmentColor { get; init; } = Colors.Grey;
}
