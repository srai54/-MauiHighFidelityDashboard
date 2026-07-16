using MauiHighFidelityDashboard.Domain.Common;
using MauiHighFidelityDashboard.Domain.Interfaces;
using MauiHighFidelityDashboard.Domain.Models;

namespace MauiHighFidelityDashboard.Infrastructure.Data;

public class StaticDashboardDataService : IDashboardDataService
{
    public Task<Result<IReadOnlyList<DashboardCard>>> GetDashboardCardsAsync() =>
        Task.FromResult(Result<IReadOnlyList<DashboardCard>>.Success(
        [
            new() { Title = "Wallet Ballance", Amount = 4567.53m, AmountDisplay = "$4,567.53", Icon = "♛", ThemeColorHex = "#F7284A" },
            new() { Title = "Referral Earning", Amount = 1689.53m, AmountDisplay = "$1689.53", Icon = "♥", ThemeColorHex = "#7C60FA" },
            new() { Title = "Estimate Sales", Amount = 2851.53m, AmountDisplay = "$2851.53", Icon = "◎", ThemeColorHex = "#2BC155" },
            new() { Title = "Earning", Amount = 52567.53m, AmountDisplay = "$52,567.53", Icon = "$", ThemeColorHex = "#FF5E9D" }
        ]));

    public Task<Result<IReadOnlyList<ActivityModel>>> GetActivitiesAsync() =>
        Task.FromResult(Result<IReadOnlyList<ActivityModel>>.Success(
        [
            new() { Title = "Task Updated", Actor = "Nikolai", Action = "Updated a Task", Time = "42 Mins Ago", Icon = "☰", IconColorHex = "#6259CE" },
            new() { Title = "Deal Added", Actor = "Panshi", Action = "Updated a Task", Time = "1 Day Ago", Icon = "＋", IconColorHex = "#EC407A" },
            new() { Title = "Published Article", Actor = "Rasel", Action = "Published a Article", Time = "42 Mins Ago", Icon = "▤", IconColorHex = "#29B6F6" },
            new() { Title = "Dock Updated", Actor = "Reshmi", Action = "Updated a Dock", Time = "1 Day Ago", Icon = "✎", IconColorHex = "#FFB822" },
            new() { Title = "Replyed Comment", Actor = "Jenathon", Action = "Added a Comment", Time = "1 Day Ago", Icon = "↩", IconColorHex = "#2BC155" }
        ]));

    public Task<Result<IReadOnlyList<OrderModel>>> GetOrdersAsync() =>
        Task.FromResult(Result<IReadOnlyList<OrderModel>>.Success(
        [
            // Page 1 mirrors the reference screenshot
            new() { Invoice = 12386, Customer = "Charly Dues", Country = "Brazil", Price = 299m, Status = "Process" },
            new() { Invoice = 12386, Customer = "Marko", Country = "Italy", Price = 2642m, Status = "Open" },
            new() { Invoice = 12386, Customer = "Deniyel Onak", Country = "Russia", Price = 981m, Status = "On Hold" },
            new() { Invoice = 12386, Customer = "Belgiri Bastana", Country = "Korea", Price = 369m, Status = "Process" },
            new() { Invoice = 12386, Customer = "Sarti Gnuska", Country = "Japan", Price = 1240m, Status = "Open" },
            new() { Invoice = 12387, Customer = "Amara Okafor", Country = "Nigeria", Price = 754m, Status = "Open" },
            new() { Invoice = 12388, Customer = "Liam Carter", Country = "USA", Price = 1899m, Status = "Process" },
            new() { Invoice = 12389, Customer = "Sofia Reyes", Country = "Mexico", Price = 432m, Status = "On Hold" },
            new() { Invoice = 12390, Customer = "Hans Meyer", Country = "Germany", Price = 3110m, Status = "Open" },
            new() { Invoice = 12391, Customer = "Yuki Tanaka", Country = "Japan", Price = 587m, Status = "Process" },
            new() { Invoice = 12392, Customer = "Priya Sharma", Country = "India", Price = 1456m, Status = "Open" },
            new() { Invoice = 12393, Customer = "Lucas Silva", Country = "Brazil", Price = 823m, Status = "On Hold" },
            new() { Invoice = 12394, Customer = "Emma Wilson", Country = "UK", Price = 2075m, Status = "Open" },
            new() { Invoice = 12395, Customer = "Omar Haddad", Country = "Egypt", Price = 640m, Status = "Process" },
            new() { Invoice = 12396, Customer = "Chen Wei", Country = "China", Price = 1785m, Status = "Open" },
            new() { Invoice = 12397, Customer = "Anna Kowalski", Country = "Poland", Price = 912m, Status = "On Hold" },
            new() { Invoice = 12398, Customer = "Pierre Dubois", Country = "France", Price = 1330m, Status = "Process" },
            new() { Invoice = 12399, Customer = "Elena Petrova", Country = "Russia", Price = 468m, Status = "Open" },
            new() { Invoice = 12400, Customer = "Marco Rossi", Country = "Italy", Price = 2210m, Status = "Open" },
            new() { Invoice = 12401, Customer = "Kim Min-jun", Country = "Korea", Price = 795m, Status = "Process" },
            new() { Invoice = 12402, Customer = "Sara Lindqvist", Country = "Sweden", Price = 1120m, Status = "On Hold" },
            new() { Invoice = 12403, Customer = "David Cohen", Country = "Israel", Price = 356m, Status = "Open" },
            new() { Invoice = 12404, Customer = "Fatima Zahra", Country = "Morocco", Price = 1670m, Status = "Process" },
            new() { Invoice = 12405, Customer = "Jack Thompson", Country = "Australia", Price = 940m, Status = "Open" },
            new() { Invoice = 12406, Customer = "Isabella Cruz", Country = "Spain", Price = 2380m, Status = "On Hold" },
            new() { Invoice = 12407, Customer = "Noah Brown", Country = "Canada", Price = 515m, Status = "Open" },
            new() { Invoice = 12408, Customer = "Aisha Bello", Country = "Ghana", Price = 1245m, Status = "Process" },
            new() { Invoice = 12409, Customer = "Mateus Costa", Country = "Portugal", Price = 860m, Status = "Open" },
            new() { Invoice = 12410, Customer = "Olga Ivanova", Country = "Ukraine", Price = 1990m, Status = "On Hold" },
            new() { Invoice = 12411, Customer = "Tom Becker", Country = "Austria", Price = 730m, Status = "Process" }
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
