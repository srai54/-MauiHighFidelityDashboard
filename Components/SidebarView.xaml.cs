using System.Windows.Input;
using MauiHighFidelityDashboard.Models;

namespace MauiHighFidelityDashboard.Components;

public partial class SidebarView : ContentView
{
    public static readonly BindableProperty SelectItemCommandProperty =
        BindableProperty.Create(nameof(SelectItemCommand), typeof(ICommand), typeof(SidebarView));

    public ICommand? SelectItemCommand
    {
        get => (ICommand?)GetValue(SelectItemCommandProperty);
        set => SetValue(SelectItemCommandProperty, value);
    }

    public List<MenuItemModel> MenuItems { get; } =
    [
        new() { Label = "Dashboard",      Icon = "⌂", IsActive = true, Route = "dashboard" },
        new() { Label = "Widgets",        Icon = "❖", Route = "widgets" },
        new() { Label = "UI Elements",    Icon = "▤", Route = "uielements" },
        new() { Label = "Advanced UI",    Icon = "✦", Route = "advancedui" },
        new() { Label = "Form Elements",  Icon = "✎", Route = "formelements" },
        new() { Label = "Editors",        Icon = "✑", Route = "editors" },
        new() { Label = "Charts",         Icon = "◔", Route = "charts" },
        new() { Label = "Tables",         Icon = "▦", Route = "tables" },
        new() { Label = "Popups",         Icon = "❏", Route = "popups" },
        new() { Label = "Notifications",  Icon = "◉", Route = "notifications" },
        new() { Label = "Icons",          Icon = "★", Route = "icons" },
        new() { Label = "Maps",           Icon = "◎", Route = "maps" },
        new() { Label = "User Pages",     Icon = "❍", Route = "userpages" },
        new() { Label = "Error Pages",    Icon = "⚠", Route = "errorpages" },
        new() { Label = "General Pages",  Icon = "❒", Route = "generalpages" },
        new() { Label = "E-Commerce",     Icon = "✤", Route = "ecommerce" },
        new() { Label = "E-mail",         Icon = "✉", Route = "email" },
        new() { Label = "Calendar",       Icon = "▧", Route = "calendar" },
        new() { Label = "Todo List",      Icon = "☑", Route = "todolist" },
        new() { Label = "Gallery",        Icon = "▩", Route = "gallery" },
        new() { Label = "Documentation",  Icon = "❑", Route = "documentation" },
    ];

    public SidebarView()
    {
        InitializeComponent();
    }
}
