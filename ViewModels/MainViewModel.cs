using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiHighFidelityDashboard.Services.Interfaces;
using MauiHighFidelityDashboard.Models;
using MauiHighFidelityDashboard.Views;

namespace MauiHighFidelityDashboard.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private const int PageSize = 5;

    private readonly IDashboardDataService _dataService;
    private readonly IPrintService _printService;
    private readonly List<OrderModel> _allOrders = [];

    public ObservableCollection<DashboardCard> DashboardCards { get; } = [];
    public ObservableCollection<RevenueCardItem> RevenueCards { get; } = [];
    public ObservableCollection<ActivityModel> Activities { get; } = [];
    public ObservableCollection<OrderModel> Orders { get; } = [];
    public ObservableCollection<PageItem> PageNumbers { get; } = [];
    public ObservableCollection<TrafficModel> TrafficSources { get; } = [];
    public ObservableCollection<SalesData> SalesDataPoints { get; } = [];

    public DashboardCard? WalletCard => DashboardCards.ElementAtOrDefault(0);
    public DashboardCard? ReferralCard => DashboardCards.ElementAtOrDefault(1);
    public DashboardCard? EstimateCard => DashboardCards.ElementAtOrDefault(2);
    public DashboardCard? EarningCard => DashboardCards.ElementAtOrDefault(3);

    public RevenueCardItem? RevenueStatusCard => RevenueCards.ElementAtOrDefault(0);
    public RevenueCardItem? PageViewCard => RevenueCards.ElementAtOrDefault(1);
    public RevenueCardItem? BounceRateCard => RevenueCards.ElementAtOrDefault(2);
    public RevenueCardItem? RevenueStatusAltCard => RevenueCards.ElementAtOrDefault(3);

    public string CurrentMonthEarnings => "$3468.96";
    public string CurrentMonthSales => "82";
    public string DashboardDisplayTitle => "Dashboard";
    public string DashboardSubtitle => "Overview of Latest Month";
    public bool IsDataLoaded { get; private set; }

#pragma warning disable MVVMTK0045 // ObservableProperty fields not AOT-compatible on WinRT
    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _ordersFooterText = string.Empty;
