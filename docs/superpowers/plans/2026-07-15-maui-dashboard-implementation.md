# MauiHighFidelityDashboard Implementation Plan

> **For agentic workers:** Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a high-fidelity .NET MAUI dashboard application with MVVM pattern.

**Architecture:** .NET 8 MAUI app using CommunityToolkit.Mvvm for MVVM, CommunityToolkit.Maui for UI helpers, ResourceDictionary for styling, reusable ContentViews.

**Tech Stack:** .NET 8, MAUI, CommunityToolkit.Maui, CommunityToolkit.Mvvm, XAML

## Global Constraints
- .NET 8 MAUI project
- MVVM pattern with CommunityToolkit.Mvvm
- Static data only - no database
- No code-behind UI logic
- Reusable ContentViews for all dashboard components
- ResourceDictionary for Colors, Styles, Fonts
- Project structure under /src

---

### Task 1: Scaffold .NET MAUI Project

**Files:**
- Create: `MauiHighFidelityDashboard.csproj`
- Create: `MauiProgram.cs`
- Create: `App.xaml` + `App.xaml.cs`
- Create: `AppShell.xaml` + `AppShell.xaml.cs`
- Create: `Platforms/**` (minimal)

- [ ] **Step 1: Create project with dotnet new**

```bash
cd C:\Users\55\source\repos\MauiHighFidelityDashboard
dotnet new maui -n MauiHighFidelityDashboard --framework net8.0 -o .
```

- [ ] **Step 2: Add NuGet packages**

```bash
dotnet add package CommunityToolkit.Maui --version 9.1.1
dotnet add package CommunityToolkit.Mvvm --version 8.4.0
```

- [ ] **Step 3: Create directory structure**

```bash
New-Item -ItemType Directory -Path "src\Models" -Force
New-Item -ItemType Directory -Path "src\ViewModels" -Force
New-Item -ItemType Directory -Path "src\Views" -Force
New-Item -ItemType Directory -Path "src\Components" -Force
New-Item -ItemType Directory -Path "src\Resources\Styles" -Force
```

- [ ] **Step 4: Update MauiProgram.cs with DI and CommunityToolkit**

Register MainViewModel, all ContentViews, and CommunityToolkit.Maui.

```csharp
using CommunityToolkit.Maui;
using MauiHighFidelityDashboard.ViewModels;
using MauiHighFidelityDashboard.Views;
using Microsoft.Extensions.Logging;

namespace MauiHighFidelityDashboard;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
```

- [ ] **Step 5: Update App.xaml to merge resource dictionaries**

```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Application xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MauiHighFidelityDashboard.App">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="src/Resources/Styles/Colors.xaml" />
                <ResourceDictionary Source="src/Resources/Styles/Styles.xaml" />
                <ResourceDictionary Source="src/Resources/Styles/Fonts.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

- [ ] **Step 6: Update AppShell to use MainPage**

```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Shell xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
       xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
       xmlns:views="clr-namespace:MauiHighFidelityDashboard.Views"
       x:Class="MauiHighFidelityDashboard.AppShell"
       Shell.FlyoutBehavior="Disabled"
       Shell.NavBarIsVisible="False">
    <ShellContent ContentTemplate="{DataTemplate views:MainPage}" />
</Shell>
```

---

### Task 2: Create Resource Dictionaries

**Files:**
- Create: `src/Resources/Styles/Colors.xaml`
- Create: `src/Resources/Styles/Styles.xaml`
- Create: `src/Resources/Styles/Fonts.xaml`

- [ ] **Step 1: Create Colors.xaml**

```xml
<?xml version="1.0" encoding="UTF-8" ?>
<?xaml-comp compile="true" ?>
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
    
    <!-- Sidebar -->
    <Color x:Key="SidebarColor">#071A52</Color>
    
    <!-- Accent Colors -->
    <Color x:Key="PrimaryOrange">#FF5B1F</Color>
    <Color x:Key="BlueAccent">#4FC3F7</Color>
    <Color x:Key="SuccessGreen">#4CAF50</Color>
    <Color x:Key="PinkAccent">#EC407A</Color>
    <Color x:Key="YellowAccent">#FFC107</Color>
    <Color x:Key="PurpleAccent">#9C27B0</Color>
    <Color x:Key="RedAccent">#F44336</Color>
    
    <!-- Page -->
    <Color x:Key="PageBackground">#F7F8FC</Color>
    <Color x:Key="CardBackground">#FFFFFF</Color>
    
    <!-- Text -->
    <Color x:Key="TextPrimary">#2D3436</Color>
    <Color x:Key="TextSecondary">#636E72</Color>
    <Color x:Key="TextLight">#B2BEC3</Color>
    <Color x:Key="TextWhite">#FFFFFF</Color>
    
    <!-- Chart Colors -->
    <Color x:Key="ChartOnline">#4FC3F7</Color>
    <Color x:Key="ChartStore">#FF5B1F</Color>
    
    <!-- Traffic Colors -->
    <Color x:Key="TrafficFacebook">#2196F3</Color>
    <Color x:Key="TrafficYoutube">#FF5722</Color>
    <Color x:Key="TrafficDirect">#FFC107</Color>
    
    <!-- Status Colors -->
    <Color x:Key="StatusProcess">#F44336</Color>
    <Color x:Key="StatusOpen">#4CAF50</Color>
    <Color x:Key="StatusOnHold">#2196F3</Color>
