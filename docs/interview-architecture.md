# MauiHighFidelityDashboard — Interview Architecture Guide

## Project Overview

A .NET MAUI dashboard application demonstrating **Clean Architecture**, **MVVM**, **Dependency Injection**, and **Result pattern**. Built for Windows (also targets Android/iOS/macCatalyst).

---

## 1. Architecture Layers

```
MauiHighFidelityDashboard/
├── src/
│   ├── Domain/              ← Business models + contracts
│   │   ├── Common/          ← Result<T> — error handling pattern
│   │   ├── Interfaces/      ← IDashboardDataService contract
│   │   └── Models/          ← DashboardCard, OrderModel, ActivityModel, etc.
│   ├── Infrastructure/      ← Data access implementations (flat)
│   │   ├── StaticDashboardDataService.cs
│   │   └── ApiDashboardDataService.cs
│   ├── Core/                ← ViewModels + Converters (flat)
│   │   ├── BaseViewModel.cs
│   │   ├── MainViewModel.cs
│   │   ├── DetailViewModel.cs
│   │   ├── *Converter.cs    ← 4 converters
│   ├── Presentation/         ← UI layer
│   │   ├── Views/           ← MainPage, DetailPage
│   │   ├── Components/      ← 8 reusable ContentViews
│   │   └── Styles/          ← Colors, Styles, Fonts
├── MauiProgram.cs           ← DI container setup
├── App.xaml(.cs)            ← Application entry
└── AppShell.xaml(.cs)       ← Shell navigation
```

**Dependency rule:** `Domain` ← `Infrastructure` ← `Core` ← `Presentation`

Each layer only depends on the layer(s) before it. Domain knows nothing about UI or data access. You can swap the data service without touching a single ViewModel.

---

## 2. Data Flow

```
User taps refresh
        │
        ▼
MainPage (XAML binding)
        │
        ▼
MainViewModel.InitializeAsync()
        │
        ▼
Task.WhenAll (
    LoadDashboardCardsAsync(),
    LoadActivitiesAsync(),
    LoadOrdersAsync(),
    LoadTrafficSourcesAsync(),
    LoadSalesDataAsync()
)
        │
        ▼
IDashboardDataService (interface)
        │                        ╱ StaticDashboardDataService (in-memory)
        ├── Get*Async() ────────╲ ApiDashboardDataService (HTTP)
        │
        ▼
Result<T>.Success(data)  OR  Result<T>.Failure(error)
        │
        ▼
ObservableCollection<T> updated → XAML auto-updates via binding
```

### Key points:
- All 5 data sources load in **parallel** via `Task.WhenAll`
- Each checks `result.IsSuccess` before using data — no exceptions for expected failures
- ViewModel properties use `ObservableCollection` and `[ObservableProperty]` so UI updates automatically

---

## 3. MVVM Pattern Explained

```
Model           → Domain models (DashboardCard, OrderModel, etc.)
ViewModel       → MainViewModel, DetailViewModel (extends BaseViewModel)
View            → XAML pages + components
```

### BaseViewModel
```csharp
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] private bool _isBusy;    // Guards against double-load
    [ObservableProperty] private string _title;    // Page title
}
```

### Why CommunityToolkit.Mvvm?
- `[ObservableProperty]` — generates `INotifyPropertyChanged` boilerplate
- `[RelayCommand]` — generates `ICommand` from methods (e.g., `LoadDataCommand` from `LoadDataAsync()`)
- Zero runtime reflection — all source-generated at compile time

---

## 4. Key Design Decisions (Interview Talking Points)

### 4.1 Why Clean Architecture?
| Benefit | Explanation |
|---------|------------|
| **Testability** | ViewModels depend on `IDashboardDataService` interface → mock in unit tests |
| **Swapability** | Switch from static data to API by changing one line in `MauiProgram.cs` |
| **Separation of concerns** | UI never talks to data layer directly |
| **Interview value** | Shows you understand enterprise patterns |

### 4.2 Why Result<T> instead of exceptions?
```csharp
public class Result<T>
{
    public T? Data { get; }
    public string? ErrorMessage { get; }
    public bool IsSuccess => ErrorMessage is null;
}
```
- **Predictable** — caller *must* check `IsSuccess` (no unhandled exceptions)
- **Composable** — chain operations without try/catch everywhere
- **Explicit** — method signature tells you it can fail

### 4.3 Why IDashboardDataService?
- Interface-based design lets you **swap implementations** without changing ViewModels
- Two implementations ship: `StaticDashboardDataService` (in-memory) and `ApiDashboardDataService` (HTTP)
- Switch in `MauiProgram.cs`:
  ```csharp
  builder.Services.AddSingleton<IDashboardDataService, StaticDashboardDataService>();
  // → change to:
  // builder.Services.AddSingleton<IDashboardDataService, ApiDashboardDataService>();
  ```

### 4.4 Why ContentViews instead of inline XAML?
- Each dashboard section (SalesChart, OrderTable, Sidebar, etc.) is a **self-contained component**
- Has its own ViewModel bindings via `x:DataType` for compile-time binding validation
- Registered globally in `ResourceDictionary` — reusable across pages

