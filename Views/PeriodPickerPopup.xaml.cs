using CommunityToolkit.Maui.Views;

namespace HighFidelity.Ui.Views;

/// <summary>
/// Modern replacement for the OS action sheet: a compact list of periods with
/// the current selection highlighted. Returns the chosen period string, or null.
/// </summary>
public partial class PeriodPickerPopup : Popup
{
    private static readonly string[] Periods = ["Daily", "Weekly", "Monthly", "Yearly"];

    public PeriodPickerPopup(string currentPeriod)
    {
        InitializeComponent();

        foreach (var period in Periods)
        {
            bool isCurrent = period == currentPeriod;

            var row = new Border
            {
                BackgroundColor = isCurrent ? Color.FromArgb("#FFF1E9") : Colors.Transparent,
                Stroke = Colors.Transparent,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                Padding = new Thickness(14, 10),
                Content = new Grid
                {
                    ColumnDefinitions =
                    [
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                    ],
                    Children =
                    {
                        new Label
                        {
                            Text = period,
                            FontSize = 13,
                            TextColor = isCurrent
                                ? Color.FromArgb("#FA6C41")
                                : Color.FromArgb("#4B5574"),
                            FontAttributes = isCurrent ? FontAttributes.Bold : FontAttributes.None,
                            VerticalOptions = LayoutOptions.Center,
                        },
                        CheckLabel(isCurrent),
                    },
                },
            };

            var tap = new TapGestureRecognizer();
            var choice = period;
            tap.Tapped += async (_, _) => await CloseAsync(choice);
            row.GestureRecognizers.Add(tap);

            OptionsHost.Children.Add(row);
        }
    }

    private static Label CheckLabel(bool visible)
    {
        var check = new Label
        {
            Text = "\uf00c",
            FontFamily = "FontAwesome",
            FontSize = 11,
            TextColor = Color.FromArgb("#FA6C41"),
            VerticalOptions = LayoutOptions.Center,
            IsVisible = visible,
        };
        Grid.SetColumn(check, 1);
        return check;
    }
}
