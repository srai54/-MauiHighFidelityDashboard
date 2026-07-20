# MAUI High-Fidelity Dashboard — Complete Architecture & Interview Walkthrough

> A single document covering: architecture, folder-by-folder walkthrough, the full data
> pipeline (DB → UI), real-world deployment for mobile and web, how the hard-coded data
> gets replaced by a live database, and detailed interview Q&A.

---

## 1. Project Overview

**What it is:** A pixel-faithful recreation of the *srtdash* admin dashboard built in
**.NET MAUI** (net10.0), running on **Windows, Android, iOS, and macOS from one C# codebase**.

**Key characteristics:**

| Aspect | Choice |
|---|---|
| UI framework | .NET MAUI (XAML views + `GraphicsView` custom-drawn charts) |
| Pattern | MVVM (Model–View–ViewModel) |
| MVVM helpers | `CommunityToolkit.Mvvm` (source-generated `[ObservableProperty]`, `[RelayCommand]`) |
| Popups | `CommunityToolkit.Maui` (`Popup`, `ShowPopupAsync`) |
| Dependency Injection | Built-in `Microsoft.Extensions.DependencyInjection` via `MauiProgram` |
| Data | `IDashboardDataService` abstraction → today static/dummy, tomorrow REST API |
| Error handling | `Result<T>` (success/failure object, no exceptions across layers) |
| Charts | Hand-written `IDrawable` implementations (no chart library dependency) |

**Architecture in one picture:**

```
┌─────────────────────────────────────────────────────────────┐
│  Views (XAML)          MainPage, DetailPage, Popups          │
│  Components (XAML)     Sidebar, Header, Cards, OrderTable    │  ← what the user sees
│  Charts/Drawables      Spline, Donut, MiniBar/Line/Area      │
└───────────────▲─────────────────────────────────────────────┘
                │ data binding (BindingContext)
┌───────────────┴─────────────────────────────────────────────┐
│  ViewModels            MainViewModel, DetailViewModel        │  ← presentation logic
│                        (commands, filtering, paging, state)  │
└───────────────▲─────────────────────────────────────────────┘
                │ constructor injection (IDashboardDataService)
┌───────────────┴─────────────────────────────────────────────┐
│  Services              StaticDashboardDataService (today)    │  ← data access
│                        ApiDashboardDataService   (future)    │
└───────────────▲─────────────────────────────────────────────┘
                │ Result<IReadOnlyList<T>>
┌───────────────┴─────────────────────────────────────────────┐
│  Models                OrderModel, DashboardCard, Result<T>… │  ← plain data (POCOs)
└─────────────────────────────────────────────────────────────┘
```

The golden rule enforced here: **each layer only knows the layer directly below it.**
Views never touch services; ViewModels never touch XAML; services never know who consumes them.

---

## 2. Folder-by-Folder Walkthrough

### `/` (project root)
| File | Purpose |
|---|---|
| `MauiProgram.cs` | **Composition root.** Builds the `MauiApp`, registers fonts, DI services, platform handler tweaks (Android Entry underline removal, WinUI focus-visual overrides, launch-maximized). The single place where "what implementation backs each interface" is decided. |
| `App.xaml` / `App.xaml.cs` | The `Application` object. Merges the resource dictionaries (`Colors.xaml`, `Fonts.xaml`, `Styles.xaml`) so every page can use shared styles. Creates the root window with `AppShell`. |
| `AppShell.xaml` / `.cs` | **Shell navigation host.** Declares routes (`detail` → `DetailPage`) so `Shell.Current.GoToAsync("detail?title=...")` works. |
| `MauiHighFidelityDashboard.csproj` | Multi-targeting: `net10.0-windows10.0.19041.0` + `net10.0-android` (plus iOS/MacCatalyst when built on macOS). Declares app icon, splash, min OS versions, NuGet packages. |

### `Models/`
Plain data classes (POCOs/records) with **zero logic and zero UI knowledge**. They are the
contract every other layer shares:

