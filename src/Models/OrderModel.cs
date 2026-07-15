namespace MauiHighFidelityDashboard.Models;

public class OrderModel
{
    public int Invoice { get; set; }
    public string Customer { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
}