</ResourceDictionary>
```

- [ ] **Step 2: Create Styles.xaml**

```xml
<?xml version="1.0" encoding="UTF-8" ?>
<?xaml-comp compile="true" ?>
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
    
    <!-- Card Style -->
    <Style x:Key="CardStyle" TargetType="Frame">
        <Setter Property="BackgroundColor" Value="{StaticResource CardBackground}" />
        <Setter Property="CornerRadius" Value="12" />
        <Setter Property="HasShadow" Value="True" />
        <Setter Property="Padding" Value="16" />
        <Setter Property="Margin" Value="6" />
    </Style>

    <Style x:Key="CardBorderStyle" TargetType="Border">
        <Setter Property="BackgroundColor" Value="{StaticResource CardBackground}" />
        <Setter Property="StrokeShape" Value="RoundRectangle 12" />
        <Setter Property="Padding" Value="16" />
        <Setter Property="Margin" Value="6" />
        <Setter Property="Stroke" Value="Transparent" />
    </Style>
    
    <!-- Section Title -->
    <Style x:Key="SectionTitle" TargetType="Label">
        <Setter Property="FontSize" Value="20" />
        <Setter Property="FontAttributes" Value="Bold" />
        <Setter Property="TextColor" Value="{StaticResource TextPrimary}" />
        <Setter Property="Margin" Value="0,0,0,4" />
    </Style>
    
    <Style x:Key="SectionSubtitle" TargetType="Label">
        <Setter Property="FontSize" Value="13" />
        <Setter Property="TextColor" Value="{StaticResource TextSecondary}" />
        <Setter Property="Margin" Value="0,0,0,16" />
    </Style>
    
    <!-- Metric Value -->
    <Style x:Key="MetricValue" TargetType="Label">
        <Setter Property="FontSize" Value="28" />
        <Setter Property="FontAttributes" Value="Bold" />
        <Setter Property="TextColor" Value="{StaticResource TextPrimary}" />
    </Style>
    
    <Style x:Key="MetricLabel" TargetType="Label">
        <Setter Property="FontSize" Value="12" />
        <Setter Property="TextColor" Value="{StaticResource TextSecondary}" />
    </Style>

    <!-- Primary Button -->
    <Style x:Key="PrimaryButton" TargetType="Button">
        <Setter Property="BackgroundColor" Value="{StaticResource PrimaryOrange}" />
        <Setter Property="TextColor" Value="{StaticResource TextWhite}" />
        <Setter Property="CornerRadius" Value="8" />
        <Setter Property="FontAttributes" Value="Bold" />
        <Setter Property="FontSize" Value="13" />
        <Setter Property="Padding" Value="16,8" />
    </Style>
</ResourceDictionary>
```

- [ ] **Step 3: Create Fonts.xaml**

```xml
<?xml version="1.0" encoding="UTF-8" ?>
<?xaml-comp compile="true" ?>
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
</ResourceDictionary>
```

---

### Task 3: Create All Models

**Files:**
- Create: `src/Models/DashboardCard.cs`
- Create: `src/Models/ActivityModel.cs`
- Create: `src/Models/TrafficModel.cs`
- Create: `src/Models/OrderModel.cs`
- Create: `src/Models/SalesData.cs`

- [ ] **Step 1: Create DashboardCard.cs**

```csharp
namespace MauiHighFidelityDashboard.Models;

public class DashboardCard
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Icon { get; set; } = string.Empty;
    public Color ThemeColor { get; set; } = Colors.Grey;
}
```

- [ ] **Step 2: Create ActivityModel.cs**

```csharp
namespace MauiHighFidelityDashboard.Models;

public class ActivityModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public Color DotColor { get; set; } = Colors.Grey;
}
```

- [ ] **Step 3: Create TrafficModel.cs**

```csharp
namespace MauiHighFidelityDashboard.Models;

public class TrafficModel
{
    public string Source { get; set; } = string.Empty;
    public double Percentage { get; set; }
    public Color SegmentColor { get; set; } = Colors.Grey;
}
```

- [ ] **Step 4: Create OrderModel.cs**

```csharp
namespace MauiHighFidelityDashboard.Models;

public class OrderModel
{
    public int Invoice { get; set; }
    public string Customer { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
}
```

- [ ] **Step 5: Create SalesData.cs**

```csharp
namespace MauiHighFidelityDashboard.Models;

public class SalesData
{
    public int Day { get; set; }
    public double Online { get; set; }
    public double Store { get; set; }
}
```

---

### Task 4: Create MainViewModel

**Files:**
- Create: `src/ViewModels/MainViewModel.cs`

- [ ] **Step 1: Create MainViewModel with all ObservableCollections and static data**

```csharp
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MauiHighFidelityDashboard.Models;

namespace MauiHighFidelityDashboard.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public ObservableCollection<DashboardCard> DashboardCards { get; } = new();
    public ObservableCollection<ActivityModel> Activities { get; } = new();
    public ObservableCollection<OrderModel> Orders { get; } = new();
    public ObservableCollection<TrafficModel> TrafficSources { get; } = new();
    public ObservableCollection<SalesData> SalesDataPoints { get; } = new();

    // Header Metrics
    public string CurrentMonthEarnings => "$3,468.96";
    public string CurrentMonthSales => "82";
    public string DashboardTitle => "Dashboard";
    public string DashboardSubtitle => "Overview of Latest Month";

    public MainViewModel()
    {
        LoadDashboardCards();
        LoadActivities();
        LoadOrders();
        LoadTrafficSources();
        LoadSalesData();
    }

    private void LoadDashboardCards()
    {
        DashboardCards.Add(new DashboardCard
        {
            Title = "Wallet Balance",
            Amount = 4567.53m,
            Icon = "wallet.png",
            ThemeColor = Color.FromArgb("#F44336")
        });
        DashboardCards.Add(new DashboardCard
        {
            Title = "Referral Earning",
            Amount = 1689.53m,
            Icon = "referral.png",
            ThemeColor = Color.FromArgb("#2196F3")
        });
        DashboardCards.Add(new DashboardCard
        {
            Title = "Estimate Sales",
            Amount = 2851.53m,
            Icon = "sales.png",
            ThemeColor = Color.FromArgb("#4CAF50")
        });
        DashboardCards.Add(new DashboardCard
        {
            Title = "Earning",
            Amount = 52567.53m,
            Icon = "earning.png",
            ThemeColor = Color.FromArgb("#EC407A")
        });
    }