- `OrderModel` — invoice, customer, country, price, status (the order table rows)
- `DashboardCard` — title, amount, icon glyph, theme color (top summary cards)
- `RevenueCardItem` — analytics cards (title, value, chart type, background/accent hex)
- `ActivityModel` — timeline entries (actor, action, time, icon)
- `TrafficModel` — donut chart segments (source, percentage, color)
- `MenuItemModel`, `PageItem`, `SectionItem`, `SummaryStat` — sidebar items, pagination chips, popup rows
- `Result<T>` — **the error-handling envelope.** A service returns `Result.Success(data)` or
  `Result.Failure("message")`; the ViewModel checks `IsSuccess` instead of catching exceptions.

### `Services/`
The **data-access layer**, split into contract and implementations:

- `Interfaces/IDashboardDataService.cs` — five async methods (`GetOrdersAsync`,
  `GetDashboardCardsAsync`, …) each returning `Task<Result<IReadOnlyList<T>>>`.
- `StaticDashboardDataService.cs` — **current implementation**: returns hard-coded lists
  (mirrors the reference screenshot). This is the "fake database."
- `ApiDashboardDataService.cs` — **production-shaped implementation**: same interface, but
  each method does `HttpClient.GetFromJsonAsync<List<T>>("api/dashboard/…")` with try/catch
  converted into `Result.Failure`. Swapping to it is a **one-line change in `MauiProgram.cs`**.
- `Interfaces/IPrintService.cs` + `PrintService.cs` — platform print/export of the order table.

### `ViewModels/`
Presentation logic — everything the UI *does*, with no reference to any UI type except popups/navigation:

- `BaseViewModel` — shared `IsBusy` + `Title` observable properties.
- `MainViewModel` — the workhorse:
  - `ObservableCollection`s for cards, orders, activities, traffic (UI auto-updates on change)
  - `LoadDataCommand` runs all five service calls **concurrently** with `Task.WhenAll`
  - search (`OnSearchTextChanged` → re-filter), pagination (`PageSize = 5`, `PageNumbers`),
    footer text ("Showing 1 to 5 of 30 entries")
  - commands for add/delete order popups, print, refresh, sidebar navigation
- `DetailViewModel` — receives the `title` query parameter for the detail page.

### `Views/`
Full pages and popups (XAML + minimal code-behind):

- `MainPage` — the dashboard. Code-behind only handles **view concerns**: responsive breakpoint
  (`Width < 980` → single-column + hamburger sidebar), scroll-to-top on launch, calling
  `_viewModel.InitializeAsync()` in `OnAppearing`.
- `DetailPage` — navigated target for sidebar items.
- Popups: `AddOrderPopup`, `ConfirmDeletePopup`, `OrderSummaryPopup`, `LastMonthSummaryPopup`,
  `PeriodPickerPopup`, `PrintPreviewPage`.

### `Components/`
Reusable `ContentView`s that compose `MainPage` — the page is an assembly of these, keeping
each file small and testable in isolation:

`SidebarView`, `DashboardHeaderView`, `SummaryCardView` (4 top cards), `RevenueCardView`
(analytics mini-chart cards), `SalesChartView` (spline chart + period tabs),
`TrafficChartView` (donut), `ActivityTimelineView`, `OrderTableView` (search + table +
pagination + bulk select/delete).

### `Charts/`
The **config-driven chart architecture** (no third-party chart library):

- `ChartData.cs` — global, declarative datasets (`SalesByPeriod`, `BounceRateByPeriod`,
  `RevenueBarValues`, …). *Adding a chart tomorrow is a data change, not a code change.*
- `ChartGeometry.cs` — shared math (spline smoothing, scaling points into a rect).
- `ChartTheme.cs` — shared colors/stroke constants.
- `Drawables/` — one `IDrawable` per chart shape: `SplineChartDrawable`, `DonutChartDrawable`,
  `MiniBarChartDrawable`, `MiniLineChartDrawable`, `MiniAreaChartDrawable`. Each draws onto a
  `GraphicsView` canvas — resolution-independent and identical on every platform.

### `Converters/`
`IValueConverter`s used in XAML bindings — tiny mapping functions:
`StatusColorConverter` / `StatusBackgroundConverter` (order status → chip colors),
`AmountToCurrencyConverter`, `HexToColorConverter`.

### `Resources/`
- `Styles/Colors.xaml` — the palette as named resources (single source of truth for colors)
- `Styles/Styles.xaml` — implicit/explicit styles for controls
- `Styles/Fonts.xaml` + `Resources/Fonts/` — OpenSans + Font Awesome glyph font
- `AppIcon/`, `Splash/`, `Images/`, `Raw/` — MAUI single-project assets, auto-resized per platform

