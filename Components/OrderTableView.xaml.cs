namespace MauiHighFidelityDashboard.Components;

public partial class OrderTableView : ContentView
{
    // The table needs at least this much width to show all columns;
    // below it (phones) the table pans horizontally instead of clipping.
    private const double MinTableWidth = 520;

    private bool _toolbarNarrow;

    public OrderTableView()
    {
        InitializeComponent();
        TableScroll.SizeChanged += (_, _) =>
        {
            if (TableScroll.Width > 0)
                TableContent.WidthRequest = Math.Max(TableScroll.Width, MinTableWidth);
        };
        OrdersToolbar.SizeChanged += (_, _) => ApplyToolbarLayout();
    }

    // Below this width the icon buttons and the 150px search field can't share one
    // row without overlapping (seen on phones), so the search drops to a second row.
    private void ApplyToolbarLayout()
    {
        if (OrdersToolbar.Width <= 0) return;
        bool narrow = OrdersToolbar.Width < 430;
        if (narrow == _toolbarNarrow) return;
        _toolbarNarrow = narrow;

        if (narrow)
        {
            PrintRight.IsVisible = false;   // duplicate of the toolbar print button
            Grid.SetRow(SearchEntry, 1);
            Grid.SetColumn(SearchEntry, 0);
            Grid.SetColumnSpan(SearchEntry, 7);
            SearchEntry.WidthRequest = -1;
            SearchEntry.HorizontalOptions = LayoutOptions.Fill;
        }
        else
        {
            PrintRight.IsVisible = true;
            Grid.SetRow(SearchEntry, 0);
            Grid.SetColumn(SearchEntry, 5);
            Grid.SetColumnSpan(SearchEntry, 1);
            SearchEntry.WidthRequest = 150;
            SearchEntry.HorizontalOptions = LayoutOptions.End;
        }
    }
}
