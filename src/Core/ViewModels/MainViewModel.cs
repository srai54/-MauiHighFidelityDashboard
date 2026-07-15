using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using MauiHighFidelityDashboard.Domain.Interfaces;
using MauiHighFidelityDashboard.Domain.Models;

namespace MauiHighFidelityDashboard.Core.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly IDashboardDataService _dataService;

    public ObservableCollection<DashboardCard> DashboardCards { get; } = [];
    public ObservableCollection<ActivityModel> Activities { get; } = [];
    public ObservableCollection<OrderModel> Orders { get; } = [];
    public ObservableCollection<TrafficModel> TrafficSources { get; } = [];
    public ObservableCollection<SalesData> SalesDataPoints { get; } = [];

    public string CurrentMonthEarnings => "$3,468.96";
    public string CurrentMonthSales => "82";
    public string DashboardDisplayTitle => "Dashboard";
    public string DashboardSubtitle => "Overview of Latest Month";
    public bool IsDataLoaded { get; private set; }

    public MainViewModel(IDashboardDataService dataService)
    {
        _dataService = dataService;
        Title = DashboardDisplayTitle;
    }

    public async Task InitializeAsync()
    {
        if (IsDataLoaded) return;
        await LoadDataCommand.ExecuteAsync(null);
        IsDataLoaded = true;
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        await Task.WhenAll(
            LoadDashboardCardsAsync(),
            LoadActivitiesAsync(),
            LoadOrdersAsync(),
            LoadTrafficSourcesAsync(),
            LoadSalesDataAsync()
        );

        IsBusy = false;
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsDataLoaded = false;
        await InitializeAsync();
    }

    [RelayCommand]
    private async Task NavigateAsync(MenuItemModel? item)
    {
        if (item is null) return;
        await Shell.Current.GoToAsync($"detail?title={item.Label}");
    }

    [RelayCommand]
    private async Task LastMonthSummaryAsync()
    {
        // Placeholder for last month summary action
        await Task.CompletedTask;
    }

    private async Task LoadDashboardCardsAsync()
    {
        var result = await _dataService.GetDashboardCardsAsync();
        if (result.IsFailure) return;

        DashboardCards.Clear();
        foreach (var card in result.Data!)
            DashboardCards.Add(card);
    }

    private async Task LoadActivitiesAsync()
    {
        var result = await _dataService.GetActivitiesAsync();
        if (result.IsFailure) return;

        Activities.Clear();
        foreach (var activity in result.Data!)
            Activities.Add(activity);
    }

    private async Task LoadOrdersAsync()
    {
        var result = await _dataService.GetOrdersAsync();
        if (result.IsFailure) return;

        Orders.Clear();
        foreach (var order in result.Data!)
            Orders.Add(order);
    }

    private async Task LoadTrafficSourcesAsync()
    {
        var result = await _dataService.GetTrafficSourcesAsync();
        if (result.IsFailure) return;

        TrafficSources.Clear();
        foreach (var source in result.Data!)
            TrafficSources.Add(source);
    }

    private async Task LoadSalesDataAsync()
    {
        var result = await _dataService.GetSalesDataAsync();
        if (result.IsFailure) return;

        SalesDataPoints.Clear();
        foreach (var dataPoint in result.Data!)
            SalesDataPoints.Add(dataPoint);
    }
}
