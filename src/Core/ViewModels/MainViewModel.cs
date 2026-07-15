using System.Collections.ObjectModel;
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

    public MainViewModel(IDashboardDataService dataService)
    {
        _dataService = dataService;
        Title = DashboardDisplayTitle;
        LoadData();
    }

    private void LoadData()
    {
        DashboardCards.Clear();
        foreach (var card in _dataService.GetDashboardCards())
            DashboardCards.Add(card);

        Activities.Clear();
        foreach (var activity in _dataService.GetActivities())
            Activities.Add(activity);

        Orders.Clear();
        foreach (var order in _dataService.GetOrders())
            Orders.Add(order);

        TrafficSources.Clear();
        foreach (var source in _dataService.GetTrafficSources())
            TrafficSources.Add(source);

        SalesDataPoints.Clear();
        foreach (var dataPoint in _dataService.GetSalesData())
            SalesDataPoints.Add(dataPoint);
    }
}