### `Platforms/`
Per-OS bootstrap and native config. Only code that **must** differ per platform lives here:

- `Android/` — `MainActivity` (the Android entry activity), `MainApplication` (hooks
  `MauiProgram.CreateMauiApp`), `AndroidManifest.xml` (permissions, app label)
- `Windows/` — `App.xaml(.cs)` (WinUI bootstrap), `Package.appxmanifest` (MSIX identity)
- `iOS/` / `MacCatalyst/` — `AppDelegate`, `Program.Main`, `Info.plist`
- `Tizen/` — template default, not actively targeted

### `docs/`
Design specs, implementation plans, deployment guide, interview question bank, video scripts.

---

## 3. Code Walkthrough — the App's Life, Step by Step

### 3.1 Startup pipeline (what runs, in order)

```
Platform entry point                      (Platforms/Android/MainApplication.cs
                                           or Platforms/Windows/App.xaml.cs …)
        │
        ▼
MauiProgram.CreateMauiApp()               ← fonts, handler tweaks, DI registrations
        │
        ▼
new App()                                 ← App.xaml merges Colors/Fonts/Styles dictionaries
        │
        ▼
App.CreateWindow → new AppShell()         ← routes registered ("detail")
        │
        ▼
Shell shows MainPage                      ← DI constructs it: MainPage(MainViewModel vm)
        │                                    which itself needed (IDashboardDataService, IPrintService)
        ▼
MainPage.OnAppearing()
        └── await _viewModel.InitializeAsync()
                └── LoadDataCommand → Task.WhenAll(5 service calls)
                        └── ObservableCollections fill → bindings refresh → UI renders
```

### 3.2 Dependency injection registrations (MauiProgram.cs)

```csharp
builder.Services.AddSingleton<IDashboardDataService, StaticDashboardDataService>();
builder.Services.AddSingleton<IPrintService, PrintService>();
builder.Services.AddSingleton<MainViewModel>();   // one dashboard state for app lifetime
builder.Services.AddSingleton<MainPage>();
builder.Services.AddTransient<DetailViewModel>(); // fresh instance per navigation
builder.Services.AddTransient<DetailPage>();
```

Why singleton vs transient: the dashboard is one long-lived screen whose state (loaded data,
current page, search text) should survive navigation; detail pages are cheap, parameterized,
and disposable.

### 3.3 A user interaction, end to end (search box example)

1. User types "japan" → `Entry.Text` two-way binding writes `MainViewModel.SearchText`.
2. Source generator emitted `OnSearchTextChanged` partial hook → resets `_currentPage = 1`
   and calls `RefreshOrdersView()`.
3. `FilteredOrders` LINQ filters `_allOrders` on customer/country/status/invoice.
4. `Orders` collection is cleared and refilled with page 1 (5 rows); `PageNumbers` rebuilt;
   `OrdersFooterText` recomputed.
5. `ObservableCollection` change notifications flow to the `BindableLayout`/`CollectionView`
   in `OrderTableView` — the table redraws. **No code ever touched a UI element directly.**

---

## 4. Data Flow: Database → Screen (the full pipeline)

> MAUI is client-side, so there is no "controller" inside the app — the **ViewModel plays the
> controller role** (it mediates between data and view). The controller proper lives in the
> **backend Web API**. Here is the complete real-world pipeline and every file involved.

### 4.1 The complete pipeline

```
   SQL Server / PostgreSQL
        │  (tables: Orders, Cards, Activities, Traffic)
        ▼
   EF Core DbContext                      [backend: Data/AppDbContext.cs]
        │  LINQ → SQL
        ▼
   Repository / Service                   [backend: Services/OrderService.cs]
        │
        ▼
   ASP.NET Core Controller                [backend: Controllers/DashboardController.cs]
        │  GET api/dashboard/orders → JSON over HTTPS
        ▼
════════ network boundary ═══════════════════════════════════════════
        ▼
   ApiDashboardDataService                [app: Services/ApiDashboardDataService.cs]
        │  HttpClient.GetFromJsonAsync<List<OrderModel>>(…)
        │  wraps outcome in Result<T>     [app: Models/Result.cs]
        ▼
   MainViewModel.LoadOrdersAsync          [app: ViewModels/MainViewModel.cs]
        │  if (result.IsSuccess) fill _allOrders → RefreshOrdersView()
        ▼
   ObservableCollection<OrderModel> Orders
        │  INotifyCollectionChanged
        ▼
   OrderTableView.xaml bindings           [app: Components/OrderTableView.xaml]
        │  {Binding Customer}, converters color the status chip
        ▼
   Pixels on screen
```

