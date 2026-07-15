# Architecture Rectification & Cleanup Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development or superpowers:executing-plans.

**Goal:** Fix Clean Architecture violations, remove machine-generated patterns, and make the project buildable/debuggable from both VS Code and Visual Studio.

**Architecture:** 
- Phase 1 (parallel): Fix Domain/Infrastructure Color dependency, fix machine-generated XAML, fix build config
- Phase 2 (sequential): Fix Presentation layer to use new converters, update color bindings

**Tech Stack:** .NET 10 MAUI, CommunityToolkit.Mvvm 8.4.0, CommunityToolkit.Maui 9.1.1

## Global Constraints
- ALL model changes must preserve existing property semantics (just change types)
- Domain layer must have zero MAUI UI dependencies
- Infrastructure layer must not reference MAUI `Color`
- Converters live in `src/Presentation/Converters/` (moved from Core)
- All XAML must use `StaticResource` for colors, never inline hex strings in attribute values
- `.sln` (VS2019+ compatible) AND `.slnx` (newer) both supported
- VSCode launch must use `externalConsole` for MAUI Windows debugging

---

### Task 1: Domain + Infrastructure — Remove MAUI Color Dependency

**Files:**
- Modify: `src/Domain/Models/DashboardCard.cs`
- Modify: `src/Domain/Models/ActivityModel.cs`
- Modify: `src/Domain/Models/TrafficModel.cs`
- Modify: `src/Infrastructure/Data/StaticDashboardDataService.cs`

**Changes:**
1. `DashboardCard.cs`: Change `public Color ThemeColor { get; init; } = Colors.Grey;` → `public string ThemeColorHex { get; init; } = "#808080";`
2. `ActivityModel.cs`: Change `public Color DotColor { get; init; } = Colors.Grey;` → `public string DotColorHex { get; init; } = "#808080";`
3. `TrafficModel.cs`: Change `public Color SegmentColor { get; init; } = Colors.Grey;` → `public string SegmentColorHex { get; init; } = "#808080";`
4. `StaticDashboardDataService.cs`: Replace all `Color.FromArgb("#XXXXXX")` with the hex string directly (e.g. `"#F44336"` instead of `Color.FromArgb("#F44336")`). Remove `using MauiHighFidelityDashboard.Domain.Models;` if no longer needed (it is still needed for model types).

**Must do:**
- Change property names to `XxxHex` so we catch all XAML bindings that need updating
- All hex values must include the `#` prefix
- Remove any `using Microsoft.Maui.Graphics;` or similar MAUI Color imports

### Task 2: Build Configuration — .sln + VSCode Debug Fixes

**Files:**
- Create: `MauiHighFidelityDashboard.sln` (VS-compatible .sln, NOT .slnx)
- Modify: `.vscode/launch.json`
- Modify: `.vscode/tasks.json`

**Changes:**
1. Create proper `.sln` file at root with the same project reference as `.slnx`. Format: Visual Studio Version 17, .NET Framework 4.8. Use `Microsoft Visual Studio Solution File, Format Version 12.00`.
2. Fix `launch.json`: Change `console` from `"internalConsole"` to `"externalConsole"`. Fix the `program` path to use runtime identifier correctly (remove `win10-x64` subfolder, just use `net10.0-windows10.0.19041.0/`).
3. Fix `tasks.json`: Ensure `build-windows` task works for preLaunchTask in launch config.

**Must do:**
- The .sln must have proper VS version header so VS doesn't prompt to upgrade
- The VSCode debug config must launch the MAUI Windows app in a visible window

### Task 3: Machine-Generated XAML Cleanup — SidebarView + SalesChartView + MainPage

**Files:**
- Modify: `src/Presentation/Components/SidebarView.xaml`
- Modify: `src/Presentation/Components/SidebarView.xaml.cs`
- Modify: `src/Presentation/Components/SalesChartView.xaml`
- Modify: `src/Presentation/Components/SalesChartView.xaml.cs`
- Modify: `src/Presentation/Components/TrafficChartView.xaml`
- Modify: `src/Presentation/Views/MainPage.xaml`

**Changes:**