#pragma warning restore MVVMTK0045

    private int _currentPage = 1;
    private OrderModel? _selectedOrder;

    public MainViewModel(IDashboardDataService dataService, IPrintService printService)
    {
        _dataService = dataService;
        _printService = printService;
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
            LoadRevenueCardsAsync(),
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

    private static readonly IReadOnlyList<SummaryStat> LastMonthStats =
    [
        new("Earnings", "$2,980.44"),
        new("Sales", "74"),
        new("New Orders", "61"),
        new("Refunds", "3"),
        new("Top Seller", "Wireless Headset"),
        new("Best Region", "Italy"),
        new("Growth vs May", "+16.4%", Highlight: true),
    ];

    [RelayCommand]
    private async Task LastMonthSummaryAsync()
    {
        var page = Shell.Current.CurrentPage;
        if (page is null) return;
        await page.ShowPopupAsync(new LastMonthSummaryPopup(LastMonthStats));
    }

    // ---------- Order Status: search / pagination / toolbar ----------

    partial void OnSearchTextChanged(string value)
    {
        _currentPage = 1;
        RefreshOrdersView();
    }

    private IEnumerable<OrderModel> FilteredOrders =>
        string.IsNullOrWhiteSpace(SearchText)
            ? _allOrders
            : _allOrders.Where(o =>
                o.Customer.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                o.Country.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                o.Status.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                o.Invoice.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase));

    private void RefreshOrdersView()
    {
        var filtered = FilteredOrders.ToList();
        int total = filtered.Count;
        int totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        _currentPage = Math.Clamp(_currentPage, 1, totalPages);

        Orders.Clear();
        foreach (var order in filtered.Skip((_currentPage - 1) * PageSize).Take(PageSize))
            Orders.Add(order);

        PageNumbers.Clear();
        for (int page = 1; page <= totalPages; page++)
            PageNumbers.Add(new PageItem(page, page == _currentPage));

        int start = total == 0 ? 0 : (_currentPage - 1) * PageSize + 1;
        int end = Math.Min(_currentPage * PageSize, total);
        OrdersFooterText = $"Showing {start} to {end} of {total} entries";
    }

    [RelayCommand]
    private void GoToPage(PageItem? page)
    {
        if (page is null) return;
        _currentPage = page.Number;
        RefreshOrdersView();
    }

    [RelayCommand]
    private void NextPage()
    {
        _currentPage++;
        RefreshOrdersView();
    }

    [RelayCommand]
    private void PreviousPage()
    {
        _currentPage--;
        RefreshOrdersView();
    }

    [RelayCommand]
    private void SelectOrder(OrderModel? order)
    {
        if (order is null) return;
        bool select = !order.IsSelected;
        foreach (var o in _allOrders) o.IsSelected = false;
        order.IsSelected = select;
        _selectedOrder = select ? order : null;
    }

    [RelayCommand]
    private async Task AddOrderAsync()
    {
        var page = Shell.Current.CurrentPage;
        if (page is null) return;
        var result = await page.ShowPopupAsync(new AddOrderPopup());
        if (result is (string customer, string country, decimal price, string status))
        {
            AppendOrder(customer, country, price, status);
        }
    }

    // Appends the new order at the end of the list and jumps to the last page,
    // so the values the user just typed are visible immediately.
    private void AppendOrder(string customer, string country, decimal price, string status)
    {
        int nextInvoice = _allOrders.Count == 0 ? 12412 : _allOrders.Max(o => o.Invoice) + 1;
        _allOrders.Add(new OrderModel
        {
            Invoice = nextInvoice,
            Customer = customer,
            Country = country,
            Price = price,
            Status = status
        });

        SearchText = string.Empty;
        _currentPage = int.MaxValue; // clamped to the last page by RefreshOrdersView
        RefreshOrdersView();
    }

    [RelayCommand]
    private async Task DeleteOrderAsync()
    {
        if (_selectedOrder is null)
        {
            await Shell.Current.DisplayAlertAsync("Delete Order", "Tap a row to select the order you want to delete.", "OK");
            return;
        }

        var page = Shell.Current.CurrentPage;
        if (page is null) return;
        var result = await page.ShowPopupAsync(new ConfirmDeletePopup(_selectedOrder));
        if (result is not true) return;

        _allOrders.Remove(_selectedOrder);
        _selectedOrder = null;
        RefreshOrdersView();
    }

    [RelayCommand]
    private async Task ShowOrderInfoAsync()
    {
        var filtered = FilteredOrders.ToList();
        int open = filtered.Count(o => o.Status == "Open");
        int process = filtered.Count(o => o.Status == "Process");
        int onHold = filtered.Count(o => o.Status == "On Hold");
        decimal totalValue = filtered.Sum(o => o.Price);

        await Shell.Current.DisplayAlertAsync(
            "Order Summary",
            $"Total orders:  {filtered.Count}\n" +
            $"Open:          {open}\n" +
            $"Process:       {process}\n" +
            $"On Hold:       {onHold}\n" +
            $"Total value:   ${totalValue:N2}",
            "Close");
    }

    [RelayCommand]
    private async Task PrintOrdersAsync()
    {
        var filtered = FilteredOrders.ToList();
        await _printService.PrintOrdersAsync(filtered, "OrderStatus_LatestMonth");
    }

    // ---------- Data loading ----------

    private async Task LoadDashboardCardsAsync()
    {
        var result = await _dataService.GetDashboardCardsAsync();
        if (result.IsFailure) return;

        DashboardCards.Clear();
        foreach (var card in result.Data!)
            DashboardCards.Add(card);

        OnPropertyChanged(nameof(WalletCard));
        OnPropertyChanged(nameof(ReferralCard));
        OnPropertyChanged(nameof(EstimateCard));
        OnPropertyChanged(nameof(EarningCard));
    }

    private Task LoadRevenueCardsAsync()
    {
        RevenueCards.Clear();

        RevenueCards.Add(new RevenueCardItem
        {
            Title = "Revinue Status",
            Value = "$432",
            Subtitle = "Jan 01 - Jan 10",
            ChartType = "Bar",
            BackgroundHex = "#E1F0FF",
            AccentHex = "#2196F3",
        });
        RevenueCards.Add(new RevenueCardItem
        {
            Title = "Page View",
            Value = "$432",
            ChartType = "Area",
            BackgroundHex = "#FFF8E1",
            AccentHex = "#FFB822",
        });
        RevenueCards.Add(new RevenueCardItem
        {
            Title = "Bounce Rate",
            Value = "$432",
            ChartType = "Line",
            BackgroundHex = "#FDEADF",
            AccentHex = "#FF7B43",
        });
        RevenueCards.Add(new RevenueCardItem
        {
            Title = "Revinue Status",
            Value = "$432",
            Subtitle = "Jan 01 - Jan 10",
            ChartType = "Bar",
            BackgroundHex = "#F3E8FD",
            AccentHex = "#A461D8",
        });

        OnPropertyChanged(nameof(RevenueStatusCard));
        OnPropertyChanged(nameof(PageViewCard));
        OnPropertyChanged(nameof(BounceRateCard));
        OnPropertyChanged(nameof(RevenueStatusAltCard));

        return Task.CompletedTask;
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

        _allOrders.Clear();
        _allOrders.AddRange(result.Data!);
        _currentPage = 1;
        RefreshOrdersView();
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
