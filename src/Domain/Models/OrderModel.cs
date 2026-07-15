namespace MauiHighFidelityDashboard.Domain.Models;

public class OrderModel
{
    public int Invoice { get; init; }
    public string Customer { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Status { get; init; } = string.Empty;
}
