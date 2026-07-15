namespace MauiHighFidelityDashboard.Presentation.Components;

public partial class SidebarView : ContentView
{
    public List<SidebarMenuItem> MenuItems { get; } =
    [
        new() { Label = "Dashboard", Icon = "\u25C6", IsActive = true },
        new() { Label = "Widgets", Icon = "\u25C7" },
        new() { Label = "UI Elements", Icon = "\u25C7" },
        new() { Label = "Advanced UI", Icon = "\u25C7" },
        new() { Label = "Form Elements", Icon = "\u25C7" },
        new() { Label = "Editors", Icon = "\u25C7" },
        new() { Label = "Charts", Icon = "\u25C7" },
        new() { Label = "Tables", Icon = "\u25C7" },
        new() { Label = "Popups", Icon = "\u25C7" },
        new() { Label = "Notifications", Icon = "\u25C7" },
        new() { Label = "Icons", Icon = "\u25C7" },
        new() { Label = "Maps", Icon = "\u25C7" },
        new() { Label = "User Pages", Icon = "\u25C7" },
        new() { Label = "Error Pages", Icon = "\u25C7" },
        new() { Label = "General Pages", Icon = "\u25C7" },
        new() { Label = "E-Commerce", Icon = "\u25C7" },
        new() { Label = "Email", Icon = "\u25C7" },
        new() { Label = "Calendar", Icon = "\u25C7" },
        new() { Label = "Todo List", Icon = "\u25C7" },
        new() { Label = "Gallery", Icon = "\u25C7" },
        new() { Label = "Documentation", Icon = "\u25C7" },
    ];

    public SidebarView()
    {
        InitializeComponent();
    }
}

public class SidebarMenuItem
{
    public string Label { get; init; } = string.Empty;
    public string Icon { get; init; } = "\u25C7";
    public bool IsActive { get; init; }
}
