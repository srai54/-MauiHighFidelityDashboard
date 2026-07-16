# Detailed Walkthrough — Every Folder, Every File, Data Flow & Debugging

> Companion to [video-script-2min.md](video-script-2min.md). Use this for a longer
> recording (~8–10 min) or as spoken-answer prep for a technical Q&A.
> Keep the Solution Explorer on screen and expand each folder as you talk.

---

## 1. Solution map

```
MauiHighFidelityDashboard/
├── MauiProgram.cs              ← entry point + DI container
├── App.xaml / App.xaml.cs      ← application object, global resources, creates window
├── AppShell.xaml / .xaml.cs    ← Shell navigation container (route table)
├── MauiHighFidelityDashboard.csproj
│
├── Views/                      ← full pages
├── Components/                 ← 8 reusable ContentViews (partial views)
├── ViewModels/                 ← state + commands (the "controllers")
├── Models/                     ← plain data classes (entities/DTOs)
├── Services/                   ← data access + printing (the "repository")
│   └── Interfaces/             ← the contracts ViewModels depend on
├── Converters/                 ← IValueConverters used inside XAML bindings
├── Resources/                  ← styles, colors, fonts, images, icon, splash
└── Platforms/                  ← per-OS bootstrap only (no feature code)
```

---

## 2. Root files

| File | What it does |
|---|---|
| `MauiProgram.cs` | The entry point — think `Program.cs` + `Startup.cs` in ASP.NET. Builds the app, registers fonts, adds the CommunityToolkit, maximizes the Windows window, and configures **dependency injection**: `IDashboardDataService → StaticDashboardDataService`, `IPrintService → PrintService`, plus `MainViewModel`, `MainPage`, `DetailViewModel`, `DetailPage`. Swapping static data for the real REST API is one line here. |
| `App.xaml` | The application object's XAML side. Merges the global ResourceDictionaries (`Colors.xaml`, `Fonts.xaml`, `Styles.xaml`) so every page can reference shared colors/styles/converters. |
| `App.xaml.cs` | Creates the main `Window` and puts an `AppShell` inside it. |
| `AppShell.xaml` | The navigation container — like a route table in MVC. Declares routes so pages can be reached by URI, e.g. `detail?title=Widgets`. |
| `MauiHighFidelityDashboard.csproj` | Multi-targets `net10.0-windows10.0.19041.0` and `net10.0-android` (plus iOS/MacCatalyst when building on a Mac). One project → all platforms. |

---

## 3. Views/ — full pages

A View decides **how things look**, never **what the data is**. XAML holds layout + bindings; code-behind holds only purely visual logic.

| File | Purpose |
|---|---|
| `MainPage.xaml` / `.cs` | The dashboard. XAML composes the eight components in a grid. Code-behind has **zero business logic** — only the responsive-breakpoint code (`< 980 px` → sidebar becomes a hamburger overlay, cards restack, summary strip becomes 2×2). |
| `DetailPage.xaml` / `.cs` | The page you navigate to when clicking a sidebar item; receives the title via Shell query parameter. |
| `PrintPreviewPage.xaml` / `.cs` | Hosts a WebView with the generated HTML order report before handing it to the OS print dialog. |
| `LastMonthSummaryPopup.xaml` / `.cs` | CommunityToolkit `Popup` showing the structured label-value summary rows. |

## 4. Components/ — reusable ContentViews

The MAUI equivalent of **partial views / user controls**. The dashboard page is just these composed in a grid; each binds to the shared ViewModel and is reusable on any page.

| File | Renders |
|---|---|
| `SidebarView` | Dark navigation rail: logo, menu items, section labels. |
| `DashboardHeaderView` | Top bar: title, subtitle, month earnings/sales, Last Month Summary button. |
| `SalesChartView` | The spline **sales chart** — custom `GraphicsView` + `IDrawable`, no chart library. |
| `TrafficChartView` | The **donut traffic chart** — also custom `IDrawable`. |
| `SummaryCardView` | One KPI stat (wallet / referral / estimate / earning) in the summary strip. |
| `RevenueCardView` | One analytics card with the bold up/down arrow — repeated via `BindableLayout`. |
| `ActivityTimelineView` | The recent-activities timeline list. |
| `OrderTableView` | The order table: search box, status badges, pagination, print button. |

