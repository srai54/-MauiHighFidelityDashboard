using System.Collections.Specialized;
using MauiHighFidelityDashboard.Charts.Drawables;
using MauiHighFidelityDashboard.Models;

namespace MauiHighFidelityDashboard.Components;

/// <summary>
/// Donut chart + legend, fully data-driven: bind <see cref="ItemsSource"/> to a
/// collection of <see cref="TrafficModel"/> and both stay in sync with it.
/// </summary>
public partial class TrafficChartView : ContentView
{
    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable<TrafficModel>), typeof(TrafficChartView),
            propertyChanged: (bindable, oldValue, newValue) => ((TrafficChartView)bindable)
                .OnItemsSourceChanged(oldValue as IEnumerable<TrafficModel>, newValue as IEnumerable<TrafficModel>));

    public IEnumerable<TrafficModel>? ItemsSource
    {
        get => (IEnumerable<TrafficModel>?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public TrafficChartView()
    {
        InitializeComponent();
    }

    private void OnItemsSourceChanged(IEnumerable<TrafficModel>? oldItems, IEnumerable<TrafficModel>? newItems)
    {
        // Observable sources (the usual case) redraw the donut as items load in.
        if (oldItems is INotifyCollectionChanged oldObservable)
            oldObservable.CollectionChanged -= OnItemsChanged;
        if (newItems is INotifyCollectionChanged newObservable)
            newObservable.CollectionChanged += OnItemsChanged;

        RenderDonut();
    }

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e) => RenderDonut();

    // Segments render in reverse list order so the smallest source starts the ring
    // clockwise from 12 o'clock (yellow sliver, orange arc, blue arc — as in the reference).
    private void RenderDonut()
    {
        var items = ItemsSource?.Reverse() ?? [];
        DonutCanvas.Drawable = new DonutChartDrawable
        {
            Segments = [.. items.Select(t => new DonutSegment((float)t.Percentage, Color.FromArgb(t.SegmentColorHex)))]
        };
        DonutCanvas.Invalidate();
    }
}
