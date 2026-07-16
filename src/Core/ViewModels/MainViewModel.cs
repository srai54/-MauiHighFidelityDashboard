using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiHighFidelityDashboard.Domain.Interfaces;
using MauiHighFidelityDashboard.Domain.Models;

namespace MauiHighFidelityDashboard.Core.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private const int PageSize = 5;

    private readonly IDashboardDataService _dataService;
    private readonly List<OrderModel> _allOrders = [];

    public ObservableCollection<DashboardCard> DashboardCards { get; } = [];
    public ObservableCollection<ActivityModel> Activities { get; } = [];
    public ObservableCollection<OrderModel> Orders { get; } = [];
    public ObservableCollection<PageItem> PageNumbers { get; } = [];
    public ObservableCollection<TrafficModel> TrafficSources { get; } = [];
    public ObservableCollection<SalesData> SalesDataPoints { get; } = [];

    public DashboardCard? WalletCard => DashboardCards.ElementAtOrDefault(0);
    public DashboardCard? ReferralCard => DashboardCards.ElementAtOrDefault(1);
    public DashboardCard? EstimateCard => DashboardCards.ElementAtOrDefault(2);
    public DashboardCard? EarningCard => DashboardCards.ElementAtOrDefault(3);

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
        await Shell.Current.DisplayAlertAsync(
            "Last Month Summary",
            "Earnings:        $2,980.44\n" +
            "Sales:           74\n" +
            "New Orders:      61\n" +
            "Refunds:         3\n" +
            "Top Seller:      Wireless Headset\n" +
            "Best Region:     Italy\n" +
            "Growth vs May:   +16.4%",
            "Close");
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

    private static readonly (string Customer, string Country, string Status)[] NewOrderPool =
    [
        ("Nina Larsen", "Norway", "Open"),
        ("Diego Fernandez", "Argentina", "Process"),
        ("Zara Ahmed", "Pakistan", "On Hold"),
        ("Felix Wagner", "Switzerland", "Open"),
        ("Mei Lin", "Singapore", "Process"),
    ];

    private int _newOrderIndex;

    [RelayCommand]
    private async Task AddOrderAsync()
    {
        var choice = await Shell.Current.DisplayActionSheetAsync(
            "Add Order", "Cancel", null, "Enter manually", "Quick add (sample data)");

        if (choice == "Enter manually")
        {
            var customer = await Shell.Current.DisplayPromptAsync(
                "New Order", "Customer name:", "Next", "Cancel", placeholder: "e.g. John Smith");
            if (string.IsNullOrWhiteSpace(customer)) return;

            var country = await Shell.Current.DisplayPromptAsync(
                "New Order", "Country:", "Next", "Cancel", placeholder: "e.g. India");
            if (string.IsNullOrWhiteSpace(country)) return;

            var priceText = await Shell.Current.DisplayPromptAsync(
                "New Order", "Price (USD):", "Next", "Cancel", placeholder: "e.g. 499", keyboard: Keyboard.Numeric);
            if (priceText is null) return;
            if (!decimal.TryParse(priceText, out var price) || price <= 0)
            {
                await Shell.Current.DisplayAlertAsync("Invalid price", "Please enter a positive number.", "OK");
                return;
            }

            var status = await Shell.Current.DisplayActionSheetAsync(
                "Order status", "Cancel", null, "Open", "Process", "On Hold");
            if (status is null or "Cancel") return;

            InsertOrder(customer.Trim(), country.Trim(), price, status);
        }
        else if (choice == "Quick add (sample data)")
        {
            var (customer, country, status) = NewOrderPool[_newOrderIndex % NewOrderPool.Length];
            InsertOrder(customer, country, 350m + (_newOrderIndex + 1) * 137m, status);
        }
    }

    private void InsertOrder(string customer, string country, decimal price, string status)
    {
        _allOrders.Insert(0, new OrderModel
        {
            Invoice = 12412 + _newOrderIndex,
            Customer = customer,
            Country = country,
            Price = price,
            Status = status
        });
        _newOrderIndex++;

        _currentPage = 1;
        SearchText = string.Empty;
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

        bool confirmed = await Shell.Current.DisplayAlertAsync(
            "Delete Order",
            $"Delete invoice {_selectedOrder.Invoice} for {_selectedOrder.Customer}?",
            "Delete", "Cancel");
        if (!confirmed) return;

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
        await Shell.Current.DisplayAlertAsync(
            "Print",
            $"Sending {filtered.Count} orders (${filtered.Sum(o => o.Price):N2}) to the printer…\n\nDocument: OrderStatus_LatestMonth.pdf",
            "Done");
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
