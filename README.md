# MauiHighFidelityDashboard

A production-grade **.NET MAUI** dashboard application demonstrating **Clean Architecture**, **MVVM**, **Dependency Injection**, and enterprise design patterns. Built for interview discussion and real-world extensibility.

---

## Architecture

```
src/
├── Domain/                        # Core business objects & contracts
│   ├── Common/                    # Result<T> — consistent error handling
│   ├── Models/                    # DashboardCard, ActivityModel, TrafficModel, ...
│   └── Interfaces/                # IDashboardDataService — async data contract
├── Infrastructure/                # Data access implementations
│   └── Data/                      # StaticDashboardDataService, ApiDashboardDataService
├── Core/                          # Application logic
│   ├── ViewModels/                # BaseViewModel, MainViewModel
│   └── Converters/                # StatusColor, StatusBackground, AmountToCurrency
└── Presentation/                  # UI layer
    ├── Views/                     # MainPage
    ├── Components/                # 8 reusable ContentViews
    └── Resources/Styles/          # Colors.xaml, Styles.xaml, Fonts.xaml
```

**Dependency flow:** `Domain` ← `Infrastructure` ← `Core` ← `Presentation`

---

## Tech Stack

| Technology | Purpose |
|-----------|---------|
| .NET 10 / MAUI | Cross-platform UI framework |
| CommunityToolkit.Mvvm | Source-generated MVVM (ObservableProperty, RelayCommand) |
| CommunityToolkit.Maui | UI helpers & behaviors |
| Clean Architecture | Domain / Infrastructure / Core / Presentation layers |
| Dependency Injection | Built-in Microsoft DI container |

---

## Key Design Decisions

### 1. Clean Architecture Layers
Each layer has a single responsibility. `Domain` knows nothing about UI or data access. `Presentation` only depends on `Core` abstractions. Swapping the data source (static → API) requires zero UI changes.

### 2. IDashboardDataService Interface
All data access flows through this async contract. Two implementations ship with the project:
- **StaticDashboardDataService** — in-memory sample data (current default)
- **ApiDashboardDataService** — HttpClient-based, ready for backend integration

Switch between them in `MauiProgram.cs` by toggling one line.

### 3. Result\<T\> Pattern
Every service method returns `Result<T>` — a discriminated union of `Success(data)` or `Failure(error)`. ViewModels check `result.IsSuccess` before using data, making error paths explicit without exceptions.

### 4. Async Everything
Data loading uses `Task.WhenAll` for parallel fetch, `[RelayCommand]` for bindable async commands, and `IsBusy` guard to prevent duplicate loads.

### 5. Reusable Components
All 8 dashboard sections are self-contained `ContentView` controls with bindable properties, registered as global styles in `ResourceDictionary`.

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (or .NET 8+ with MAUI workload)
- MAUI workload: `dotnet workload install maui`
- Windows 10 19041+ (for Windows target)

---

## How to Run

### Command Line
```bash
dotnet build -f net10.0-windows10.0.19041.0
dotnet run -f net10.0-windows10.0.19041.0
```

### Visual Studio
Open `MauiHighFidelityDashboard.sln` and press **F5**.

### VS Code
Install the **C# Dev Kit** extension, then press **F5** (uses `.vscode/launch.json`).

---

## Project Structure

```
MauiHighFidelityDashboard.sln
MauiHighFidelityDashboard.csproj
MauiProgram.cs                      # DI container setup
App.xaml / App.xaml.cs               # Application entry, merged resource dictionaries
AppShell.xaml                        # Shell navigation (hidden nav bar)

src/
├── Domain/
│   ├── Common/Result.cs             # Success/Failure result type
│   ├── Models/                      # 5 domain models (init-only properties)
│   └── Interfaces/                  # IDashboardDataService contract
├── Infrastructure/
│   └── Data/                        # Static + API data service implementations
├── Core/
│   ├── ViewModels/                  # BaseViewModel, MainViewModel
│   └── Converters/                  # 3 IValueConverters for XAML
└── Presentation/
    ├── Views/MainPage.xaml          # Full dashboard layout
    ├── Components/                  # 8 ContentViews:
    │   ├── SidebarView              # 220px sidebar navigation
    │   ├── DashboardHeaderView      # Title, metrics, action button
    │   ├── SalesChartView           # Bar chart + tabs
    │   ├── TrafficChartView         # Donut chart with legend
    │   ├── SummaryCardView          # Reusable KPI card (4 instances)
    │   ├── RevenueCardView          # Reusable analytics card (4 instances)
    │   ├── ActivityTimelineView     # Vertical timeline
    │   └── OrderTableView           # Table with status badges
    └── Resources/Styles/            # Colors.xaml, Styles.xaml, Fonts.xaml
```

---

## Dashboard Sections

| Section | Component | Description |
|---------|-----------|-------------|
| Sidebar | `SidebarView` | 220px navigation, #071A52 background, 21 menu items |
| Header | `DashboardHeaderView` | Title + $3,468.96 earnings + 82 sales + action button |
| Sales Chart | `SalesChartView` | Bar chart with Daily/Weekly/Monthly/Yearly tabs |
| Traffic | `TrafficChartView` | Donut chart: Facebook 34%, YouTube 55%, Direct 11% |
| KPI Cards | `SummaryCardView` × 4 | Wallet Balance, Referral, Sales, Earning |
| Analytics | `RevenueCardView` × 4 | Revenue Status, Page View, Bounce Rate, Revenue |
| Timeline | `ActivityTimelineView` | 4 recent activity entries with colored dots |
| Orders | `OrderTableView` | 5 orders with search, filter, status badges |

---

## Interview Topics

- **Why Clean Architecture?** Independent layers, testable in isolation, swap data sources without touching UI
- **Why Service Interface?** `IDashboardDataService` enables unit testing with mocks, parallel async loading
- **Why Result\<T\>?** Explicit error handling vs exceptions — predictable, composable, testable
- **Why CommunityToolkit.Mvvm?** Source generators eliminate boilerplate (`[ObservableProperty]`, `[RelayCommand]`)
- **Why ContentViews?** Encapsulated, reusable UI components with bindable properties
- **Why ResourceDictionary?** Centralized styling, consistent theming, maintainable

---

## Switching to API Mode

In `MauiProgram.cs`, comment out the static service and uncomment the API service:

```csharp
// builder.Services.AddSingleton<IDashboardDataService, StaticDashboardDataService>();
builder.Services.AddSingleton<IDashboardDataService>(sp =>
{
    var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };
    return new ApiDashboardDataService(client);
});
```

The `ApiDashboardDataService` expects the following endpoints:
- `GET /api/dashboard/cards`
- `GET /api/dashboard/activities`
- `GET /api/dashboard/orders`
- `GET /api/dashboard/traffic`
- `GET /api/dashboard/sales`

---

## Built With

- .NET MAUI
- CommunityToolkit.Mvvm
- CommunityToolkit.Maui
- Clean Architecture
- MVVM Pattern
