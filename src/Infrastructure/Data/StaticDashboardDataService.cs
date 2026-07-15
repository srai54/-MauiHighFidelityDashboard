using MauiHighFidelityDashboard.Domain.Common;
using MauiHighFidelityDashboard.Domain.Interfaces;
using MauiHighFidelityDashboard.Domain.Models;

namespace MauiHighFidelityDashboard.Infrastructure.Data;

public class StaticDashboardDataService : IDashboardDataService
{
    public Task<Result<IReadOnlyList<DashboardCard>>> GetDashboardCardsAsync() =>
        Task.FromResult(Result<IReadOnlyList<DashboardCard>>.Success(
        [
            new() { Title = "Wallet Balance", Amount = 4567.53m, Icon = "\u2613", ThemeColorHex = "#F44336" },
            new() { Title = "Referral Earning", Amount = 1689.53m, Icon = "\u2191", ThemeColorHex = "#2196F3" },
            new() { Title = "Estimate Sales", Amount = 2851.53m, Icon = "\u2605", ThemeColorHex = "#4CAF50" },
            new() { Title = "Earning", Amount = 52567.53m, Icon = "\u263A", ThemeColorHex = "#EC407A" }
        ]));

    public Task<Result<IReadOnlyList<ActivityModel>>> GetActivitiesAsync() =>
        Task.FromResult(Result<IReadOnlyList<ActivityModel>>.Success(
        [
            new() { Title = "Task Updated", Description = "Nikolai Updated a Task", Time = "42 Mins Ago", DotColorHex = "#9C27B0" },
            new() { Title = "Deal Added", Description = "Panshi Updated a Task", Time = "1 Day Ago", DotColorHex = "#EC407A" },
            new() { Title = "Published Article", Description = "Rasel Published an Article", Time = "42 Mins Ago", DotColorHex = "#4FC3F7" },
            new() { Title = "Dock Updated", Description = "", Time = "1 Day Ago", DotColorHex = "#FFC107" }
        ]));

    public Task<Result<IReadOnlyList<OrderModel>>> GetOrdersAsync() =>
        Task.FromResult(Result<IReadOnlyList<OrderModel>>.Success(
        [
            new() { Invoice = 12386, Customer = "Charly Dues", Country = "Brazil", Price = 299m, Status = "Process" },
            new() { Invoice = 12386, Customer = "Marko", Country = "Italy", Price = 2642m, Status = "Open" },
            new() { Invoice = 12386, Customer = "Deniyel Onak", Country = "Russia", Price = 981m, Status = "On Hold" },
            new() { Invoice = 12386, Customer = "Belgiri Bastana", Country = "Korea", Price = 369m, Status = "Process" },
            new() { Invoice = 12386, Customer = "Vaska Simon", Country = "Japan", Price = 1240m, Status = "Open" }
        ]));

    public Task<Result<IReadOnlyList<TrafficModel>>> GetTrafficSourcesAsync() =>
        Task.FromResult(Result<IReadOnlyList<TrafficModel>>.Success(
        [
            new() { Source = "Facebook", Percentage = 34, SegmentColorHex = "#2196F3" },
            new() { Source = "Youtube", Percentage = 55, SegmentColorHex = "#FF5722" },
            new() { Source = "Direct Search", Percentage = 11, SegmentColorHex = "#FFC107" }
        ]));

    public Task<Result<IReadOnlyList<SalesData>>> GetSalesDataAsync() =>
        Task.FromResult(Result<IReadOnlyList<SalesData>>.Success(
        [
            new() { Day = 1, Online = 2, Store = 3 },
            new() { Day = 2, Online = 13, Store = 12 },
            new() { Day = 3, Online = 3, Store = 5 },
            new() { Day = 4, Online = 6, Store = 14 },
            new() { Day = 5, Online = 12, Store = 10 },
            new() { Day = 6, Online = 27, Store = 17 }
        ]));
}