### 4.2 Files in the pipeline, per request (client side, today)

| Step | File | Responsibility |
|---|---|---|
| 1 | `MauiProgram.cs` | decides which `IDashboardDataService` is injected |
| 2 | `Views/MainPage.xaml.cs` | `OnAppearing` → `InitializeAsync()` (once) |
| 3 | `ViewModels/MainViewModel.cs` | `LoadDataCommand`, `Task.WhenAll` of 5 loads |
| 4 | `Services/StaticDashboardDataService.cs` (today) / `ApiDashboardDataService.cs` (with API) | produce `Result<IReadOnlyList<T>>` |
| 5 | `Models/Result.cs` + entity models | typed transport between layers |
| 6 | `Components/*.xaml` | data-bound templates render the collections |
| 7 | `Converters/*.cs` | value → color/currency formatting during binding |
| 8 | `Charts/ChartData.cs` + `Charts/Drawables/*` | datasets drawn on `GraphicsView` |

### 4.3 Why the `Result<T>` envelope instead of exceptions?

- The UI must *always* render something — a failed fetch becomes an error banner, not a crash.
- Callers are forced (by the type) to consider failure; an exception can be silently forgotten.
- `Task.WhenAll` with five loads: one failing endpoint doesn't abort the other four.

---

## 5. Real-World Deployment

### 5.1 Mobile — Android (Play Store)

1. **Prepare identity** — in `.csproj`: real `ApplicationId`
   (e.g. `io.fiftyfivetech.dashboard`), bump `ApplicationDisplayVersion` (user-facing) and
   `ApplicationVersion` (integer, must increase every upload).
2. **Create a signing keystore** (one time, guard it — losing it = losing the app identity):
   ```bash
   keytool -genkeypair -v -keystore dashboard.keystore -alias dashkey \
           -keyalg RSA -keysize 2048 -validity 10000
   ```
3. **Publish a signed release bundle (AAB)**:
   ```bash
   dotnet publish -f net10.0-android -c Release \
     -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=dashboard.keystore \
     -p:AndroidSigningKeyAlias=dashkey -p:AndroidSigningKeyPass=*** -p:AndroidSigningStorePass=***
   ```
   Output: `bin/Release/net10.0-android/publish/*.aab` (AAB for Play Store; APK for sideload/testing).
4. **Play Console** — create app, upload AAB to *Internal testing* → *Closed* → *Production*;
   fill data-safety form, screenshots; review takes hours→days. Staged rollout (5% → 100%).

### 5.2 Mobile — iOS (App Store)

1. Requires a **Mac build host** (or cloud Mac) + Apple Developer account ($99/yr).
2. Certificates & **provisioning profiles** via Apple Developer portal / Xcode.
3. ```bash
   dotnet publish -f net10.0-ios -c Release -p:ArchiveOnBuild=true \
     -p:CodesignKey="Apple Distribution: …" -p:CodesignProvision="DashboardAppStore"
   ```
4. Upload the `.ipa` with Xcode Organizer / Transporter → **TestFlight** beta → App Store review.

### 5.3 Windows desktop

- `dotnet publish -f net10.0-windows10.0.19041.0 -c Release` — currently
  `WindowsPackageType=None` (unpackaged exe, good for direct distribution).
- For the **Microsoft Store**: switch to MSIX packaging, sign with a certificate, submit via
  Partner Center.

### 5.4 "Web" deployment — what it means for this stack

.NET MAUI itself does not produce a website. In a real-world scenario "web" means two things:

**(a) The backend API the app talks to** (the mandatory part):

```
ASP.NET Core Web API (DashboardController + EF Core)
   → containerized (Dockerfile)
   → pushed by CI to a registry
   → hosted on Azure App Service / AKS / AWS ECS
   → fronted by HTTPS, JWT auth, health checks (/healthz)
   → DB: Azure SQL / RDS with migrations applied on deploy (EF `dotnet ef database update`
     or migration bundles)
Environments: dev → staging → production, each with its own connection string in
Key Vault/App Service settings — never in source.
```

