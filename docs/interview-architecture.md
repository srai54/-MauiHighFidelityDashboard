# MauiHighFidelityDashboard — Interview Architecture Guide

## Project Overview

A .NET MAUI dashboard application following the assignment-mandated **MVVM folder structure** with **Dependency Injection**, the **Result pattern**, service interfaces, real cross-platform **printing**, and a **responsive layout**. Built for Windows and Android (also targets iOS/macCatalyst).

---

## 1. Project Structure (per assignment PDF)

```
MauiHighFidelityDashboard/
├── Views/                       ← Pages
│   ├── MainPage.xaml            ← Dashboard (responsive layout logic in code-behind)
│   ├── DetailPage.xaml          ← Sidebar-navigation detail page
│   ├── PrintPreviewPage.xaml    ← Print preview + OS print dialog
│   └── LastMonthSummaryPopup.xaml ← Structured stats popup (CommunityToolkit Popup)
├── Components/                  ← 8 reusable ContentViews
│   ├── SidebarView, DashboardHeaderView, SalesChartView, TrafficChartView,
│   ├── SummaryCardView, RevenueCardView, ActivityTimelineView, OrderTableView
├── ViewModels/
│   ├── BaseViewModel.cs         ← IsBusy, Title (ObservableObject)
│   ├── MainViewModel.cs         ← Dashboard state + commands
│   └── DetailViewModel.cs       ← Detail page catalog
├── Models/                      ← POCOs + Result<T> + SummaryStat
├── Services/
│   ├── Interfaces/              ← IDashboardDataService, IPrintService
│   ├── StaticDashboardDataService.cs  (in-memory, default)
│   ├── ApiDashboardDataService.cs     (HttpClient, ready for backend)
│   └── PrintService.cs          ← HTML report + platform print flow
├── Converters/                  ← 4 IValueConverters
├── Resources/Styles/            ← Colors.xaml, Styles.xaml, Fonts.xaml
├── MauiProgram.cs               ← DI container setup
├── App.xaml(.cs)                ← Application entry, merged dictionaries
├── AppShell.xaml(.cs)           ← Shell navigation
└── run.cmd                      ← Platform picker: Windows or Android
```

**Dependency rule:** Views bind to ViewModels; ViewModels depend on service *interfaces*; services depend on Models. Swapping the data source touches one line in `MauiProgram.cs` — no ViewModel changes.

---

## 2. Data Flow

```
App start / user taps refresh
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

## 3. Print Flow (real printing, both platforms)

```
🖨 button → PrintOrdersCommand (MainViewModel)
        │
        ▼
IPrintService.PrintOrdersAsync(filteredOrders, jobName)
        │
        ▼
PrintService builds a styled HTML report (status badges, totals, timestamp)
        │
        ▼
PrintPreviewPage (modal) renders it in a WebView
        │
        ▼  [Print button]
Windows: WebView2.CoreWebView2.ShowPrintUI  → OS dialog (printers + Print to PDF)
Android: PrintManager + WebView.CreatePrintDocumentAdapter → system print sheet
```

The preview's Print button is disabled until the WebView's `Navigated` event fires, so the document is never printed half-loaded.

---

## 4. MVVM Pattern Explained

```
Model           → Models/ (DashboardCard, OrderModel, SummaryStat, …)
ViewModel       → MainViewModel, DetailViewModel (extend BaseViewModel)
View            → Views/ pages + Components/ ContentViews
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

### Where code-behind IS used (and why that's OK)
The assignment says *avoid code-behind where possible*. The remaining code-behind is **visual/layout logic**, not business logic:
- `MainPage.xaml.cs` — responsive breakpoint (rearranges Grid rows/columns on `SizeChanged`)
- Chart components — `IDrawable` drawing code for GraphicsView
- `PrintPreviewPage.xaml.cs` — platform print API calls (`#if WINDOWS / ANDROID`)

Business state and actions (orders, search, pagination, print trigger, popup trigger) all live in ViewModels.

---

## 5. Key Design Decisions (Interview Talking Points)

### 5.1 Why service interfaces?
| Benefit | Explanation |
|---------|------------|
| **Testability** | ViewModels depend on `IDashboardDataService` / `IPrintService` → mock in unit tests |
| **Swapability** | Switch static data → API by changing one line in `MauiProgram.cs` |
| **Separation of concerns** | UI never talks to the data layer directly |

