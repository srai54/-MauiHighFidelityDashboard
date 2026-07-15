# MauiHighFidelityDashboard - Design Specification

## Overview
A high-fidelity .NET MAUI (.NET 8) dashboard application using MVVM, replicated from a provided screenshot specification.

## Architecture
- **Pattern**: MVVM with CommunityToolkit.Mvvm
- **Framework**: .NET 8 MAUI
- **NuGet**: CommunityToolkit.Maui, CommunityToolkit.Mvvm
- **Styling**: ResourceDictionary (Colors.xaml, Styles.xaml, Fonts.xaml)
- **DI**: MauiProgram.cs with standard MAUI DI

## Project Structure
```
/src
  /Models (5 files)
  /ViewModels (MainViewModel.cs)
  /Views (MainPage.xaml + code-behind - no UI logic)
  /Components (8 ContentViews)
  /Resources/Styles (Colors.xaml, Styles.xaml, Fonts.xaml)
```

## Layout
- Root Grid: Sidebar (220px fixed) + Main Content (ScrollView)
- Main Content sections: Header, Sales Chart + Traffic Chart, 4 Summary Cards, 4 Analytics Cards, Activity Timeline, Orders Table

## Components
1. **SidebarView** - 220px nav with icon+text items, #071A52 background
2. **DashboardHeaderView** - Title, metrics, action button (#FF5B1F)
3. **SalesChartView** - Spline chart with Daily/Weekly/Monthly/Yearly tabs, Online vs Store series
4. **TrafficChartView** - Donut chart (Facebook 34%, YouTube 55%, Direct Search 11%)
5. **SummaryCardView** - Reusable card with Title, Amount, Icon, ThemeColor (4 instances)
6. **RevenueCardView** - Reusable analytics card with mini chart (4 instances)
7. **ActivityTimelineView** - Vertical timeline with colored dots (4 entries)
8. **OrderTableView** - Table with search, filter, Add/Filter/Notification/Settings buttons, status badges

## Data Flow
- MainViewModel exposes ObservableCollection<DashboardCard>, <ActivityModel>, <OrderModel>, <TrafficModel>, <SalesData>
- All views bind via {Binding} with no code-behind UI logic
- Static data initialized in ViewModel constructor

## Color Palette
- Sidebar: #071A52 | PrimaryOrange: #FF5B1F | BlueAccent: #4FC3F7
- SuccessGreen: #4CAF50 | PinkAccent: #EC407A | PageBackground: #F7F8FC | CardBackground: #FFFFFF

## Models
- DashboardCard (Title, Amount, Icon, ThemeColor)
- ActivityModel (Title, Description, Time, DotColor)
- TrafficModel (Source, Percentage, SegmentColor)
- OrderModel (Invoice, Customer, Country, Price, Status)
- SalesData (Day, Online, Store)