**SidebarView:** Replace 21 hardcoded Labels with a data-driven approach. Use a `CollectionView` bound to a list of menu items. Create a simple menu item class. The menu items should be: Dashboard (active), Widgets, UI Elements, Advanced UI, Form Elements, Editors, Charts, Tables, Popups, Notifications, Icons, Maps, User Pages, Error Pages, General Pages, E-Commerce, Email, Calendar, Todo List, Gallery, Documentation.

**SalesChartView:** Replace hardcoded BoxView bars with `BindableLayout.ItemsSource` or horizontal bar rendering. Use the `SalesDataPoints` from the ViewModel. Each data point renders: Online bar (blue) + Store bar (orange) stacked vertically. Day 1-6 labels at bottom.

**TrafficChartView:** Make the donut chart proportional. Currently it's just a Border stroke. Use a stacked horizontal approach or overlapping Borders to show proportional segments. Preserve the Facebook/YouTube/Direct color scheme.

**MainPage.xaml:** Replace all inline hex color strings in RevenueCardView attributes (CardBackground, ChartBackground) with StaticResource references. Create needed color resources in Colors.xaml if missing.

**Must do:**
- SidebarView: Keep `#071A52` background, `WidthRequest="220"`, active item highlight
- SalesChartView: Data-driven bars reading from `SalesData` model (uses `Day`, `Online`, `Store` properties). Scale bar heights proportionally to max value (max is 24).
- TrafficChartView: Keep legend, make donut segments proportional to percentages
- MainPage.xaml: Map hex colors like `#E3F2FD` to StaticResource keys in Colors.xaml

### Task 4: Presentation Layer — Converters, MVVM Fixes, Color Binding Updates

**Depends on: Task 1 (Domain model changes)**

**Files:**
- Create: `src/Presentation/Converters/HexToColorConverter.cs`
- Create: `src/Presentation/Converters/` (directory if needed)
- Modify: `src/Core/ViewModels/BaseViewModel.cs`
- Modify: `src/Core/ViewModels/MainViewModel.cs`
- Modify: `src/Presentation/Views/MainPage.xaml.cs`
- Modify: `src/Presentation/Components/SummaryCardView.xaml`
- Modify: `src/Presentation/Components/ActivityTimelineView.xaml`
- Modify: `src/Presentation/Components/TrafficChartView.xaml` (if it binds Color)
- Modify: `src/Presentation/Resources/Styles/Colors.xaml` (add missing colors)

**Changes:**

**HexToColorConverter:**
```csharp
using System.Globalization;

namespace MauiHighFidelityDashboard.Presentation.Converters;

public class HexToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string hex && !string.IsNullOrEmpty(hex))
            return Color.FromArgb(hex);
        return Colors.Grey;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
```

**BaseViewModel.cs:** Fix MVVM AOT warnings (MVVMTK0045). Replace `[ObservableProperty] private bool _isBusy;` with partial property pattern:
```csharp
[ObservableProperty]
private partial bool IsBusy { get; set; }

[ObservableProperty]
private partial string Title { get; set; } = string.Empty;
```

**MainViewModel.cs:** Remove `LoadDataCommand.ExecuteAsync(null)` from constructor. Add a separate `InitializeAsync()` method or use `WeakReferenceMessenger` pattern.

**MainPage.xaml.cs:** Subscribe to page `Appearing` event to trigger ViewModel initialization.

**SummaryCardView.xaml:** Change `BackgroundColor="{Binding ThemeColor}"` → `BackgroundColor="{Binding ThemeColorHex, Converter={StaticResource HexToColorConverter}}"`.

**ActivityTimelineView.xaml:** Change `BackgroundColor="{Binding DotColor}"` → `BackgroundColor="{Binding DotColorHex, Converter={StaticResource HexToColorConverter}}"`.

**TrafficChartView.xaml:** If it uses SegmentColor, update binding similarly.

**Must do:**
- Register `HexToColorConverter` as a XAML resource (at App.xaml or MainPage level)
- Fix all MVVMTK0045 warnings — none remaining after this task
- MainViewModel must still load data, just not in constructor
- Keep existing `IsBusy` semantics

**Must not do:**
- Don't change ViewModel public API surface unnecessarily
- Don't rename methods that XAML commands bind to