**(b) A browser-delivered UI (optional):** the same MVVM core (Models, ViewModels, Services
are plain .NET) can be reused in a **Blazor Hybrid/Web** front end — publish as an ASP.NET
Core site or WebAssembly to Azure Static Web Apps. The XAML views are the only non-reusable layer.

### 5.5 CI/CD (typical real pipeline)

```yaml
# GitHub Actions sketch
on: push → branches [master]
jobs:
  build-test:   dotnet restore → dotnet build -c Release → dotnet test
  android:      dotnet publish -f net10.0-android  (signing keys from repo Secrets)
                → upload AAB to Play internal track (fastlane / gradle-play-publisher)
  ios:          runs-on: macos-latest → publish ipa → TestFlight (App Store Connect API key)
  api:          docker build → push → deploy slot swap on Azure App Service
```

Secrets (keystore, Apple keys, connection strings) live in the CI secret store, never in git.

---

## 6. Replacing Hard-Coded Values with a Real-Time Database

The app was deliberately built so this is a **configuration change, not a rewrite**.

### Step 1 — Build the backend (new code, outside this repo)

```csharp
// Controllers/DashboardController.cs (ASP.NET Core)
[ApiController]
[Route("api/dashboard")]
public class DashboardController(AppDbContext db) : ControllerBase
{
    [HttpGet("orders")]
    public async Task<ActionResult<List<OrderModel>>> GetOrders() =>
        await db.Orders.AsNoTracking().OrderByDescending(o => o.Invoice).ToListAsync();
    // …cards, activities, traffic, revenue-cards endpoints matching
    //   the routes ApiDashboardDataService already calls
}
```
EF Core maps `Orders`, `Cards`, `Activities`, `Traffic` tables; migrations create the schema.

### Step 2 — Flip one registration in `MauiProgram.cs`

```csharp
// remove: builder.Services.AddSingleton<IDashboardDataService, StaticDashboardDataService>();
builder.Services.AddSingleton<IDashboardDataService>(sp =>
{
    var client = new HttpClient { BaseAddress = new Uri("https://api.mycompany.com/") };
    return new ApiDashboardDataService(client);
});
```

`ApiDashboardDataService` already exists and already targets `api/dashboard/orders`,
`api/dashboard/cards`, etc. **No ViewModel, View, or Model changes** — they only know
`IDashboardDataService` and `Result<T>`.

### Step 3 — Clean up the few remaining literals

