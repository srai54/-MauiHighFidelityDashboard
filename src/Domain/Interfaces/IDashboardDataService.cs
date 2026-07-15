using MauiHighFidelityDashboard.Domain.Common;
using MauiHighFidelityDashboard.Domain.Models;

namespace MauiHighFidelityDashboard.Domain.Interfaces;

public interface IDashboardDataService
{
    Task<Result<IReadOnlyList<DashboardCard>>> GetDashboardCardsAsync();
    Task<Result<IReadOnlyList<ActivityModel>>> GetActivitiesAsync();
    Task<Result<IReadOnlyList<OrderModel>>> GetOrdersAsync();
    Task<Result<IReadOnlyList<TrafficModel>>> GetTrafficSourcesAsync();
    Task<Result<IReadOnlyList<SalesData>>> GetSalesDataAsync();
}
