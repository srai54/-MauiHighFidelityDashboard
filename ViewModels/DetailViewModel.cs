using System.Collections.ObjectModel;
using HighFidelity.Ui.Models;

namespace HighFidelity.Ui.ViewModels;

public partial class DetailViewModel : BaseViewModel
{
    public ObservableCollection<SectionItem> Items { get; } = [];

    public string SectionSubtitle { get; private set; } = string.Empty;

    public void LoadSection(string sectionTitle)
    {
        Title = sectionTitle;

        var (subtitle, items) = Catalog.TryGetValue(sectionTitle, out var section)
            ? section
            : ("Overview and recent records", DefaultItems(sectionTitle));

        SectionSubtitle = subtitle;
        OnPropertyChanged(nameof(SectionSubtitle));
        OnPropertyChanged(nameof(Title));

        Items.Clear();
        foreach (var item in items)
            Items.Add(item);
    }

    private static List<SectionItem> DefaultItems(string section) =>
    [
        new($"{section} Report", "Generated 2 hours ago", "Ready", "#2BC155"),
        new($"{section} Settings", "Last edited by Admin", "Active", "#2196F3"),
        new($"{section} Archive", "128 records stored", "Synced", "#6259CE"),
    ];

    private static readonly Dictionary<string, (string Subtitle, List<SectionItem> Items)> Catalog = new()
    {
        ["Widgets"] = ("Reusable dashboard widgets", [
            new("Revenue Widget", "Live earnings tile • v2.4", "Active", "#2BC155"),
            new("Sales Spark-line", "Mini trend for 30 days • v1.9", "Active", "#2BC155"),
            new("Traffic Donut", "Source split by channel • v3.0", "Beta", "#FF5B1F"),
            new("Weather Widget", "Location-based forecast • v1.2", "Disabled", "#6259CE"),
            new("Clock Widget", "Multi-timezone display • v2.0", "Active", "#2BC155"),
        ]),
        ["UI Elements"] = ("Buttons, cards, badges and more", [
            new("Buttons", "12 variants • filled, outline, ghost", "Stable", "#2BC155"),
            new("Cards", "8 layouts • shadow and border styles", "Stable", "#2BC155"),
            new("Badges", "Status pills used in Order table", "Stable", "#2BC155"),
            new("Modals", "Confirmation and form dialogs", "Preview", "#FF5B1F"),
            new("Avatars", "Circle initials and image avatars", "Stable", "#2BC155"),
        ]),
        ["Advanced UI"] = ("Composite interaction patterns", [
            new("Timeline", "Activity feed with icon rail", "In Use", "#2196F3"),
            new("Kanban Board", "Drag & drop task columns", "Preview", "#FF5B1F"),
            new("Tree View", "Nested navigation structure", "Stable", "#2BC155"),
            new("Stepper Wizard", "Multi-step checkout flow", "In Use", "#2196F3"),
            new("Data Grid Pro", "Sorting, filters, pagination", "In Use", "#2196F3"),
        ]),
        ["Form Elements"] = ("Inputs and validation", [
            new("Text Inputs", "Entry with floating labels", "Stable", "#2BC155"),
            new("Date Picker", "Range and single date modes", "Stable", "#2BC155"),
            new("Dropdowns", "Single and multi-select", "Stable", "#2BC155"),
            new("File Upload", "Drag & drop with progress", "Beta", "#FF5B1F"),
            new("Validation", "Inline rules with error hints", "Stable", "#2BC155"),
        ]),
        ["Editors"] = ("Rich text and code editing", [
            new("Markdown Editor", "Split preview mode", "Active", "#2BC155"),
            new("Rich Text Editor", "Toolbar with 18 actions", "Active", "#2BC155"),
            new("Code Editor", "Syntax highlight • 12 languages", "Beta", "#FF5B1F"),
            new("JSON Editor", "Tree and raw views", "Active", "#2BC155"),
        ]),
        ["Charts"] = ("Visualisations used across the app", [
            new("Spline Chart", "Online vs Store sales", "Live", "#2BC155"),
            new("Donut Chart", "Traffic source split", "Live", "#2BC155"),
            new("Bar Chart", "Revenue status mini bars", "Live", "#2BC155"),
            new("Area Chart", "Page view trend", "Live", "#2BC155"),
            new("Candlestick", "Market data preview", "Planned", "#6259CE"),
        ]),
        ["Tables"] = ("Data grids and lists", [
            new("Order Status Table", "30 rows • search + pagination", "Live", "#2BC155"),
            new("Invoice Table", "Export to CSV enabled", "Live", "#2BC155"),
            new("Customer Table", "Inline row editing", "Beta", "#FF5B1F"),
            new("Audit Log", "Immutable event history", "Live", "#2BC155"),
        ]),
        ["Popups"] = ("Dialogs, alerts and toasts", [
            new("Confirm Dialog", "Used by Delete order action", "In Use", "#2196F3"),
            new("Action Sheet", "Used by Bounce Rate period", "In Use", "#2196F3"),
            new("Toast", "Auto-dismiss notifications", "Preview", "#FF5B1F"),
            new("Snackbar", "Undo pattern support", "Preview", "#FF5B1F"),
        ]),
        ["Notifications"] = ("Latest system notifications", [
            new("Server backup completed", "Today, 04:12 AM", "Info", "#2196F3"),
            new("New order #12406 received", "Today, 09:31 AM", "Success", "#2BC155"),
            new("Payment gateway latency", "Yesterday, 11:48 PM", "Warning", "#FF5B1F"),
            new("Password expires in 5 days", "Yesterday, 08:00 AM", "Notice", "#6259CE"),
            new("Weekly report is ready", "Monday, 07:00 AM", "Info", "#2196F3"),
        ]),
        ["Icons"] = ("Icon sets available to the app", [
            new("Line Icons", "480 glyphs • 24px grid", "Installed", "#2BC155"),
            new("Solid Icons", "512 glyphs • filled style", "Installed", "#2BC155"),
            new("Brand Icons", "96 logos • social networks", "Installed", "#2BC155"),
            new("Emoji Set", "Unicode 15 coverage", "System", "#6259CE"),
        ]),
        ["Maps"] = ("Geo views and locations", [
            new("Sales by Region", "Choropleth of 24 countries", "Live", "#2BC155"),
            new("Warehouse Map", "6 fulfilment centres", "Live", "#2BC155"),
            new("Delivery Routes", "Realtime courier tracking", "Beta", "#FF5B1F"),
            new("Store Locator", "112 retail locations", "Live", "#2BC155"),
        ]),
        ["User Pages"] = ("Team members", [
            new("Nikolai Petrov", "Product Manager • Online", "Admin", "#FF5B1F"),
            new("Panshi Rao", "Sales Lead • Online", "Editor", "#2196F3"),
            new("Rasel Ahmed", "Content Writer • Away", "Editor", "#2196F3"),
            new("Reshmi Nair", "UX Designer • Online", "Viewer", "#6259CE"),
            new("Jenathon Cole", "Support Agent • Offline", "Viewer", "#6259CE"),
        ]),
        ["Error Pages"] = ("Error templates", [
            new("404 Not Found", "Custom illustration + search", "Ready", "#2BC155"),
            new("500 Server Error", "Retry with status page link", "Ready", "#2BC155"),
            new("403 Forbidden", "Role-based access message", "Ready", "#2BC155"),
            new("Maintenance", "Scheduled downtime banner", "Draft", "#FF5B1F"),
        ]),
        ["General Pages"] = ("Static site pages", [
            new("About Us", "Company story and team", "Published", "#2BC155"),
            new("Pricing", "3 plans with comparison", "Published", "#2BC155"),
            new("FAQ", "28 questions in 6 groups", "Published", "#2BC155"),
            new("Terms of Service", "Last updated Jun 2026", "Review", "#FF5B1F"),
        ]),
        ["E-Commerce"] = ("Store overview", [
            new("Wireless Headset", "SKU HD-220 • $89.00 • 214 in stock", "Best Seller", "#FF5B1F"),
            new("Smart Watch S3", "SKU SW-310 • $199.00 • 78 in stock", "Active", "#2BC155"),
            new("USB-C Dock", "SKU DK-104 • $59.00 • 12 in stock", "Low Stock", "#F7284A"),
            new("4K Monitor 27\"", "SKU MN-427 • $329.00 • 41 in stock", "Active", "#2BC155"),
            new("Mechanical Keyboard", "SKU KB-880 • $129.00 • 0 in stock", "Sold Out", "#6259CE"),
        ]),
        ["E-mail"] = ("Inbox • 4 unread", [
            new("Invoice #12406 paid", "billing@store.com • 09:32 AM", "Unread", "#FF5B1F"),
            new("Q3 marketing plan", "marketing@corp.com • 08:15 AM", "Unread", "#FF5B1F"),
            new("Weekly analytics digest", "reports@corp.com • Yesterday", "Read", "#6259CE"),
            new("Password reset confirmed", "security@corp.com • Yesterday", "Read", "#6259CE"),
            new("New comment on article", "cms@corp.com • Monday", "Read", "#6259CE"),
        ]),
        ["Calendar"] = ("Upcoming events", [
            new("Sprint Review", "Today • 2:00 PM – 3:00 PM", "Meeting", "#2196F3"),
            new("Release v2.5", "Tomorrow • All day", "Milestone", "#FF5B1F"),
            new("1:1 with Panshi", "Thu • 10:30 AM", "Meeting", "#2196F3"),
            new("Marketing sync", "Fri • 4:00 PM", "Meeting", "#2196F3"),
            new("Inventory audit", "Next Mon • 9:00 AM", "Task", "#6259CE"),
        ]),
        ["Todo List"] = ("Sprint tasks", [
            new("Fix order table pagination", "Assigned to Nikolai", "Done", "#2BC155"),
            new("Add bounce-rate dropdown", "Assigned to Reshmi", "Done", "#2BC155"),
            new("Write API integration docs", "Assigned to Rasel", "In Progress", "#FF5B1F"),
            new("Refresh brand icons", "Assigned to Panshi", "In Progress", "#FF5B1F"),
            new("QA pass on Windows build", "Assigned to Jenathon", "To Do", "#6259CE"),
        ]),
        ["Gallery"] = ("Media library", [
            new("Product Shoot – June", "48 photos • 1.2 GB", "Synced", "#2BC155"),
            new("Dashboard Screenshots", "12 images • 96 MB", "Synced", "#2BC155"),
            new("Launch Event", "134 photos • 3.8 GB", "Uploading", "#FF5B1F"),
            new("Brand Assets", "SVG + PNG logo pack", "Synced", "#2BC155"),
        ]),
        ["Documentation"] = ("Guides and references", [
            new("Getting Started", "Install, build and run", "Updated", "#2BC155"),
            new("Architecture Guide", "Clean layers + MVVM", "Updated", "#2BC155"),
            new("API Reference", "IDashboardDataService contract", "Updated", "#2BC155"),
            new("Styling Guide", "Colors, typography, spacing", "Draft", "#FF5B1F"),
            new("Release Notes", "v2.4 → v2.5 changes", "Updated", "#2BC155"),
        ]),
        ["Dashboard"] = ("You are here", [
            new("Overview of Latest Month", "Earnings, sales and orders", "Live", "#2BC155"),
        ]),
    };
}
