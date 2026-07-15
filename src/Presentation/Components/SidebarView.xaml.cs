using System.Windows.Input;
using MauiHighFidelityDashboard.Domain.Models;

namespace MauiHighFidelityDashboard.Presentation.Components;

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
        new() { Label = "Dashboard",      Icon = "\u25C6", IsActive = true,  Route = "dashboard" },
        new() { Label = "Widgets",         Icon = "\u25C7",                    Route = "widgets" },
        new() { Label = "UI Elements",     Icon = "\u25C7",                    Route = "uielements" },
        new() { Label = "Advanced UI",     Icon = "\u25C7",                    Route = "advancedui" },
        new() { Label = "Form Elements",   Icon = "\u25C7",                    Route = "formelements" },
        new() { Label = "Editors",         Icon = "\u25C7",                    Route = "editors" },
        new() { Label = "Charts",          Icon = "\u25C7",                    Route = "charts" },
        new() { Label = "Tables",          Icon = "\u25C7",                    Route = "tables" },
        new() { Label = "Popups",          Icon = "\u25C7",                    Route = "popups" },
        new() { Label = "Notifications",   Icon = "\u25C7",                    Route = "notifications" },
        new() { Label = "Icons",           Icon = "\u25C7",                    Route = "icons" },
        new() { Label = "Maps",            Icon = "\u25C7",                    Route = "maps" },
        new() { Label = "User Pages",      Icon = "\u25C7",                    Route = "userpages" },
        new() { Label = "Error Pages",     Icon = "\u25C7",                    Route = "errorpages" },
        new() { Label = "General Pages",   Icon = "\u25C7",                    Route = "generalpages" },
        new() { Label = "E-Commerce",      Icon = "\u25C7",                    Route = "ecommerce" },
        new() { Label = "Email",           Icon = "\u25C7",                    Route = "email" },
        new() { Label = "Calendar",        Icon = "\u25C7",                    Route = "calendar" },
        new() { Label = "Todo List",       Icon = "\u25C7",                    Route = "todolist" },
        new() { Label = "Gallery",         Icon = "\u25C7",                    Route = "gallery" },
        new() { Label = "Documentation",   Icon = "\u25C7",                    Route = "documentation" },
    ];

    public SidebarView()
    {
        InitializeComponent();
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not MenuItemModel item) return;
        if (SelectItemCommand?.CanExecute(item) == true)
            SelectItemCommand.Execute(item);
    }
}