    private void LoadActivities()
    {
        Activities.Add(new ActivityModel
        {
            Title = "Task Updated",
            Description = "Nikolai Updated a Task",
            Time = "42 Mins Ago",
            DotColor = Color.FromArgb("#9C27B0")
        });
        Activities.Add(new ActivityModel
        {
            Title = "Deal Added",
            Description = "Panshi Updated a Task",
            Time = "1 Day Ago",
            DotColor = Color.FromArgb("#EC407A")
        });
        Activities.Add(new ActivityModel
        {
            Title = "Published Article",
            Description = "Rasel Published an Article",
            Time = "42 Mins Ago",
            DotColor = Color.FromArgb("#4FC3F7")
        });
        Activities.Add(new ActivityModel
        {
            Title = "Dock Updated",
            Description = "",
            Time = "1 Day Ago",
            DotColor = Color.FromArgb("#FFC107")
        });
    }

    private void LoadOrders()
    {
        Orders.Add(new OrderModel { Invoice = 12386, Customer = "Charly Dues", Country = "Brazil", Price = 299m, Status = "Process" });
        Orders.Add(new OrderModel { Invoice = 12386, Customer = "Marko", Country = "Italy", Price = 2642m, Status = "Open" });
        Orders.Add(new OrderModel { Invoice = 12386, Customer = "Deniyel Onak", Country = "Russia", Price = 981m, Status = "On Hold" });
        Orders.Add(new OrderModel { Invoice = 12386, Customer = "Belgiri Bastana", Country = "Korea", Price = 369m, Status = "Process" });
        Orders.Add(new OrderModel { Invoice = 12386, Customer = "Vaska Simon", Country = "Japan", Price = 1240m, Status = "Open" });
    }

    private void LoadTrafficSources()
    {
        TrafficSources.Add(new TrafficModel { Source = "Facebook", Percentage = 34, SegmentColor = Color.FromArgb("#2196F3") });
        TrafficSources.Add(new TrafficModel { Source = "Youtube", Percentage = 55, SegmentColor = Color.FromArgb("#FF5722") });
        TrafficSources.Add(new TrafficModel { Source = "Direct Search", Percentage = 11, SegmentColor = Color.FromArgb("#FFC107") });
    }

    private void LoadSalesData()
    {
        SalesDataPoints.Add(new SalesData { Day = 1, Online = 2, Store = 3 });
        SalesDataPoints.Add(new SalesData { Day = 2, Online = 12, Store = 11 });
        SalesDataPoints.Add(new SalesData { Day = 3, Online = 3, Store = 5 });
        SalesDataPoints.Add(new SalesData { Day = 4, Online = 6, Store = 14 });
        SalesDataPoints.Add(new SalesData { Day = 5, Online = 10, Store = 12 });
        SalesDataPoints.Add(new SalesData { Day = 6, Online = 24, Store = 16 });
    }
}
```

---

### Task 5: Create SidebarView

**Files:**
- Create: `src/Components/SidebarView.xaml`
- Create: `src/Components/SidebarView.xaml.cs`

- [ ] **Step 1: Create SidebarView.xaml**

Sidebar with 220px width, #071A52 background, menu items with icons (use Unicode symbols as icon replacements since we don't have icon fonts), Dashboard selected state.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MauiHighFidelityDashboard.Components.SidebarView"
             BackgroundColor="#071A52"
             WidthRequest="220"
             HeightRequest="700">
    
    <ScrollView>
        <VerticalStackLayout Padding="0">
            
            <!-- Logo Area -->
            <VerticalStackLayout Padding="20,24,20,20">
                <Label Text="MAUI DASHBOARD"
                       TextColor="White"
                       FontSize="16"
                       FontAttributes="Bold" />
                <BoxView HeightRequest="1"
                         Color="#1A3A6A"
                         Margin="0,12,0,0" />
            </VerticalStackLayout>

            <!-- Menu Items -->
            <VerticalStackLayout Spacing="2" Padding="0,0,0,20">
                <Label Text="◆ Dashboard" TextColor="White" FontSize="14" Padding="20,12" BackgroundColor="#1A3A6A" />
                <Label Text="◈ Widgets" TextColor="#8A9BB5" FontSize="14" Padding="20,12" />
                <Label Text="◈ UI Elements" TextColor="#8A9BB5" FontSize="14" Padding="20,12" />
                <Label Text="◈ Advanced UI" TextColor="#8A9BB5" FontSize="14" Padding="20,12" />
                <Label Text="◈ Form Elements" TextColor="#8A9BB5" FontSize="14" Padding="20,12" />
                <Label Text="◈ Editors" TextColor="#8A9BB5" FontSize="14" Padding="20,12" />
                <Label Text="◈ Charts" TextColor="#8A9BB5" FontSize="14" Padding="20,12" />
                <Label Text="◈ Tables" TextColor="#8A9BB5" FontSize="14" Padding="20,12" />
                <Label Text="◈ Popups" TextColor="#8A9BB5" FontSize="14" Padding="20,12" />
                <Label Text="◈ Notifications" TextColor="#8A9BB5" FontSize="14" Padding="20,12" />
                <Label Text="◈ Icons" TextColor="#8A9BB5" FontSize="14" Padding="20,12" />
                <Label Text="◈ Maps" TextColor="#8A9BB5" FontSize="14" Padding="20,12" />
                <Label Text="◈ User Pages" TextColor="#8A9BB5" FontSize="14" Padding="20,12" />
                <Label Text="◈ Error Pages" TextColor="#8A9BB5" FontSize="14" Padding="20,12" />
                <Label Text="◈ General Pages" TextColor="#8A9BB5" FontSize="14" Padding="20,12" />
                <Label Text="◈ E-Commerce" TextColor="#8A9BB5" FontSize="14" Padding="20,12" />
                <Label Text="◈ Email" TextColor="#8A9BB5" FontSize="14" Padding="20,12" />
                <Label Text="◈ Calendar" TextColor="#8A9BB5" FontSize="14" Padding="20,12" />
                <Label Text="◈ Todo List" TextColor="#8A9BB5" FontSize="14" Padding="20,12" />
                <Label Text="◈ Gallery" TextColor="#8A9BB5" FontSize="14" Padding="20,12" />
                <Label Text="◈ Documentation" TextColor="#8A9BB5" FontSize="14" Padding="20,12" />
            </VerticalStackLayout>
        </VerticalStackLayout>
    </ScrollView>
</ContentView>
```