### 5.2 Why Result<T> instead of exceptions?
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

### 5.3 Why vector Path arrows on the revenue cards?
The design shows a **bold** up arrow next to $432. A text `↑` with `FontAttributes="Bold"` does nothing — the glyph comes from a system fallback font that ignores the bold attribute. A stroked `Path` (thickness 3.2, round caps) is guaranteed bold and identical on every platform, and its `Stroke` is set from the card's accent color.

### 5.4 Why ContentViews instead of inline XAML?
- Each dashboard section is a **self-contained component** in `/Components`
- `x:DataType` gives compile-time binding validation
- `SummaryCardView` and `RevenueCardView` are instantiated 4× each with different data

### 5.5 Dependency Injection (DI)
- All services and ViewModels registered in `MauiProgram.cs`
- `MainViewModel` receives `IDashboardDataService` + `IPrintService` via constructor
- `MainPage` receives `MainViewModel` via constructor
- No `new ViewModel()` anywhere — DI container manages lifetimes

---

## 6. Responsive Layout (implemented)

`MainPage` applies a **980px logical-width breakpoint** in `ApplyResponsiveLayout()` (code-behind, triggered by `SizeChanged`):

| Region | Wide (≥ 980px) | Narrow (< 980px — phone, small window, high zoom) |
|--------|----------------|---------------------------------------------------|
| Sidebar | Fixed 200px column | Hidden; ☰ hamburger toggles a 200px overlay |
| Main card + Traffic | 2.55* / 1* columns | Stacked vertically |
| Header + spline chart | 205px / * columns | Header stacks above chart |
| KPI stat strip | 4 cards + dividers in a row | 2 × 2 grid, dividers hidden |
| Revenue cards | 4 in a row (FlexLayout) | Wrap 2- or 1-per-row (`Basis=240`, `Grow=1`) |
| Activities + Orders | 2* / 3* columns | Stacked vertically |

Because Windows display zoom shrinks the **logical** width, the same breakpoint handles 100%→400% zoom automatically — the layout restacks instead of clipping.

---

## 7. Component Map

| Component | Type | What it shows |
|-----------|------|---------------|
| SidebarView | Navigation | 21 menu items, dark theme |
| DashboardHeaderView | Header | Title, earnings ($3,468.96), sales count, summary button |
| SalesChartView | Chart | Spline chart, full grid (horizontal + vertical lines), Daily/Weekly/Monthly/Yearly tabs |
| TrafficChartView | Chart | Donut chart (Facebook 34%, YouTube 55%, Direct 11%) |
| SummaryCardView | KPI | 4 cards: Wallet, Referral, Sales, Earning |
| RevenueCardView | Analytics | 4 cards with bold vector arrows: Revenue Status, Page View, Bounce Rate, Revenue |
| ActivityTimelineView | Timeline | 5 recent activity items with colored dots |
| OrderTableView | Table | 30 orders with search, pagination, add/delete/info/print |
| PrintPreviewPage | Page | HTML report preview + real OS print dialog |
| LastMonthSummaryPopup | Popup | Structured label/value stat rows, highlighted growth |

---

## 8. Running the App

```bash
# Interactive platform picker (Windows / Android):
.\run.cmd

# Or explicitly:
dotnet run -f net10.0-windows10.0.19041.0        # Windows
dotnet build -t:Run -f net10.0-android            # Android (device/emulator must be running)
```

> Android tip: if the emulator shows a black/stuck screen, launch it with software rendering:
> `emulator -avd pixel_7_-_api_36_0 -gpu swiftshader_indirect` (run.cmd does this automatically).

---

## 9. Build & Tech Stack

| Technology | Purpose |
|-----------|---------|
| .NET 10 / MAUI | Cross-platform UI |
| CommunityToolkit.Mvvm 8.4 | Source-generated MVVM |
| CommunityToolkit.Maui 9.1 | Popup, UI helpers |
| Service interfaces + DI | Swappable data/print services |
| Result\<T\> Pattern | Explicit error handling |
| GraphicsView + IDrawable | Custom charts, no third-party chart library |
