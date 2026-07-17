using System.Net.Http.Json;
using MauiHighFidelityDashboard.Models;
using MauiHighFidelityDashboard.Services.Interfaces;

namespace MauiHighFidelityDashboard.Services;

public class ApiDashboardDataService : IDashboardDataService
{
    private readonly HttpClient _httpClient;

    public ApiDashboardDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<Result<IReadOnlyList<DashboardCard>>> GetDashboardCardsAsync() =>
        GetListAsync<DashboardCard>("api/dashboard/cards", "dashboard cards");

    public Task<Result<IReadOnlyList<RevenueCardItem>>> GetRevenueCardsAsync() =>
        GetListAsync<RevenueCardItem>("api/dashboard/revenue-cards", "revenue cards");

    public Task<Result<IReadOnlyList<ActivityModel>>> GetActivitiesAsync() =>
        GetListAsync<ActivityModel>("api/dashboard/activities", "activities");

    public Task<Result<IReadOnlyList<OrderModel>>> GetOrdersAsync() =>
        GetListAsync<OrderModel>("api/dashboard/orders", "orders");

    public Task<Result<IReadOnlyList<TrafficModel>>> GetTrafficSourcesAsync() =>
        GetListAsync<TrafficModel>("api/dashboard/traffic", "traffic sources");

    private async Task<Result<IReadOnlyList<T>>> GetListAsync<T>(string endpoint, string resourceName)
    {
        try
        {
            var data = await _httpClient.GetFromJsonAsync<List<T>>(endpoint);
            return Result<IReadOnlyList<T>>.Success(data ?? []);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<T>>.Failure($"Failed to load {resourceName}: {ex.Message}");
        }
    }
}
