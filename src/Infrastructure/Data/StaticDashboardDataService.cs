using MauiHighFidelityDashboard.Domain.Interfaces;
using MauiHighFidelityDashboard.Domain.Models;

namespace MauiHighFidelityDashboard.Infrastructure.Data;

public class StaticDashboardDataService : IDashboardDataService
{
    public IReadOnlyList<DashboardCard> GetDashboardCards() =>
    [
        new() { Title = "Wallet Balance", Amount = 4567.53m, Icon = "\u2613", ThemeColor = Color.FromArgb("#F44336") },
        new() { Title = "Referral Earning", Amount = 1689.53m, Icon = "\u2191", ThemeColor = Color.FromArgb("#2196F3") },
        new() { Title = "Estimate Sales", Amount = 2851.53m, Icon = "\u2605", ThemeColor = Color.FromArgb("#4CAF50") },
        new() { Title = "Earning", Amount = 52567.53m, Icon = "\u263A", ThemeColor = Color.FromArgb("#EC407A") }
    ];

    public IReadOnlyList<ActivityModel> GetActivities() =>
    [
        new() { Title = "Task Updated", Description = "Nikolai Updated a Task", Time = "42 Mins Ago", DotColor = Color.FromArgb("#9C27B0") },
        new() { Title = "Deal Added", Description = "Panshi Updated a Task", Time = "1 Day Ago", DotColor = Color.FromArgb("#EC407A") },
        new() { Title = "Published Article", Description = "Rasel Published an Article", Time = "42 Mins Ago", DotColor = Color.FromArgb("#4FC3F7") },
        new() { Title = "Dock Updated", Description = "", Time = "1 Day Ago", DotColor = Color.FromArgb("#FFC107") }
    ];

    public IReadOnlyList<OrderModel> GetOrders() =>
    [
        new() { Invoice = 12386, Customer = "Charly Dues", Country = "Brazil", Price = 299m, Status = "Process" },
        new() { Invoice = 12386, Customer = "Marko", Country = "Italy", Price = 2642m, Status = "Open" },
        new() { Invoice = 12386, Customer = "Deniyel Onak", Country = "Russia", Price = 981m, Status = "On Hold" },
        new() { Invoice = 12386, Customer = "Belgiri Bastana", Country = "Korea", Price = 369m, Status = "Process" },
        new() { Invoice = 12386, Customer = "Vaska Simon", Country = "Japan", Price = 1240m, Status = "Open" }
    ];

    public IReadOnlyList<TrafficModel> GetTrafficSources() =>
    [
        new() { Source = "Facebook", Percentage = 34, SegmentColor = Color.FromArgb("#2196F3") },
        new() { Source = "Youtube", Percentage = 55, SegmentColor = Color.FromArgb("#FF5722") },
        new() { Source = "Direct Search", Percentage = 11, SegmentColor = Color.FromArgb("#FFC107") }
    ];

    public IReadOnlyList<SalesData> GetSalesData() =>
    [
        new() { Day = 1, Online = 2, Store = 3 },
        new() { Day = 2, Online = 12, Store = 11 },
        new() { Day = 3, Online = 3, Store = 5 },
        new() { Day = 4, Online = 6, Store = 14 },
        new() { Day = 5, Online = 10, Store = 12 },
        new() { Day = 6, Online = 24, Store = 16 }
    ];
}