## 5. ViewModels/ — the brain (the "controller")

A ViewModel never touches a UI control and never knows *where* data comes from — it only talks to service **interfaces**.

| File | Purpose |
|---|---|
| `BaseViewModel.cs` | Shared state every screen needs: `Title`, `IsBusy` (from `ObservableObject`). |
| `MainViewModel.cs` | All dashboard state — seven `ObservableCollection`s (cards, revenue cards, activities, orders, page numbers, traffic, sales points) — and all **commands** (`[RelayCommand]`): load data, search orders, change page, print, open summary popup. `InitializeAsync` loads everything in parallel with `Task.WhenAll`. |
| `DetailViewModel.cs` | State for the detail page; receives the clicked title through Shell navigation. |

## 6. Models/ — plain data classes (entities/DTOs)

No logic, just data the ViewModels expose and the XAML binds to: `OrderModel`, `ActivityModel`, `TrafficModel`, `SalesData`, `DashboardCard`, `RevenueCardItem`, `SummaryStat`, `MenuItemModel`, `PageItem`, `SectionItem`.

Special file — **`Result.cs`**: a `Result<T>` wrapper so every service call returns *success-with-data* or *failure-with-message* instead of throwing. The ViewModel checks `IsSuccess` and reacts; no unhandled exceptions cross layers.

## 7. Services/ — data access (the "repository")

| File | Purpose |
|---|---|
| `Interfaces/IDashboardDataService.cs` | The data contract: get cards, revenue cards, activities, orders, traffic, sales data — all async, all returning `Result<T>`. |
| `Interfaces/IPrintService.cs` | The printing contract: print an HTML document. |
| `StaticDashboardDataService.cs` | The in-memory "database" used today — returns the exact data from the design screenshot. |
| `ApiDashboardDataService.cs` | The REST-ready implementation with `HttpClient` — already written; enable it by swapping one DI registration in `MauiProgram`. |
| `PrintService.cs` | Real OS printing: WebView2 print dialog on Windows, `PrintManager` on Android — paper or PDF. |

## 8. Converters/ — formatting inside bindings

Small `IValueConverter` classes — formatting logic that belongs to neither the ViewModel nor the View:

| File | Converts |
|---|---|
| `StatusColorConverter.cs` | Order status ("Delivered", "Pending"…) → badge text color. |
| `StatusBackgroundConverter.cs` | Order status → badge background color. |
| `HexToColorConverter.cs` | Hex string from a model (e.g. `"#4CAF50"`) → MAUI `Color`. |
| `AmountToCurrencyConverter.cs` | Number → money format. |

## 9. Resources/ — everything visual, centralized

| Folder/File | Purpose |
|---|---|
| `Styles/Colors.xaml` | The entire palette as named resources — single source of truth for the design. |
| `Styles/Fonts.xaml` | Text styles (sizes, weights). |
| `Styles/Styles.xaml` | Implicit/explicit control styles — the app's "CSS stylesheet". |
| `Fonts/` | OpenSans font files registered in `MauiProgram`. |
| `Images/`, `AppIcon/`, `Splash/`, `Raw/` | Images, app icon, splash screen, raw assets — shared by every platform. |

## 10. Platforms/ — per-OS bootstrap only

The only per-OS code — just what each OS requires to boot; **zero feature code**:

- `Android/` — `MainActivity.cs`, `MainApplication.cs`, Android manifest/resources.
- `iOS/`, `MacCatalyst/` — `AppDelegate.cs`, `Program.cs`, `Info.plist`.
- `Windows/` — `App.xaml(.cs)`, `app.manifest`, `Package.appxmanifest`.
- `Tizen/` — `Main.cs` (not built by default).

100% of the dashboard lives in the shared folders — that's the point of MAUI.

---

## 11. Data flow — from "DB" to UI

### The MVC analogy

In ASP.NET MVC: **request → Controller → business layer → repository → database**, then the server renders a view. Here there's no request — the app is stateful — but the layers map almost one-to-one:

