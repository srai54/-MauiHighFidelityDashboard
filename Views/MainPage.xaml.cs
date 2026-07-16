using MauiHighFidelityDashboard.ViewModels;

namespace MauiHighFidelityDashboard.Views;

public partial class MainPage : ContentPage
{
    // Below this logical width (phones, small windows, high zoom levels)
    // the dashboard stacks into a single column.
    private const double NarrowBreakpoint = 980;

    private readonly MainViewModel _viewModel;
    private bool? _isNarrow;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        SizeChanged += (_, _) => ApplyResponsiveLayout();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();

        // Keep the dashboard anchored at the top on launch; without this WinUI can
        // scroll the first focusable input (the order search box) into view.
        Dispatcher.Dispatch(() =>
        {
            try { _ = MainScroll.ScrollToAsync(0, 0, false); }
            catch { /* scrolling to origin is best-effort */ }
        });

        // TEMP: automated UI verification hooks
        if (Environment.GetEnvironmentVariable("DASH_TEST_DETAIL") == "1")
            await Shell.Current.GoToAsync("detail?title=Widgets");
        if (Environment.GetEnvironmentVariable("DASH_TEST_PRINT") == "1")
            await _viewModel.PrintOrdersCommand.ExecuteAsync(null);
    }

    private void OnMenuClicked(object? sender, EventArgs e)
        => Sidebar.IsVisible = !Sidebar.IsVisible;

    private void ApplyResponsiveLayout()
    {
        if (Width <= 0) return;
        bool narrow = Width < NarrowBreakpoint;
        if (narrow == _isNarrow) return;
        _isNarrow = narrow;

        ContentStack.Padding = narrow ? new Thickness(12, 56, 12, 12) : new Thickness(18);
        MenuButton.IsVisible = narrow;

        if (narrow)
        {
            // Sidebar becomes a hidden overlay toggled by the hamburger button.
            RootGrid.ColumnDefinitions[0].Width = new GridLength(0);
            Grid.SetColumn(Sidebar, 1);
            Sidebar.WidthRequest = 200;
            Sidebar.HorizontalOptions = LayoutOptions.Start;
            Sidebar.ZIndex = 10;
            Sidebar.IsVisible = false;

            // Traffic card drops below the main card.
            SetGridShape(TopRowGrid, columns: [GridLength.Star], rows: [GridLength.Auto, GridLength.Auto]);
            Grid.SetColumn(TrafficChart, 0);
            Grid.SetRow(TrafficChart, 1);

            // Header stacks above the chart.
            SetGridShape(HeaderChartGrid, columns: [GridLength.Star], rows: [GridLength.Auto, GridLength.Auto]);
            Grid.SetColumn(ChartView, 0);
            Grid.SetRow(ChartView, 1);

            // Stat strip becomes a 2x2 grid without dividers.
            SetGridShape(SummaryStrip,
                columns: [GridLength.Star, GridLength.Star],
                rows: [GridLength.Auto, GridLength.Auto]);
            SummaryDiv1.IsVisible = SummaryDiv2.IsVisible = SummaryDiv3.IsVisible = false;
            PlaceInGrid(SummaryWallet, 0, 0);
            PlaceInGrid(SummaryReferral, 0, 1);
            PlaceInGrid(SummaryEstimate, 1, 0);
            PlaceInGrid(SummaryEarning, 1, 1);
            SummaryEstimate.Margin = new Thickness(0);

            // Activities and Orders stack.
            SetGridShape(BottomRowGrid, columns: [GridLength.Star], rows: [GridLength.Auto, GridLength.Auto]);
            Grid.SetColumn(OrderTable, 0);
            Grid.SetRow(OrderTable, 1);
        }
        else
        {
            RootGrid.ColumnDefinitions[0].Width = new GridLength(200);
            Grid.SetColumn(Sidebar, 0);
            Sidebar.WidthRequest = -1;
            Sidebar.HorizontalOptions = LayoutOptions.Fill;
            Sidebar.ZIndex = 0;
            Sidebar.IsVisible = true;

            SetGridShape(TopRowGrid,
                columns: [new GridLength(2.55, GridUnitType.Star), GridLength.Star],
                rows: [GridLength.Auto]);
            Grid.SetRow(TrafficChart, 0);
            Grid.SetColumn(TrafficChart, 1);

            SetGridShape(HeaderChartGrid,
                columns: [new GridLength(205), GridLength.Star],
                rows: [GridLength.Auto]);
            Grid.SetRow(ChartView, 0);
            Grid.SetColumn(ChartView, 1);

            SetGridShape(SummaryStrip,
                columns:
                [
                    GridLength.Star, new GridLength(1), GridLength.Star, new GridLength(1),
                    GridLength.Star, new GridLength(1), GridLength.Star
                ],
                rows: [GridLength.Auto]);
            SummaryDiv1.IsVisible = SummaryDiv2.IsVisible = SummaryDiv3.IsVisible = true;
            PlaceInGrid(SummaryWallet, 0, 0);
            PlaceInGrid(SummaryReferral, 0, 2);
            PlaceInGrid(SummaryEstimate, 0, 4);
            PlaceInGrid(SummaryEarning, 0, 6);
            SummaryEstimate.Margin = new Thickness(18, 0, 0, 0);

            SetGridShape(BottomRowGrid,
                columns: [new GridLength(2, GridUnitType.Star), new GridLength(3, GridUnitType.Star)],
                rows: [GridLength.Auto]);
            Grid.SetRow(OrderTable, 0);
            Grid.SetColumn(OrderTable, 1);
        }
    }

    private static void SetGridShape(Grid grid, GridLength[] columns, GridLength[] rows)
    {
        grid.ColumnDefinitions.Clear();
        foreach (var column in columns)
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = column });

        grid.RowDefinitions.Clear();
        foreach (var row in rows)
            grid.RowDefinitions.Add(new RowDefinition { Height = row });
    }

    private static void PlaceInGrid(IView view, int row, int column)
    {
        var element = (BindableObject)view;
        Grid.SetRow(element, row);
        Grid.SetColumn(element, column);
    }
}