- [ ] **Step 2: Create SidebarView.xaml.cs**

```csharp
namespace MauiHighFidelityDashboard.Components;

public partial class SidebarView : ContentView
{
    public SidebarView()
    {
        InitializeComponent();
    }
}
```

---

### Task 6: Create DashboardHeaderView

**Files:**
- Create: `src/Components/DashboardHeaderView.xaml`
- Create: `src/Components/DashboardHeaderView.xaml.cs`

- [ ] **Step 1: Create DashboardHeaderView.xaml**

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MauiHighFidelityDashboard.Components.DashboardHeaderView"
             Padding="24,20">
    
    <Grid ColumnDefinitions="*,Auto">
        
        <!-- Left: Title + Subtitle -->
        <VerticalStackLayout>
            <Label Text="{Binding DashboardTitle}"
                   Style="{StaticResource SectionTitle}"
                   FontSize="28"
                   Margin="0" />
            <Label Text="{Binding DashboardSubtitle}"
                   Style="{StaticResource SectionSubtitle}"
                   Margin="0,4,0,0" />
        </VerticalStackLayout>
        
        <!-- Right: Metrics + Button -->
        <HorizontalStackLayout Grid.Column="1" Spacing="24" VerticalOptions="Center">
            
            <!-- Metric 1 -->
            <VerticalStackLayout Spacing="2">
                <Label Text="{Binding CurrentMonthEarnings}"
                       FontSize="22"
                       FontAttributes="Bold"
                       TextColor="{StaticResource TextPrimary}" />
                <Label Text="Current Month Earnings"
                       FontSize="11"
                       TextColor="{StaticResource TextSecondary}" />
            </VerticalStackLayout>
            
            <!-- Metric 2 -->
            <VerticalStackLayout Spacing="2">
                <Label Text="{Binding CurrentMonthSales}"
                       FontSize="22"
                       FontAttributes="Bold"
                       TextColor="{StaticResource TextPrimary}" />
                <Label Text="Current Month Sales"
                       FontSize="11"
                       TextColor="{StaticResource TextSecondary}" />
            </VerticalStackLayout>
            
            <!-- Button -->
            <Button Text="Last Month Summary"
                    Style="{StaticResource PrimaryButton}"
                    VerticalOptions="Center" />
        </HorizontalStackLayout>
    </Grid>
</ContentView>
```

- [ ] **Step 2: Create DashboardHeaderView.xaml.cs**

```csharp
namespace MauiHighFidelityDashboard.Components;

public partial class DashboardHeaderView : ContentView
{
    public DashboardHeaderView()
    {
        InitializeComponent();
    }
}
```

---

### Task 7: Create SummaryCardView

**Files:**
- Create: `src/Components/SummaryCardView.xaml`
- Create: `src/Components/SummaryCardView.xaml.cs`

- [ ] **Step 1: Create SummaryCardView.xaml**

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MauiHighFidelityDashboard.Components.SummaryCardView">
    
    <Border Style="{StaticResource CardBorderStyle}">
        <Grid ColumnDefinitions="Auto,*" ColumnSpacing="16">
            
            <!-- Icon -->
            <Border WidthRequest="48"
                    HeightRequest="48"
                    StrokeShape="RoundRectangle 12"
                    BackgroundColor="{Binding ThemeColor}"
                    Stroke="Transparent"
                    VerticalOptions="Center">
                <Label Text="{Binding Icon}"
                       TextColor="White"
                       FontSize="20"
                       HorizontalOptions="Center"
                       VerticalOptions="Center" />
            </Border>
            
            <!-- Text -->
            <VerticalStackLayout Grid.Column="1" Spacing="4" VerticalOptions="Center">
                <Label Text="{Binding Title}"
                       FontSize="13"
                       TextColor="{StaticResource TextSecondary}" />
                <Label Text="{Binding Amount, StringFormat='{0:C}'}"
                       FontSize="22"
                       FontAttributes="Bold"
                       TextColor="{StaticResource TextPrimary}" />
            </VerticalStackLayout>
        </Grid>
    </Border>
</ContentView>
```

- [ ] **Step 2: Create SummaryCardView.xaml.cs**

```csharp
namespace MauiHighFidelityDashboard.Components;

public partial class SummaryCardView : ContentView
{
    public SummaryCardView()
    {
        InitializeComponent();
    }
}
```

---

### Task 8: Create RevenueCardView

**Files:**
- Create: `src/Components/RevenueCardView.xaml`
- Create: `src/Components/RevenueCardView.xaml.cs`