### 4.5 Dependency Injection (DI)
- All services and ViewModels registered in `MauiProgram.cs`
- `MainViewModel` receives `IDashboardDataService` through constructor injection
- `MainPage` receives `MainViewModel` through constructor injection
- No `new ViewModel()` anywhere — DI container manages lifetimes

---

## 5. Component Map

| Component | Type | What it shows |
|-----------|------|---------------|
| SidebarView | Navigation | 21 menu items grouped by category |
| DashboardHeaderView | Header | Title, earnings ($3,468.96), sales count |
| SalesChartView | Chart | Spline chart with Daily/Weekly/Monthly tabs |
| TrafficChartView | Chart | Donut chart (Facebook 34%, YouTube 55%, Direct 11%) |
| SummaryCardView | KPI | 4 cards: Wallet, Referral, Sales, Earning |
| RevenueCardView | Analytics | 4 cards: Revenue Status, Page View, Bounce Rate, Revenue |
| ActivityTimelineView | Timeline | 5 recent activity items with colored dots |
| OrderTableView | Table | 30 orders with search, pagination, add/delete |

---

## 6. Common Interview Questions & Answers

### Q: "Why did you choose MAUI over other frameworks?"
> MAUI provides a single project that targets Windows, Android, iOS, and macOS with shared C# code and XAML. For enterprise dashboard apps, this means one team maintains one codebase. The `TargetFramework` conditions let us optimize per-platform behavior (e.g., maximize on Windows launch) while keeping the core identical.

### Q: "Explain your dependency injection setup."
> All registrations are in `MauiProgram.cs`. `IDashboardDataService` is registered as singleton (one data source instance). `MainViewModel` and `MainPage` are singletons (one dashboard session). `DetailViewModel` and `DetailPage` are transient (new instance per navigation). Each ViewModel receives its dependencies via constructor — no service locator pattern.

### Q: "How would you add a new dashboard card?"
> 1. Add a model class in `Domain/Models/`
> 2. Add a method to `IDashboardDataService` interface
> 3. Implement it in both `StaticDashboardDataService` and `ApiDashboardDataService`
> 4. Add load method in `MainViewModel`
> 5. Create a `ContentView` component in `Presentation/Components/`
> 6. Register in `MainPage.xaml` and bind to the ViewModel property

### Q: "How does the Result<T> pattern help vs exceptions?"
> Exceptions are for exceptional situations — database down, network timeout. `Result<T>` is for expected failures — empty data, validation errors. It makes the failure path explicit in the type system. The ViewModel checks `result.IsSuccess` and decides how to handle each case without try/catch noise.

### Q: "What's the data flow when the app starts?"
> 1. `MauiProgram.cs` builds the DI container
> 2. `App.xaml.cs` creates `App` → creates `AppShell`
> 3. `AppShell` routes to `MainPage`
> 4. `MainPage` receives `MainViewModel` via DI
> 5. In constructor, `MainViewModel` sets title
> 6. `MainPage.OnAppearing()` calls `MainViewModel.InitializeAsync()`
> 7. `InitializeAsync()` fires `LoadDataCommand` which calls all 5 service methods in parallel
> 8. Each service returns `Result<T>` → ViewModel populates `ObservableCollection`
> 9. XAML bindings auto-update the UI

### Q: "How do you handle errors gracefully?"
> The `Result<T>` pattern means errors don't crash the app. If a service call fails, the ViewModel simply skips that section — other sections still render. The user can hit Refresh to retry. The `IsBusy` guard prevents duplicate loads. The `ApiDashboardDataService` catches HTTP exceptions and returns `Result.Failure` with a descriptive message.

### Q: "How are converters organized?"
> All 4 converters live in `Core/` — the layer that bridges data and presentation:
> - `StatusColorConverter` — order status → foreground color
> - `StatusBackgroundConverter` — order status → background color  
> - `AmountToCurrencyConverter` — decimal → currency string
> - `HexToColorConverter` — hex string → Color object

### Q: "How would you make this app responsive?"
> Currently uses fixed sidebar width (200px) and grid columns. For responsiveness:
> - Use `OnIdiom` for phone/tablet/desktop layouts
> - Replace `ScrollView` with `FlexLayout` or adaptive `Grid`
> - Collapse sidebar to a hamburger menu on small screens
> - Use `DeviceInfo.Idiom` in ViewModel to switch layouts
> - Media queries aren't standard in MAUI, but `OnSizeAllocated` override can trigger layout switches

---

## 7. Running the App

```bash
cd MauiHighFidelityDashboard
dotnet run
```

> Builds and launches the dashboard (Windows target). The window opens maximized. First load fetches all data in parallel — you'll see cards, charts, timeline, and order table populate.

---

## 8. Build & Tech Stack

| Technology | Purpose |
|-----------|---------|
| .NET 10 / MAUI | Cross-platform UI |
| CommunityToolkit.Mvvm 8.4 | Source-generated MVVM |
| CommunityToolkit.Maui 9.1 | UI helpers |
| Clean Architecture | 4-layer separation |
| Result\<T\> Pattern | Explicit error handling |
