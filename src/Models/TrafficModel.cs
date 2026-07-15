namespace MauiHighFidelityDashboard.Models;

public class TrafficModel
{
    public string Source { get; set; } = string.Empty;
    public double Percentage { get; set; }
    public Color SegmentColor { get; set; } = Colors.Grey;
}
