using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MauiHighFidelityDashboard.Models;

namespace MauiHighFidelityDashboard.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public ObservableCollection<DashboardCard> DashboardCards { get; } = new();
    public ObservableCollection<ActivityModel> Activities { get; } = new();
    public ObservableCollection<OrderModel> Orders { get; } = new();
    public ObservableCollection<TrafficModel> TrafficSources { get; } = new();
    public ObservableCollection<SalesData> SalesDataPoints { get; } = new();

    public string CurrentMonthEarnings => "$3,468.96";
    public string CurrentMonthSales => "82";
    public string DashboardTitle => "Dashboard";
    public string DashboardSubtitle => "Overview of Latest Month";

    public MainViewModel()
    {
        LoadDashboardCards();
        LoadActivities();
        LoadOrders();
        LoadTrafficSources();
        LoadSalesData();
    }

    private void LoadDashboardCards()
    {
        DashboardCards.Add(new DashboardCard
        {
            Title = "Wallet Balance",
            Amount = 4567.53m,
            Icon = "\u2613",
            ThemeColor = Color.FromArgb("#F44336")
        });
        DashboardCards.Add(new DashboardCard
        {
            Title = "Referral Earning",
            Amount = 1689.53m,
            Icon = "\u2191",
            ThemeColor = Color.FromArgb("#2196F3")
        });
        DashboardCards.Add(new DashboardCard
        {
            Title = "Estimate Sales",
            Amount = 2851.53m,
            Icon = "\u2605",
            ThemeColor = Color.FromArgb("#4CAF50")
        });
        DashboardCards.Add(new DashboardCard
        {
            Title = "Earning",
            Amount = 52567.53m,
            Icon = "\u263A",
            ThemeColor = Color.FromArgb("#EC407A")
        });
    }

    private void LoadActivities()
    {
        Activities.Add(new ActivityModel
        {
            Title = "Task Updated",
            Description = "Nikolai Updated a Task",
            Time = "42 Mins Ago",
            DotColor = Color.FromArgb("#9C27B0")
        });
        Activities.Add(new ActivityModel
        {
            Title = "Deal Added",
            Description = "Panshi Updated a Task",
            Time = "1 Day Ago",
            DotColor = Color.FromArgb("#EC407A")
        });
        Activities.Add(new ActivityModel
        {
            Title = "Published Article",
            Description = "Rasel Published an Article",
            Time = "42 Mins Ago",
            DotColor = Color.FromArgb("#4FC3F7")
        });
        Activities.Add(new ActivityModel
        {
            Title = "Dock Updated",
            Description = "",
            Time = "1 Day Ago",
            DotColor = Color.FromArgb("#FFC107")
        });
    }

    private void LoadOrders()
    {
        Orders.Add(new OrderModel { Invoice = 12386, Customer = "Charly Dues", Country = "Brazil", Price = 299m, Status = "Process" });
        Orders.Add(new OrderModel { Invoice = 12386, Customer = "Marko", Country = "Italy", Price = 2642m, Status = "Open" });
        Orders.Add(new OrderModel { Invoice = 12386, Customer = "Deniyel Onak", Country = "Russia", Price = 981m, Status = "On Hold" });
        Orders.Add(new OrderModel { Invoice = 12386, Customer = "Belgiri Bastana", Country = "Korea", Price = 369m, Status = "Process" });
        Orders.Add(new OrderModel { Invoice = 12386, Customer = "Vaska Simon", Country = "Japan", Price = 1240m, Status = "Open" });
    }

    private void LoadTrafficSources()
    {
        TrafficSources.Add(new TrafficModel { Source = "Facebook", Percentage = 34, SegmentColor = Color.FromArgb("#2196F3") });
        TrafficSources.Add(new TrafficModel { Source = "Youtube", Percentage = 55, SegmentColor = Color.FromArgb("#FF5722") });
        TrafficSources.Add(new TrafficModel { Source = "Direct Search", Percentage = 11, SegmentColor = Color.FromArgb("#FFC107") });
    }

    private void LoadSalesData()
    {
        SalesDataPoints.Add(new SalesData { Day = 1, Online = 2, Store = 3 });
        SalesDataPoints.Add(new SalesData { Day = 2, Online = 12, Store = 11 });
        SalesDataPoints.Add(new SalesData { Day = 3, Online = 3, Store = 5 });
        SalesDataPoints.Add(new SalesData { Day = 4, Online = 6, Store = 14 });
        SalesDataPoints.Add(new SalesData { Day = 5, Online = 10, Store = 12 });
        SalesDataPoints.Add(new SalesData { Day = 6, Online = 24, Store = 16 });
    }
}
