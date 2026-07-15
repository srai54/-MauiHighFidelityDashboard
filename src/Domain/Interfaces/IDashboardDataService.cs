using MauiHighFidelityDashboard.Domain.Models;

namespace MauiHighFidelityDashboard.Domain.Interfaces;

public interface IDashboardDataService
{
    IReadOnlyList<DashboardCard> GetDashboardCards();
    IReadOnlyList<ActivityModel> GetActivities();
    IReadOnlyList<OrderModel> GetOrders();
    IReadOnlyList<TrafficModel> GetTrafficSources();
    IReadOnlyList<SalesData> GetSalesData();
}