- [ ] **Step 1: Create RevenueCardView.xaml**

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MauiHighFidelityDashboard.Components.RevenueCardView">
    
    <Border Style="{StaticResource CardBorderStyle}"
            BackgroundColor="{Binding CardBackground}">
        <VerticalStackLayout Spacing="12">
            
            <!-- Title + Dropdown Row -->
            <Grid ColumnDefinitions="*,Auto">
                <Label Text="{Binding CardTitle}"
                       FontSize="15"
                       FontAttributes="Bold"
                       TextColor="{StaticResource TextPrimary}" />
                <Label Grid.Column="1"
                       Text="{Binding DropdownLabel}"
                       FontSize="11"
                       TextColor="{StaticResource TextSecondary}" />
            </Grid>
            
            <!-- Value -->
            <Label Text="{Binding CardValue}"
                   FontSize="28"
                   FontAttributes="Bold"
                   TextColor="{StaticResource TextPrimary}" />
            
            <!-- Subtitle -->
            <Label Text="{Binding CardSubtitle}"
                   FontSize="11"
                   TextColor="{StaticResource TextSecondary}" />
            
            <!-- Mini Chart Placeholder -->
            <Border HeightRequest="60"
                    BackgroundColor="{Binding ChartBackground}"
                    StrokeShape="RoundRectangle 8"
                    Stroke="Transparent" />
        </VerticalStackLayout>
    </Border>
</ContentView>
```

- [ ] **Step 2: Create RevenueCardView.xaml.cs**

```csharp
namespace MauiHighFidelityDashboard.Components;

public partial class RevenueCardView : ContentView
{
    public static readonly BindableProperty CardTitleProperty =
        BindableProperty.Create(nameof(CardTitle), typeof(string), typeof(RevenueCardView), string.Empty);
    public static readonly BindableProperty CardValueProperty =
        BindableProperty.Create(nameof(CardValue), typeof(string), typeof(RevenueCardView), string.Empty);
    public static readonly BindableProperty CardSubtitleProperty =
        BindableProperty.Create(nameof(CardSubtitle), typeof(string), typeof(RevenueCardView), string.Empty);
    public static readonly BindableProperty DropdownLabelProperty =
        BindableProperty.Create(nameof(DropdownLabel), typeof(string), typeof(RevenueCardView), string.Empty);
    public static readonly BindableProperty CardBackgroundProperty =
        BindableProperty.Create(nameof(CardBackground), typeof(Color), typeof(RevenueCardView), Colors.White);
    public static readonly BindableProperty ChartBackgroundProperty =
        BindableProperty.Create(nameof(ChartBackground), typeof(Color), typeof(RevenueCardView), Colors.LightGray);

    public string CardTitle { get => (string)GetValue(CardTitleProperty); set => SetValue(CardTitleProperty, value); }
    public string CardValue { get => (string)GetValue(CardValueProperty); set => SetValue(CardValueProperty, value); }
    public string CardSubtitle { get => (string)GetValue(CardSubtitleProperty); set => SetValue(CardSubtitleProperty, value); }
    public string DropdownLabel { get => (string)GetValue(DropdownLabelProperty); set => SetValue(DropdownLabelProperty, value); }
    public Color CardBackground { get => (Color)GetValue(CardBackgroundProperty); set => SetValue(CardBackgroundProperty, value); }
    public Color ChartBackground { get => (Color)GetValue(ChartBackgroundProperty); set => SetValue(ChartBackgroundProperty, value); }

    public RevenueCardView()
    {
        InitializeComponent();
    }
}
```

---

### Task 9: Create SalesChartView

**Files:**
- Create: `src/Components/SalesChartView.xaml`
- Create: `src/Components/SalesChartView.xaml.cs`

- [ ] **Step 1: Create SalesChartView.xaml**

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MauiHighFidelityDashboard.Components.SalesChartView">
    
    <Border Style="{StaticResource CardBorderStyle}">
        <VerticalStackLayout Spacing="16">
            
            <!-- Title Row -->
            <Grid ColumnDefinitions="*,Auto">
                <Label Text="Sales Analytics"
                       Style="{StaticResource SectionTitle}"
                       Margin="0" />
                <HorizontalStackLayout Grid.Column="1" Spacing="8">
                    <Label Text="◆ Online" TextColor="{StaticResource ChartOnline}" FontSize="12" VerticalOptions="Center" />
                    <Label Text="■ Store" TextColor="{StaticResource ChartStore}" FontSize="12" VerticalOptions="Center" />
                </HorizontalStackLayout>
            </Grid>
            
            <!-- Tabs -->
            <HorizontalStackLayout Spacing="16">
                <Label Text="DAILY" TextColor="{StaticResource PrimaryOrange}" FontSize="12" FontAttributes="Bold" />
                <Label Text="WEEKLY" TextColor="{StaticResource TextSecondary}" FontSize="12" />
                <Label Text="MONTHLY" TextColor="{StaticResource TextSecondary}" FontSize="12" />
                <Label Text="YEARLY" TextColor="{StaticResource TextSecondary}" FontSize="12" />
            </HorizontalStackLayout>
            
            <!-- Chart Area -->
            <Border HeightRequest="200"
                    BackgroundColor="{StaticResource PageBackground}"
                    StrokeShape="RoundRectangle 8"
                    Stroke="Transparent">
                <Grid Padding="16">
                    <!-- Y Axis Labels -->
                    <VerticalStackLayout VerticalOptions="Fill" JustifyContent="SpaceBetween">
                        <Label Text="30" FontSize="10" TextColor="{StaticResource TextLight}" />
                        <Label Text="20" FontSize="10" TextColor="{StaticResource TextLight}" />
                        <Label Text="10" FontSize="10" TextColor="{StaticResource TextLight}" />
                        <Label Text="0" FontSize="10" TextColor="{StaticResource TextLight}" />
                    </VerticalStackLayout>
                    
                    <!-- Sample Data Points as bars for visual reference -->
                    <HorizontalStackLayout Grid.Column="1" 
                                           HorizontalOptions="End"
                                           VerticalOptions="End"
                                           Spacing="12"
                                           HeightRequest="160">
                        <VerticalStackLayout Spacing="4" VerticalOptions="End">
                            <BoxView HeightRequest="30" WidthRequest="8" Color="{StaticResource ChartOnline}" CornerRadius="2" />
                            <BoxView HeightRequest="45" WidthRequest="8" Color="{StaticResource ChartStore}" CornerRadius="2" />
                        </VerticalStackLayout>
                        <VerticalStackLayout Spacing="4" VerticalOptions="End">
                            <BoxView HeightRequest="110" WidthRequest="8" Color="{StaticResource ChartOnline}" CornerRadius="2" />
                            <BoxView HeightRequest="100" WidthRequest="8" Color="{StaticResource ChartStore}" CornerRadius="2" />
                        </VerticalStackLayout>
                        <VerticalStackLayout Spacing="4" VerticalOptions="End">
                            <BoxView HeightRequest="35" WidthRequest="8" Color="{StaticResource ChartOnline}" CornerRadius="2" />
                            <BoxView HeightRequest="55" WidthRequest="8" Color="{StaticResource ChartStore}" CornerRadius="2" />
                        </VerticalStackLayout>
                        <VerticalStackLayout Spacing="4" VerticalOptions="End">
                            <BoxView HeightRequest="60" WidthRequest="8" Color="{StaticResource ChartOnline}" CornerRadius="2" />
                            <BoxView HeightRequest="130" WidthRequest="8" Color="{StaticResource ChartStore}" CornerRadius="2" />
                        </VerticalStackLayout>
                        <VerticalStackLayout Spacing="4" VerticalOptions="End">
                            <BoxView HeightRequest="95" WidthRequest="8" Color="{StaticResource ChartOnline}" CornerRadius="2" />
                            <BoxView HeightRequest="110" WidthRequest="8" Color="{StaticResource ChartStore}" CornerRadius="2" />
                        </VerticalStackLayout>
                        <VerticalStackLayout Spacing="4" VerticalOptions="End">
                            <BoxView HeightRequest="150" WidthRequest="8" Color="{StaticResource ChartOnline}" CornerRadius="2" />
                            <BoxView HeightRequest="145" WidthRequest="8" Color="{StaticResource ChartStore}" CornerRadius="2" />
                        </VerticalStackLayout>
                    </HorizontalStackLayout>
                </Grid>
            </Border>
        </VerticalStackLayout>
    </Border>
</ContentView>
```

