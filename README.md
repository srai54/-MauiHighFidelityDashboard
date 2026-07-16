# MauiHighFidelityDashboard

A **.NET MAUI** dashboard that recreates the assignment screenshot with high visual fidelity, following the assignment's **MVVM folder structure**, with **Dependency Injection**, service interfaces, real cross-platform **printing**, and a **responsive layout** that adapts from desktop to phone (and 100%→400% zoom).

Runs on **Windows** and **Android** (iOS/macCatalyst targets included).

---

## Project Structure (per assignment)

```
MauiHighFidelityDashboard/
├── Views/                          # Pages
│   ├── MainPage.xaml               # Dashboard + responsive breakpoint logic
│   ├── DetailPage.xaml             # Sidebar navigation detail page
│   ├── PrintPreviewPage.xaml       # Report preview + OS print dialog
│   └── LastMonthSummaryPopup.xaml  # Structured stats popup
├── Components/                     # 8 reusable ContentViews
│   ├── SidebarView.xaml            # 200px dark navigation (hamburger overlay on phones)
│   ├── DashboardHeaderView.xaml    # Title, earnings, sales, summary button
│   ├── SalesChartView.xaml         # Spline chart + grid + period tabs (GraphicsView)
│   ├── TrafficChartView.xaml       # Donut chart with legend
│   ├── SummaryCardView.xaml        # KPI card (used 4x)
│   ├── RevenueCardView.xaml        # Analytics card with bold vector arrow (used 4x)
│   ├── ActivityTimelineView.xaml   # Vertical activity timeline
│   └── OrderTableView.xaml         # Orders: search, pagination, add/delete/print
├── ViewModels/
│   ├── BaseViewModel.cs            # ObservableObject base (IsBusy, Title)
│   ├── MainViewModel.cs            # Dashboard state + commands
│   └── DetailViewModel.cs
├── Models/                         # POCO models + Result<T> + SummaryStat
├── Services/
│   ├── Interfaces/                 # IDashboardDataService, IPrintService
│   ├── StaticDashboardDataService.cs   # In-memory data (default)
│   ├── ApiDashboardDataService.cs      # HttpClient implementation (swap-in ready)
│   └── PrintService.cs             # HTML report + print preview flow
├── Converters/                     # 4 IValueConverters
├── Resources/Styles/               # Colors.xaml, Styles.xaml, Fonts.xaml
├── MauiProgram.cs                  # DI registrations
└── run.cmd                         # Interactive launcher: Windows or Android
```

**MVVM:** Views bind to ViewModels (CommunityToolkit.Mvvm source generators); ViewModels depend on service **interfaces**; code-behind is reserved for visual concerns (chart drawing, responsive re-layout, platform print calls).

---

## How to Run

### Interactive picker (Windows or Android)
```bat
.\run.cmd
```
Choose `1` for Windows or `2` for Android. For Android it auto-starts the Pixel 7 emulator (software rendering) if no device is connected, waits for boot, then deploys.

### Direct commands
```bash
dotnet run -f net10.0-windows10.0.19041.0        # Windows
dotnet build -t:Run -f net10.0-android            # Android (device/emulator required)
```

### Visual Studio
Open the solution, pick the **framework** (`net10.0-windows…` or `net10.0-android`) and the device from the debug-target dropdown, press **F5**.

> **Emulator tip:** if the Android emulator boots to a black/stuck screen on this machine, start it with software rendering:
> `emulator -avd pixel_7_-_api_36_0 -gpu swiftshader_indirect` — `run.cmd` already does this. On first boot the emulator is slow; if an "isn't responding" dialog appears, choose **Wait**.

---

## Feature Highlights

| Feature | Where | Notes |
|---------|-------|-------|
| High-fidelity dashboard | `Views/MainPage.xaml` + `Components/` | Pixel-matched to the assignment screenshot |
| Custom charts | `SalesChartView`, `TrafficChartView`, `RevenueCardView` | `GraphicsView` + `IDrawable`, no chart library; full grid behind splines |
| Bold trend arrows | `RevenueCardView` | Stroked vector `Path` (text `↑` ignores bold — fallback-font glyph) |
| Real printing | `Services/PrintService`, `Views/PrintPreviewPage` | Windows: WebView2 print dialog (printers + PDF). Android: system `PrintManager` |
| Structured summary popup | `Views/LastMonthSummaryPopup` | CommunityToolkit Popup, label/value rows, highlighted growth |
| Responsive layout | `MainPage.xaml.cs` (`ApplyResponsiveLayout`) | <980px: sidebar → hamburger overlay, single-column stacking, 2×2 KPI grid, revenue cards wrap 4/2/1, order table pans horizontally |
| Orders toolbar | `OrderTableView` + `MainViewModel` | Search, pagination, add (manual/quick), delete, info summary, print |

---

## Architecture Notes

- **DI:** everything is registered in `MauiProgram.cs`; ViewModels/pages get dependencies via constructor.
- **`Result<T>`:** every service call returns `Success(data)` or `Failure(error)` — explicit error handling, no exception-driven flow.
- **Parallel loading:** `MainViewModel.InitializeAsync` fetches all five data sets with `Task.WhenAll`.
- **Swappable data source:** switch `StaticDashboardDataService` → `ApiDashboardDataService` with one line in `MauiProgram.cs`:

```csharp
// builder.Services.AddSingleton<IDashboardDataService, StaticDashboardDataService>();
builder.Services.AddSingleton<IDashboardDataService>(sp =>
{
    var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };
    return new ApiDashboardDataService(client);
});
```

Expected endpoints: `GET /api/dashboard/{cards|activities|orders|traffic|sales}`.

---

## Docs

- [docs/interview-architecture.md](docs/interview-architecture.md) — architecture walkthrough, data/print flow, design decisions
- [docs/interview-qa.md](docs/interview-qa.md) — interview Q&A master guide

## Built With

- .NET 10 / MAUI
- CommunityToolkit.Mvvm (source-generated MVVM)
- CommunityToolkit.Maui (Popup)
- GraphicsView + IDrawable custom charts
