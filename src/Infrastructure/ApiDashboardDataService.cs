using System.Net.Http.Json;
using MauiHighFidelityDashboard.Domain.Common;
using MauiHighFidelityDashboard.Domain.Interfaces;
using MauiHighFidelityDashboard.Domain.Models;

namespace MauiHighFidelityDashboard.Infrastructure;

public class ApiDashboardDataService : IDashboardDataService
{
    private readonly HttpClient _httpClient;

    public ApiDashboardDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<IReadOnlyList<DashboardCard>>> GetDashboardCardsAsync()
    {
        try
        {
            var data = await _httpClient.GetFromJsonAsync<List<DashboardCard>>("api/dashboard/cards");
            return Result<IReadOnlyList<DashboardCard>>.Success(data ?? []);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<DashboardCard>>.Failure($"Failed to load dashboard cards: {ex.Message}");
        }
    }

    public async Task<Result<IReadOnlyList<ActivityModel>>> GetActivitiesAsync()
    {
        try
        {
            var data = await _httpClient.GetFromJsonAsync<List<ActivityModel>>("api/dashboard/activities");
            return Result<IReadOnlyList<ActivityModel>>.Success(data ?? []);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<ActivityModel>>.Failure($"Failed to load activities: {ex.Message}");
        }
    }

    public async Task<Result<IReadOnlyList<OrderModel>>> GetOrdersAsync()
    {
        try
        {
            var data = await _httpClient.GetFromJsonAsync<List<OrderModel>>("api/dashboard/orders");
            return Result<IReadOnlyList<OrderModel>>.Success(data ?? []);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<OrderModel>>.Failure($"Failed to load orders: {ex.Message}");
        }
    }

    public async Task<Result<IReadOnlyList<TrafficModel>>> GetTrafficSourcesAsync()
    {
        try
        {
            var data = await _httpClient.GetFromJsonAsync<List<TrafficModel>>("api/dashboard/traffic");
            return Result<IReadOnlyList<TrafficModel>>.Success(data ?? []);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<TrafficModel>>.Failure($"Failed to load traffic sources: {ex.Message}");
        }
    }

    public async Task<Result<IReadOnlyList<SalesData>>> GetSalesDataAsync()
    {
        try
        {
            var data = await _httpClient.GetFromJsonAsync<List<SalesData>>("api/dashboard/sales");
            return Result<IReadOnlyList<SalesData>>.Success(data ?? []);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<SalesData>>.Failure($"Failed to load sales data: {ex.Message}");
        }
    }
}