- [ ] **Step 2: Create SalesChartView.xaml.cs**

```csharp
namespace MauiHighFidelityDashboard.Components;

public partial class SalesChartView : ContentView
{
    public SalesChartView()
    {
        InitializeComponent();
    }
}
```

---

### Task 10: Create TrafficChartView

**Files:**
- Create: `src/Components/TrafficChartView.xaml`
- Create: `src/Components/TrafficChartView.xaml.cs`

- [ ] **Step 1: Create TrafficChartView.xaml**

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MauiHighFidelityDashboard.Components.TrafficChartView">
    
    <Border Style="{StaticResource CardBorderStyle}">
        <VerticalStackLayout Spacing="16">
            
            <Label Text="Traffic"
                   Style="{StaticResource SectionTitle}"
                   Margin="0" />
            
            <!-- Donut Chart Placeholder -->
            <Grid HeightRequest="160" HorizontalOptions="Center">
                <Border WidthRequest="140"
                        HeightRequest="140"
                        StrokeShape="RoundRectangle 70"
                        BackgroundColor="{StaticResource PageBackground}"
                        Stroke="{StaticResource TrafficFacebook}"
                        StrokeThickness="20"
                        HorizontalOptions="Center"
                        VerticalOptions="Center" />
            </Grid>
            
            <!-- Legend -->
            <VerticalStackLayout Spacing="8">
                <HorizontalStackLayout Spacing="8">
                    <BoxView WidthRequest="12" HeightRequest="12" Color="{StaticResource TrafficFacebook}" CornerRadius="2" />
                    <Label Text="Facebook - 34%" FontSize="12" TextColor="{StaticResource TextSecondary}" />
                </HorizontalStackLayout>
                <HorizontalStackLayout Spacing="8">
                    <BoxView WidthRequest="12" HeightRequest="12" Color="{StaticResource TrafficYoutube}" CornerRadius="2" />
                    <Label Text="Youtube - 55%" FontSize="12" TextColor="{StaticResource TextSecondary}" />
                </HorizontalStackLayout>
                <HorizontalStackLayout Spacing="8">
                    <BoxView WidthRequest="12" HeightRequest="12" Color="{StaticResource TrafficDirect}" CornerRadius="2" />
                    <Label Text="Direct Search - 11%" FontSize="12" TextColor="{StaticResource TextSecondary}" />
                </HorizontalStackLayout>
            </VerticalStackLayout>
        </VerticalStackLayout>
    </Border>
</ContentView>
```

- [ ] **Step 2: Create TrafficChartView.xaml.cs**

```csharp
namespace MauiHighFidelityDashboard.Components;

public partial class TrafficChartView : ContentView
{
    public TrafficChartView()
    {
        InitializeComponent();
    }
}
```

---

### Task 11: Create ActivityTimelineView

**Files:**
- Create: `src/Components/ActivityTimelineView.xaml`
- Create: `src/Components/ActivityTimelineView.xaml.cs`

- [ ] **Step 1: Create ActivityTimelineView.xaml**

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MauiHighFidelityDashboard.Components.ActivityTimelineView">
    
    <Border Style="{StaticResource CardBorderStyle}">
        <VerticalStackLayout Spacing="16">
            
            <Label Text="Recent Activities"
                   Style="{StaticResource SectionTitle}"
                   Margin="0" />
            
            <CollectionView ItemsSource="{Binding Activities}">
                <CollectionView.ItemTemplate>
                    <DataTemplate>
                        <Grid ColumnDefinitions="20,*" ColumnSpacing="12" Padding="0,0,0,16">
                            
                            <!-- Timeline Line + Dot -->
                            <VerticalStackLayout HorizontalOptions="Center">
                                <Border WidthRequest="12"
                                        HeightRequest="12"
                                        StrokeShape="RoundRectangle 6"
                                        BackgroundColor="{Binding DotColor}"
                                        Stroke="Transparent"
                                        VerticalOptions="Start"
                                        Margin="0,4,0,0" />
                                <BoxView WidthRequest="2"
                                         HeightRequest="40"
                                         Color="{StaticResource PageBackground}"
                                         HorizontalOptions="Center" />
                            </VerticalStackLayout>
                            
                            <!-- Content -->
                            <VerticalStackLayout Grid.Column="1" Spacing="2">
                                <Label Text="{Binding Time}"
                                       FontSize="11"
                                       TextColor="{StaticResource TextSecondary}" />
                                <Label Text="{Binding Title}"
                                       FontSize="14"
                                       FontAttributes="Bold"
                                       TextColor="{StaticResource TextPrimary}" />
                                <Label Text="{Binding Description}"
                                       FontSize="12"
                                       TextColor="{StaticResource TextSecondary}"
                                       IsVisible="{Binding Description, Converter={x:Null}}" />
                            </VerticalStackLayout>
                        </Grid>
                    </DataTemplate>
                </CollectionView.ItemTemplate>
            </CollectionView>
        </VerticalStackLayout>
    </Border>
</ContentView>
```

