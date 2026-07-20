using MauiHighFidelityDashboard.Models;
using MauiHighFidelityDashboard.Services.Interfaces;

namespace MauiHighFidelityDashboard.Services;

public class StaticDashboardDataService : IDashboardDataService
{
    public Task<Result<IReadOnlyList<DashboardCard>>> GetDashboardCardsAsync() =>
        Task.FromResult(Result<IReadOnlyList<DashboardCard>>.Success(
        [
            // Icons are Font Awesome solid glyphs (family "FontAwesome")
            new() { Title = "Wallet Ballance", Amount = 4567.53m, AmountDisplay = "$4,567.53", Icon = "", ThemeColorHex = "#F7284A" },  // crown
            new() { Title = "Referral Earning", Amount = 1689.53m, AmountDisplay = "$1689.53", Icon = "", ThemeColorHex = "#7C60FA" },  // heart
            new() { Title = "Estimate Sales", Amount = 2851.53m, AmountDisplay = "$2851.53", Icon = "", ThemeColorHex = "#2BC155" },    // bullseye
            new() { Title = "Earning", Amount = 52567.53m, AmountDisplay = "$52,567.53", Icon = "", ThemeColorHex = "#FF5E9D" }         // dollar-sign
        ]));

    public Task<Result<IReadOnlyList<RevenueCardItem>>> GetRevenueCardsAsync() =>
        Task.FromResult(Result<IReadOnlyList<RevenueCardItem>>.Success(
        [
            // The reference template spells this "Revinue"; corrected to "Revenue" per client request.
            new() { Title = "Revenue Status", Value = "$432", Subtitle = "Jan 01 - Jan 10", ChartType = "Bar", BackgroundHex = "#E1F0FF", AccentHex = "#2196F3" },
            new() { Title = "Page View", Value = "$432", ChartType = "Area", BackgroundHex = "#FFF8E1", AccentHex = "#FFB822" },
            new() { Title = "Bounce Rate", Value = "$432", ChartType = "Line", BackgroundHex = "#FBE4D7", AccentHex = "#ED5520" },
            new() { Title = "Revenue Status", Value = "$432", Subtitle = "Jan 01 - Jan 10", ChartType = "Bar", BackgroundHex = "#F0DEFE", AccentHex = "#8214E8" }
        ]));

    public Task<Result<IReadOnlyList<ActivityModel>>> GetActivitiesAsync() =>
        Task.FromResult(Result<IReadOnlyList<ActivityModel>>.Success(
        [
            new() { Title = "Task Updated", Actor = "Nikolai", Action = "Updated a Task", Time = "42 Mins Ago", Icon = "", IconColorHex = "#6259CE" },        // list
            new() { Title = "Deal Added", Actor = "Panshi", Action = "Updated a Task", Time = "1 Day Ago", Icon = "", IconColorHex = "#EC407A" },            // plus
            new() { Title = "Published Article", Actor = "Rasel", Action = "Published a Article", Time = "42 Mins Ago", Icon = "", IconColorHex = "#29B6F6" }, // file-lines
            new() { Title = "Dock Updated", Actor = "Reshmi", Action = "Updated a Dock", Time = "1 Day Ago", Icon = "", IconColorHex = "#FFB822" },          // pen
            new() { Title = "Replyed Comment", Actor = "Jenathon", Action = "Added a Comment", Time = "1 Day Ago", Icon = "", IconColorHex = "#2BC155" }     // reply
        ]));

    // Held as instance state so Add/Delete behave like a real backend while offline.
    private readonly List<OrderModel> _orders = new (int Invoice, string Customer, string Country, decimal Price, string Status)[]
    {
        // Page 1 mirrors the reference screenshot
        (12386, "Charly Dues", "Brazil", 299m, "Process"),
        (12386, "Marko", "Italy", 2642m, "Open"),
        (12386, "Deniyel Onak", "Russia", 981m, "On Hold"),
        (12386, "Belgiri Bastana", "Korea", 369m, "Process"),
        (12386, "Sarti Gnuska", "Japan", 1240m, "Open"),
        (12387, "Amara Okafor", "Nigeria", 754m, "Open"),
        (12388, "Liam Carter", "USA", 1899m, "Process"),
        (12389, "Sofia Reyes", "Mexico", 432m, "On Hold"),
        (12390, "Hans Meyer", "Germany", 3110m, "Open"),
        (12391, "Yuki Tanaka", "Japan", 587m, "Process"),
        (12392, "Priya Sharma", "India", 1456m, "Open"),
        (12393, "Lucas Silva", "Brazil", 823m, "On Hold"),
        (12394, "Emma Wilson", "UK", 2075m, "Open"),
        (12395, "Omar Haddad", "Egypt", 640m, "Process"),
        (12396, "Chen Wei", "China", 1785m, "Open"),
        (12397, "Anna Kowalski", "Poland", 912m, "On Hold"),
        (12398, "Pierre Dubois", "France", 1330m, "Process"),
        (12399, "Elena Petrova", "Russia", 468m, "Open"),
        (12400, "Marco Rossi", "Italy", 2210m, "Open"),
        (12401, "Kim Min-jun", "Korea", 795m, "Process"),
        (12402, "Sara Lindqvist", "Sweden", 1120m, "On Hold"),
        (12403, "David Cohen", "Israel", 356m, "Open"),
        (12404, "Fatima Zahra", "Morocco", 1670m, "Process"),
        (12405, "Jack Thompson", "Australia", 940m, "Open"),
        (12406, "Isabella Cruz", "Spain", 2380m, "On Hold"),
        (12407, "Noah Brown", "Canada", 515m, "Open"),
        (12408, "Aisha Bello", "Ghana", 1245m, "Process"),
        (12409, "Mateus Costa", "Portugal", 860m, "Open"),
        (12410, "Olga Ivanova", "Ukraine", 1990m, "On Hold"),
        (12411, "Tom Becker", "Austria", 730m, "Process")
    }
    .Select(static (o, i) => new OrderModel
    {
        Id = i + 1,
        Invoice = o.Invoice,
        Customer = o.Customer,
        Country = o.Country,
        Price = o.Price,
        Status = o.Status
    })
    .ToList();

    public Task<Result<IReadOnlyList<OrderModel>>> GetOrdersAsync() =>
        Task.FromResult(Result<IReadOnlyList<OrderModel>>.Success(_orders.ToList()));

    public Task<Result<OrderModel>> AddOrderAsync(string customer, string country, decimal price, string status)
    {
        var order = new OrderModel
        {
            Id = _orders.Count == 0 ? 1 : _orders.Max(o => o.Id) + 1,
            Invoice = _orders.Count == 0 ? 12412 : _orders.Max(o => o.Invoice) + 1,
            Customer = customer,
            Country = country,
            Price = price,
            Status = status
        };
        _orders.Add(order);
        return Task.FromResult(Result<OrderModel>.Success(order));
    }

    public Task<Result<int>> DeleteOrdersAsync(IReadOnlyList<int> orderIds)
    {
        int removed = _orders.RemoveAll(o => orderIds.Contains(o.Id));
        return Task.FromResult(Result<int>.Success(removed));
    }

    public Task<Result<IReadOnlyList<TrafficModel>>> GetTrafficSourcesAsync() =>
        Task.FromResult(Result<IReadOnlyList<TrafficModel>>.Success(
        [
            new() { Source = "Facebook", Percentage = 34, SegmentColorHex = "#2196F3" },
            new() { Source = "Youtube", Percentage = 55, SegmentColorHex = "#FF5722" },
            new() { Source = "Direct Search", Percentage = 11, SegmentColorHex = "#FFC107" }
        ]));
}