| ASP.NET MVC | This MAUI app |
|---|---|
| Route table | `AppShell` routes |
| Controller action | ViewModel command (`[RelayCommand]`) |
| Business/service layer | ViewModel logic + `Result<T>` handling |
| Repository interface | `IDashboardDataService` |
| Repository implementation | `StaticDashboardDataService` / `ApiDashboardDataService` |
| Database | In-memory data today, REST API tomorrow |
| Razor view rendering | XAML **data binding** (automatic) |

### Startup flow, step by step

```
MauiProgram.CreateMauiApp()          1. DI container built: interface → implementation
        ↓
App.CreateWindow → AppShell          2. Shell shows MainPage
        ↓
DI constructs MainPage               3. MainViewModel injected via constructor,
   (MainViewModel injected)             set as BindingContext
        ↓
MainPage.OnAppearing                 4. calls viewModel.InitializeAsync()
        ↓
LoadDataCommand (RelayCommand)       5. Task.WhenAll → all datasets in parallel
        ↓
IDashboardDataService                6. ViewModel calls the INTERFACE — doesn't know
        ↓                               (or care) if it's static data or a REST API
StaticDashboardDataService           7. returns Result<T> (success + data / failure + msg)
        ↓
ObservableCollections filled         8. collections raise CollectionChanged
        ↓
XAML data binding                    9. UI updates itself — nothing calls "refresh"
```

### User-interaction flow (e.g. clicking a page number)

```
Tap in XAML → Command binding → RelayCommand in MainViewModel
→ slice _allOrders for that page → Orders ObservableCollection updated
→ the table re-renders itself via binding
```

**The one-line difference from MVC:** MVC re-renders the page per request; in MVVM the View is *permanently bound* to the ViewModel, and every state change flows to the screen by itself through `INotifyPropertyChanged` / `CollectionChanged`.

---

## 12. How to debug

### Build & run (Windows)

```powershell
dotnet build -f net10.0-windows10.0.19041.0
# then launch:
bin\Debug\net10.0-windows10.0.19041.0\win-x64\MauiHighFidelityDashboard.exe
```

> A running app instance **locks the exe** — the next build fails with MSB3027.
> Stop the app before rebuilding.

### Where to set breakpoints (follow the data path)

1. `MauiProgram.CreateMauiApp()` — verify DI registrations run.
2. `MainViewModel.LoadDataAsync()` — see all parallel loads start/finish.
3. `StaticDashboardDataService` methods — inspect exactly what data is returned.
4. Any converter's `Convert()` — see the raw value arriving from the binding.
5. `MainPage.ApplyResponsiveLayout()` — step through the breakpoint logic while resizing.

### Debugging bindings

- Binding failures **never throw** — they print to the **Output window** (`Debug` pane): look for lines like `Binding: 'X' property not found on 'Y'`. A blank label/empty list is almost always a typo'd binding path or a missing `BindingContext`.
- `builder.Logging.AddDebug()` is already on in DEBUG, so `ILogger`/`Debug.WriteLine` output lands in the same pane.
- Use **XAML Hot Reload** for instant layout/style tweaks and **Live Visual Tree** to inspect the real control hierarchy and property values at runtime.

### Gotcha: XAML that builds clean but crashes at launch

Component XAML is inflated at **runtime**, so an invalid property (e.g. a property that doesn't exist in MAUI) passes `dotnet build` and then kills the app at startup with a WER APPCRASH (`0xc000027b`) and **no console output**.

Fix workflow: temporarily add an `UnhandledException` handler in `Platforms/Windows/App.xaml.cs` that writes the exception to a file in `%TEMP%` — the `XamlParseException` names the exact file, property, and position. Rule of thumb: **after any XAML change, launch the exe and confirm the process stays alive** — "build succeeded" is not proof the XAML is valid.

### Debugging the service layer

- Every service call returns `Result<T>` — a failure never crashes the app. Breakpoint the `IsSuccess == false` branch in the ViewModel to see the error message.
- To test the API path without a backend, register `ApiDashboardDataService` in `MauiProgram` pointing at a mock URL and watch the `Result` failure propagate gracefully into the UI.

### Scripted UI verification hooks (temporary, in `MainPage.OnAppearing`)

| Env var | Effect |
|---|---|
| `DASH_TEST_DETAIL=1` | Auto-navigates to the detail page on launch. |
| `DASH_TEST_PRINT=1` | Auto-opens the print preview on launch. |

Useful for screenshot-based verification without clicking through the UI.