- [ ] **Step 2: Create ActivityTimelineView.xaml.cs**

```csharp
namespace MauiHighFidelityDashboard.Components;

public partial class ActivityTimelineView : ContentView
{
    public ActivityTimelineView()
    {
        InitializeComponent();
    }
}
```

---

### Task 12: Create OrderTableView

**Files:**
- Create: `src/Components/OrderTableView.xaml`
- Create: `src/Components/OrderTableView.xaml.cs`

- [ ] **Step 1: Create OrderTableView.xaml**

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MauiHighFidelityDashboard.Components.OrderTableView">
    
    <Border Style="{StaticResource CardBorderStyle}">
        <VerticalStackLayout Spacing="16">
            
            <!-- Header Row -->
            <Grid ColumnDefinitions="*,Auto,Auto">
                <VerticalStackLayout>
                    <Label Text="Order Status"
                           Style="{StaticResource SectionTitle}"
                           Margin="0" />
                    <Label Text="Overview of Latest Month"
                           Style="{StaticResource SectionSubtitle}"
                           Margin="0" />
                </VerticalStackLayout>
                
                <!-- Action Buttons -->
                <HorizontalStackLayout Grid.Column="1" Spacing="8" VerticalOptions="Center">
                    <Button Text="+" WidthRequest="36" HeightRequest="36" CornerRadius="8" 
                            BackgroundColor="{StaticResource PageBackground}" TextColor="{StaticResource TextPrimary}" FontSize="16" />
                    <Button Text="☰" WidthRequest="36" HeightRequest="36" CornerRadius="8" 
                            BackgroundColor="{StaticResource PageBackground}" TextColor="{StaticResource TextPrimary}" FontSize="14" />
                </HorizontalStackLayout>
                
                <HorizontalStackLayout Grid.Column="2" Spacing="8" VerticalOptions="Center" Margin="8,0,0,0">
                    <Button Text="🔔" WidthRequest="36" HeightRequest="36" CornerRadius="8" 
                            BackgroundColor="{StaticResource PageBackground}" TextColor="{StaticResource TextPrimary}" FontSize="14" />
                    <Button Text="⚙" WidthRequest="36" HeightRequest="36" CornerRadius="8" 
                            BackgroundColor="{StaticResource PageBackground}" TextColor="{StaticResource TextPrimary}" FontSize="16" />
                </HorizontalStackLayout>
            </Grid>
            
            <!-- Search Box -->
            <Border BackgroundColor="{StaticResource PageBackground}"
                    StrokeShape="RoundRectangle 8"
                    Stroke="Transparent"
                    Padding="12,8">
                <Label Text="Search Invoice..."
                       TextColor="{StaticResource TextLight}"
                       FontSize="13" />
            </Border>
            
            <!-- Table Header -->
            <Grid ColumnDefinitions="80,*,80,80,80" Padding="0,8" ColumnSpacing="8">
                <Label Text="Invoice" FontSize="12" FontAttributes="Bold" TextColor="{StaticResource TextSecondary}" />
                <Label Grid.Column="1" Text="Customers" FontSize="12" FontAttributes="Bold" TextColor="{StaticResource TextSecondary}" />
                <Label Grid.Column="2" Text="From" FontSize="12" FontAttributes="Bold" TextColor="{StaticResource TextSecondary}" />
                <Label Grid.Column="3" Text="Price" FontSize="12" FontAttributes="Bold" TextColor="{StaticResource TextSecondary}" HorizontalOptions="End" />
                <Label Grid.Column="4" Text="Status" FontSize="12" FontAttributes="Bold" TextColor="{StaticResource TextSecondary}" HorizontalOptions="Center" />
            </Grid>
            
            <!-- Table Rows -->
            <CollectionView ItemsSource="{Binding Orders}">
                <CollectionView.ItemTemplate>
                    <DataTemplate>
                        <Grid ColumnDefinitions="80,*,80,80,80"
                              ColumnSpacing="8"
                              Padding="0,10"
                              HeightRequest="44">
                            
                            <Label Text="{Binding Invoice}" FontSize="13" TextColor="{StaticResource TextPrimary}" />
                            <Label Grid.Column="1" Text="{Binding Customer}" FontSize="13" TextColor="{StaticResource TextPrimary}" />
                            <Label Grid.Column="2" Text="{Binding Country}" FontSize="13" TextColor="{StaticResource TextSecondary}" />
                            <Label Grid.Column="3" Text="{Binding Price, StringFormat='${0:N0}'}" FontSize="13" TextColor="{StaticResource TextPrimary}" HorizontalOptions="End" />
                            
                            <!-- Status Badge -->
                            <Border Grid.Column="4"
                                    HorizontalOptions="Center"
                                    Padding="8,4"
                                    StrokeShape="RoundRectangle 4"
                                    Stroke="Transparent">
                                <Border.Triggers>
                                    <DataTrigger TargetType="Border" Binding="{Binding Status}" Value="Process">
                                        <Setter Property="BackgroundColor" Value="#FFF0F0" />
                                    </DataTrigger>
                                    <DataTrigger TargetType="Border" Binding="{Binding Status}" Value="Open">
                                        <Setter Property="BackgroundColor" Value="#F0FFF0" />
                                    </DataTrigger>
                                    <DataTrigger TargetType="Border" Binding="{Binding Status}" Value="On Hold">
                                        <Setter Property="BackgroundColor" Value="#F0F5FF" />
                                    </DataTrigger>
                                </Border.Triggers>
                                <Label Text="{Binding Status}" FontSize="11">
                                    <Label.Triggers>
                                        <DataTrigger TargetType="Label" Binding="{Binding Status}" Value="Process">
                                            <Setter Property="TextColor" Value="{StaticResource StatusProcess}" />
                                        </DataTrigger>
                                        <DataTrigger TargetType="Label" Binding="{Binding Status}" Value="Open">
                                            <Setter Property="TextColor" Value="{StaticResource StatusOpen}" />
                                        </DataTrigger>
                                        <DataTrigger TargetType="Label" Binding="{Binding Status}" Value="On Hold">
                                            <Setter Property="TextColor" Value="{StaticResource StatusOnHold}" />
                                        </DataTrigger>
                                    </Label.Triggers>
                                </Label>
                            </Border>
                        </Grid>
                    </DataTemplate>
                </CollectionView.ItemTemplate>
            </CollectionView>
        </VerticalStackLayout>
    </Border>
