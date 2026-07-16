namespace MauiHighFidelityDashboard.Components;

public partial class OrderTableView : ContentView
{
    // The table needs at least this much width to show all columns;
    // below it (phones) the table pans horizontally instead of clipping.
    private const double MinTableWidth = 520;

    public OrderTableView()
    {
        InitializeComponent();
        TableScroll.SizeChanged += (_, _) =>
        {
            if (TableScroll.Width > 0)
                TableContent.WidthRequest = Math.Max(TableScroll.Width, MinTableWidth);
        };
    }
}