Small display strings still live in `MainViewModel` (`CurrentMonthEarnings = "$3468.96"`,
`LastMonthStats`) and chart curves in `Charts/ChartData.cs`. These become:
- two more service methods (`GetMonthlySummaryAsync`, `GetChartSeriesAsync`), and
- the chart views feed the fetched arrays into the same drawables (the drawables are already
  data-driven — they take `float[]`, they don't care where it came from).

### Step 4 — Make it *real-time* (push, not just fetch)

- **SignalR** hub on the API (`/hubs/dashboard`); server broadcasts `OrderCreated`,
  `MetricsUpdated` when the DB changes.
- The app subscribes with `HubConnection`; on a message it updates the
  `ObservableCollection` **on the main thread**
  (`MainThread.BeginInvokeOnMainThread(() => Orders.Insert(0, order))`) — the UI updates live.
- Lighter alternative: polling timer calling `RefreshCommand` every N seconds.

### Step 5 — Production hardening

| Concern | Real-world answer |
|---|---|
| Offline | Cache last payload in local **SQLite** (`sqlite-net-pcl`); serve cache on `Result.Failure`, sync when back online |
| Auth | JWT bearer token added by an `HttpMessageHandler`; refresh-token flow; tokens in `SecureStorage` |
| Resilience | `Polly` retry + circuit-breaker around `HttpClient` (via `IHttpClientFactory`) |
| Config per environment | `appsettings.{env}.json` embedded, or MSBuild `#if` — never hard-code the base URL |
| Paging at scale | Move `Skip/Take` from the ViewModel to the API (`?page=2&size=5`) once orders stop fitting in memory |

---

## 7. Interview Questions & Detailed Answers

### Architecture & MVVM

**Q1. Walk me through your architecture. Why MVVM?**
Four layers: Models (POCOs), Services (data access behind `IDashboardDataService`),
ViewModels (state + commands), Views/Components (XAML). MVVM because XAML's binding engine
makes it natural: the ViewModel exposes observable state, the View declaratively binds to it.
Benefits I actually used: (1) swappable data source — static today, REST tomorrow, one DI
line; (2) testable logic — search/pagination in `MainViewModel` is plain C#, no UI needed to
test it; (3) parallel work — XAML and logic evolve independently.

**Q2. Where's the controller in MAUI?**
There isn't one — MVVM replaces MVC's controller with the ViewModel plus the binding engine.
The binding engine does the "route user input to code" job (a controller's main role), and the
ViewModel holds the handlers (`[RelayCommand]` methods). In the full system the literal
controllers live in the ASP.NET Core Web API.

**Q3. What does CommunityToolkit.Mvvm generate for you?**
`[ObservableProperty] string _searchText` generates a `SearchText` property raising
`PropertyChanged`, plus partial hooks like `OnSearchTextChanged(value)` — which I use to
re-filter orders. `[RelayCommand]` on `LoadDataAsync` generates `LoadDataCommand`
(`IAsyncRelayCommand`) that XAML binds to. Source generators (compile time), not reflection —
so it's fast and AOT-friendly.

**Q4. Why `ObservableCollection` and what's its limitation?**
It raises `CollectionChanged`, so adds/removes update bound UI automatically. Limitation: no
`AddRange` — every `Add` fires a UI notification. For 5-row pages that's fine; at scale I'd
batch by swapping the collection reference or use a range-enabled collection.

**Q5. Singleton vs Transient — how did you choose?**
`MainViewModel`/`MainPage` are singletons: the dashboard is the app's home; its loaded data
and UI state should survive navigating away and back (and `InitializeAsync` guards with
`IsDataLoaded` so data loads once). `DetailViewModel`/`DetailPage` are transient: each
navigation carries a different `title` parameter and holds no shared state.

### Data layer

**Q6. Explain your `Result<T>` pattern. Why not just throw?**
`Result<T>` carries either `Data` or `ErrorMessage`; `IsSuccess` discriminates. Services catch
at the boundary (`ApiDashboardDataService` catches `HttpRequestException` etc.) and return
`Failure`. The ViewModel then decides presentation (empty state, banner). Advantages:
failure is part of the method signature so it can't be ignored; five concurrent loads degrade
independently; no exception-driven control flow across layers.

**Q7. How would you connect this to a real database?**
(See §6 — summarize:) Build an ASP.NET Core Web API with EF Core over SQL; the endpoints
match the routes `ApiDashboardDataService` already calls; change one DI registration in
`MauiProgram`. Nothing above the service layer changes — that's the payoff of programming to
`IDashboardDataService`.

**Q8. Would you ever put SQLite in the app itself?**
Yes, for offline-first: cache the last successful payloads locally, render instantly from
cache on launch, refresh from the API in the background, and queue local writes (new orders)
for sync. The clean seam is a third `IDashboardDataService` implementation (a caching
decorator wrapping the API service).

**Q9. Why does `GetOrdersAsync` return `IReadOnlyList<T>`?**
It states the contract: the service hands you a snapshot you must not mutate. Mutation happens
deliberately — the ViewModel copies into `_allOrders` and projects into `Orders` for display.

### UI, XAML & Charts

**Q10. How do the charts work without a chart library?**
Each chart is an `IDrawable` painting on a `GraphicsView` canvas (`Charts/Drawables/`).
Datasets are declared once in `ChartData.cs` (e.g. `SalesByPeriod["MONTHLY"]` = two
`float[]` series + labels). `ChartGeometry` normalizes values into the canvas rect;
`ChartTheme` centralizes colors. Adding a chart = add a dataset + point a view at a drawable.
Benefits: zero dependency, pixel-exact fidelity to the design, one rendering path on all platforms.

**Q11. How did you make the layout responsive?**
A width breakpoint in `MainPage` code-behind (980 logical px): below it, the grid collapses to
one column, the sidebar becomes a hamburger-toggled overlay, paddings shrink. It's view
logic, so code-behind is the correct home — not the ViewModel.

**Q12. What platform-specific issues did you hit?**
(1) Android Material `Entry` draws an underline — removed via a handler mapping setting
`BackgroundTintList` transparent. (2) WinUI paints an accent underline on focused TextBoxes;
overriding resources before the control is loaded throws `COMException 0x800F0902`, so the
override is deferred to the `Loaded` event, and rest-state keys must be overridden too.
(3) WinUI auto-scrolls the first focusable element into view on launch — countered with a
dispatched `ScrollToAsync(0,0)`. All fixed centrally in `MauiProgram`/`MainPage`, not
scattered through views.

**Q13. Value converters — why not properties on the model?**
`StatusColorConverter` maps "Open"→green at binding time. Keeping color out of `OrderModel`
keeps models UI-agnostic (the API shouldn't dictate pixels), and one converter serves every
view that shows a status.

### Async & performance

**Q14. Why `Task.WhenAll` in `LoadDataAsync`?**
Five independent fetches; sequential awaits would sum their latencies, `WhenAll` overlaps them
so total ≈ slowest call. Safe because each populates a different collection — with continuations
resuming on the captured UI context, they don't collide.

**Q15. `async void` in `OnAppearing` — isn't that bad?**
`async void` is required for event handlers/overrides; the rule is: only there, and the body
must not let exceptions escape. Work inside goes through the command/`Result` path which
handles failures. Everywhere else the code is `async Task`.

**Q16. How does search + pagination work, and where would it break at scale?**
All orders sit in `_allOrders`; a LINQ filter + `Skip/Take` projects the current page (5 rows)
into `Orders`. Fine for hundreds of rows. At tens of thousands, memory and filter cost grow —
the fix is server-side paging/filtering (`GET api/orders?search=&page=&size=`), turning the
ViewModel into a thin requester. The UI wouldn't change.

### Deployment & DevOps

**Q17. Describe shipping this app to real users.**
Android: signed AAB via `dotnet publish`, Play Console internal → production with staged
rollout. iOS: publish on a Mac with distribution cert + provisioning profile, TestFlight, then
review. Windows: MSIX (Store) or signed exe. Backend: Docker → Azure App Service with
dev/staging/prod slots; EF migrations on deploy; secrets in Key Vault. CI/CD builds, tests,
signs, and uploads on every merge to master (§5.5).

**Q18. Same codebase — how does one project build 4 apps?**
Multi-targeting: `<TargetFrameworks>net10.0-windows…;net10.0-android;…</TargetFrameworks>`.
Each TFM compiles shared C#/XAML plus its `Platforms/<OS>/` folder; MAUI's single-project
system generates per-platform icons/splash from one SVG. `#if ANDROID` / `#if WINDOWS`
sections compile only into their target.

**Q19. How do you manage app versions and API compatibility?**
`ApplicationDisplayVersion` (marketing) + `ApplicationVersion` (store-enforced increasing
integer). API side: versioned routes (`/api/v1/...`) so old app builds in the field keep
working while v2 rolls out; additive JSON changes are safe because `System.Text.Json`
ignores unknown fields by default.

**Q20. What would you add before calling this production-ready?**
Auth (JWT + `SecureStorage`), retry/circuit-breaker (Polly), offline cache (SQLite),
crash/analytics telemetry (App Center/Sentry/AppInsights), unit tests around
`MainViewModel` filtering/paging and the services, localization (`.resx` instead of literal
strings), accessibility pass (semantic properties), and server-side paging.

---

## 8. One-Minute Summary (elevator pitch)

> "It's a cross-platform admin dashboard in .NET MAUI — one C# codebase producing Windows,
> Android, iOS and macOS apps. Clean MVVM: XAML views bind to source-generated ViewModels;
> all data flows through an `IDashboardDataService` interface returning `Result<T>`, so the
> current in-memory data source swaps for a REST API by changing one DI registration —
> the API-backed implementation is already written. Charts are hand-drawn `IDrawable`s fed by
> declarative datasets, so they're pixel-faithful and dependency-free. Deployment is
> `dotnet publish` per target — signed AAB to Play, IPA via TestFlight, MSIX for Windows —
> with the backend API containerized on Azure and CI/CD doing build-sign-upload on merge."