</ContentView>
```

- [ ] **Step 2: Create OrderTableView.xaml.cs**

```csharp
namespace MauiHighFidelityDashboard.Components;

public partial class OrderTableView : ContentView
{
    public OrderTableView()
    {
        InitializeComponent();
    }
}
```

---

### Task 13: Create MainPage.xaml

**Files:**
- Create: `src/Views/MainPage.xaml`
- Create: `src/Views/MainPage.xaml.cs`

- [ ] **Step 1: Create MainPage.xaml**

Main layout with Grid containing Sidebar + Main Content with all sections.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:components="clr-namespace:MauiHighFidelityDashboard.Components"
             xmlns:vm="clr-namespace:MauiHighFidelityDashboard.ViewModels"
             x:Class="MauiHighFidelityDashboard.Views.MainPage"
             x:DataType="vm:MainViewModel"
             BackgroundColor="{StaticResource PageBackground}"
             Shell.NavBarIsVisible="False"
             Title="Dashboard">
    
    <Grid ColumnDefinitions="220,*">
        
        <!-- Sidebar -->
        <components:SidebarView />
        
        <!-- Main Content -->
        <ScrollView Grid.Column="1">
            <VerticalStackLayout Spacing="0">
                
                <!-- Header -->
                <components:DashboardHeaderView />
                
                <!-- Sales Chart + Traffic Chart Row -->
                <Grid ColumnDefinitions="*,*" Padding="24,0" ColumnSpacing="12">
                    <components:SalesChartView />
                    <components:TrafficChartView Grid.Column="1" />
                </Grid>
                
                <!-- Summary Cards Row -->
                <Grid ColumnDefinitions="*,*,*,*" Padding="24,12" ColumnSpacing="12">
                    <components:SummaryCardView BindingContext="{Binding DashboardCards[0]}" />
                    <components:SummaryCardView Grid.Column="1" BindingContext="{Binding DashboardCards[1]}" />
                    <components:SummaryCardView Grid.Column="2" BindingContext="{Binding DashboardCards[2]}" />
                    <components:SummaryCardView Grid.Column="3" BindingContext="{Binding DashboardCards[3]}" />
                </Grid>
                
                <!-- Revenue Cards Row -->
                <Grid ColumnDefinitions="*,*,*,*" Padding="24,0" ColumnSpacing="12">
                    <components:RevenueCardView 
                        CardTitle="Revenue Status"
                        CardValue="$432"
                        CardSubtitle="Jan 01 - Jan 10"
                        CardBackground="#E3F2FD"
                        ChartBackground="#BBDEFB" />
                    <components:RevenueCardView 
                        Grid.Column="1"
                        CardTitle="Page View"
                        CardValue="$432"
                        CardSubtitle=""
                        CardBackground="#FFF8E1"
                        ChartBackground="#FFE082" />
                    <components:RevenueCardView 
                        Grid.Column="2"
                        CardTitle="Bounce Rate"
                        CardValue="$432"
                        CardSubtitle="Monthly"
                        CardBackground="#FFF3E0"
                        ChartBackground="#FFCC80" />
                    <components:RevenueCardView 
                        Grid.Column="3"
                        CardTitle="Revenue Status"
                        CardValue="$432"
                        CardSubtitle=""
                        CardBackground="#F3E5F5"
                        ChartBackground="#CE93D8" />
                </Grid>
                
                <!-- Activity + Orders Row -->
                <Grid ColumnDefinitions="*,2*" Padding="24,12,24,24" ColumnSpacing="12">
                    <components:ActivityTimelineView />
                    <components:OrderTableView Grid.Column="1" />
                </Grid>
            </VerticalStackLayout>
        </ScrollView>
    </Grid>
</ContentPage>
```

- [ ] **Step 2: Create MainPage.xaml.cs**

```csharp
using MauiHighFidelityDashboard.ViewModels;

namespace MauiHighFidelityDashboard.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
```

---

### Task 14: Clean up default files and verify build

**Files:**
- Modify: Remove default MainPage.xaml, MainViewModel from root, etc.

- [ ] **Step 1: Remove default generated files**

Remove MainPage.xaml, MainPage.xaml.cs that dotnet new creates in the root.

- [ ] **Step 2: Build the project**

```bash
dotnet build -f net8.0-windows10.0.19041.0
```
