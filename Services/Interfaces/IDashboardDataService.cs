using MauiHighFidelityDashboard.Models;

namespace MauiHighFidelityDashboard.Services.Interfaces;

public interface IDashboardDataService
{
    Task<Result<IReadOnlyList<DashboardCard>>> GetDashboardCardsAsync();
    Task<Result<IReadOnlyList<RevenueCardItem>>> GetRevenueCardsAsync();
    Task<Result<IReadOnlyList<ActivityModel>>> GetActivitiesAsync();
    Task<Result<IReadOnlyList<OrderModel>>> GetOrdersAsync();
    Task<Result<IReadOnlyList<TrafficModel>>> GetTrafficSourcesAsync();

    /// <summary>Persists a new order; the backend assigns Id and Invoice.</summary>
    Task<Result<OrderModel>> AddOrderAsync(string customer, string country, decimal price, string status);

    /// <summary>Deletes orders by database Id. Returns the number deleted.</summary>
    Task<Result<int>> DeleteOrdersAsync(IReadOnlyList<int> orderIds);
}
