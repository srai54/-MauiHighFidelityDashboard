# 2-Minute Intro Video Script

> Speaking pace ~150 words/min → ~300 words total. Timings are cues, not hard stops.
> Screen plan: app running on Windows (maximized) → quick resize demo → VS/IDE folder tree → app again.

---

## [0:00–0:15] — Opening (show the running dashboard)

"Hi, I'm Rajib. This is a high-fidelity admin dashboard I built with **.NET MAUI** — a single C# and XAML codebase that runs on **Windows and Android**. It recreates the given design screenshot pixel-for-pixel: sidebar navigation, sales spline chart, KPI cards, analytics cards, an activity timeline, and a live order table."

## [0:15–0:45] — Structure & MVVM (show the folder tree)

"The project follows the **MVVM structure** from the assignment: **Views** for pages, **Components** for eight reusable ContentViews, **ViewModels**, **Models**, and styles centralized in **Resources/Styles** as ResourceDictionaries. On top of that I added a **Services** layer with its interfaces in a separate folder — that's what keeps it industry-grade: ViewModels talk only to `IDashboardDataService`, so today the data is a static in-memory service, and swapping in the real REST API is **one line** in the DI container in `MauiProgram` — no ViewModel changes."

## [0:45–1:15] — Logic & code flow (show a click or refresh)

"The flow is: the page loads, `MainViewModel.InitializeAsync` fires and fetches all five datasets **in parallel** with `Task.WhenAll`. Every service call returns a **`Result<T>`** — success or failure, no unhandled exceptions — and the results land in `ObservableCollection`s, so the XAML updates itself through data binding. Commands come from **CommunityToolkit.Mvvm** source generators. The charts are fully custom — `GraphicsView` and `IDrawable`, no third-party chart library."

## [1:15–1:40] — Standout features (demo print + popup)

"Two features I want to highlight. **Printing is real**: the print button builds an HTML report of the filtered orders, shows a preview, and hands it to the OS — the WebView2 print dialog on Windows, Android's PrintManager on mobile — I can print to paper or PDF. And the **Last Month Summary** opens a structured popup with proper label-value rows."

## [1:40–2:00] — Responsiveness & close (resize the window live)

"Finally, the layout is **responsive**: below a 980-pixel breakpoint the sidebar collapses into a hamburger overlay, the cards restack into a single column, and the table pans horizontally — the same behavior on a phone, a small window, or 400% zoom. That's the app: one codebase, native performance, clean MVVM, and ready for a real backend. Thanks for watching."

---

# Part 2 — Project Structure Deep-Dive (~4–5 min, show the Solution Explorer)

> Use this section when a longer walkthrough is expected. Keep the IDE folder tree
> on screen and expand each folder as you talk about it.

## [Intro] — Root files (expand the project root)

"Before the folders, three root files run the show.

**`MauiProgram.cs`** is the entry point of the whole app — think of it as `Program.cs` + `Startup.cs` in ASP.NET. It builds the app, registers fonts, and most importantly configures the **dependency injection container**: this is where `IDashboardDataService` is mapped to `StaticDashboardDataService`, and where ViewModels and Pages are registered so MAUI can construct them with their dependencies injected.

**`App.xaml` / `App.xaml.cs`** is the application object. The XAML side merges the global ResourceDictionaries — colors, fonts, styles, converters — so every page can use them. The C# side creates the main window and puts an **`AppShell`** inside it.

**`AppShell.xaml`** is the navigation container. It declares the routes — like a route table in MVC — so I can navigate with a URI such as `detail?title=Widgets`."

## [Folder 1] — Views (expand it)

"**Views** holds the full *pages* — one XAML file plus a small code-behind each:

- **`MainPage`** — the dashboard itself. Its XAML is only layout and data binding; the code-behind has zero business logic — just the responsive breakpoint code that reshapes the grids when the window gets narrow.
- **`DetailPage`** — the page you navigate to when clicking a sidebar item.
- **`PrintPreviewPage`** — hosts a WebView with the generated HTML report before printing.
- **`LastMonthSummaryPopup`** — the structured popup for the summary button.

The rule: a View decides **how things look**, never **what the data is**."

## [Folder 2] — Components (expand it)

"**Components** are reusable **ContentViews** — the MAUI equivalent of partial views or user controls in MVC. The dashboard page is just these eight components composed in a grid: `SidebarView`, `DashboardHeaderView`, `SalesChartView`, `TrafficChartView`, `SummaryCardView`, `RevenueCardView`, `ActivityTimelineView`, and `OrderTableView`. Each one binds to the same ViewModel data, and each can be reused on any other page. The two chart components contain the custom `IDrawable` drawing code — no chart library."

## [Folder 3] — ViewModels (expand it)

"**ViewModels** are the brain — the closest thing to an MVC *controller*:

- **`BaseViewModel`** — shared state every screen needs: `Title`, `IsBusy`.
- **`MainViewModel`** — all dashboard state (seven `ObservableCollection`s for cards, orders, activities, traffic, chart points) and all the **commands**: load data, search orders, change page, print, open the summary popup.
- **`DetailViewModel`** — state for the detail page, receives the title through Shell navigation.

A ViewModel never touches a UI control and never knows *where* data comes from — it only talks to service **interfaces**."

## [Folder 4] — Models (expand it)

"**Models** are plain C# data classes — the entities/DTOs of the app: `OrderModel`, `ActivityModel`, `TrafficModel`, `SalesData`, `DashboardCard`, `RevenueCardItem`, `SummaryStat`, `MenuItemModel`, `PageItem`, `SectionItem`. No logic, just data the ViewModels expose and the XAML binds to.

One special file: **`Result.cs`** — a `Result<T>` wrapper so every service call returns *success-with-data* or *failure-with-message* instead of throwing exceptions."

## [Folder 5] — Services + Services/Interfaces (expand both)

"**Services** is the data-access layer — the *repository* of this app.

**`Interfaces/`** holds the contracts: **`IDashboardDataService`** (give me cards, orders, activities, traffic, sales data) and **`IPrintService`** (print this HTML).

Then three implementations: **`StaticDashboardDataService`** — the in-memory 'database' used today; **`ApiDashboardDataService`** — the REST-ready implementation with `HttpClient`, already written; and **`PrintService`** — platform printing via WebView2 on Windows and PrintManager on Android. Swapping static data for the real API is one line in `MauiProgram` — nothing else changes."

## [Folder 6] — Converters (expand it)

"**Converters** are small `IValueConverter` classes used *inside* XAML bindings — formatting logic that belongs to neither the ViewModel nor the View: `StatusColorConverter` and `StatusBackgroundConverter` turn an order status like *Delivered* into its badge colors, `HexToColorConverter` turns a hex string from the model into a MAUI `Color`, and `AmountToCurrencyConverter` formats numbers as money."

## [Folder 7] — Resources (expand it)

"**Resources** centralizes everything visual:

- **`Styles/Colors.xaml`** — the entire color palette as named resources, so the design has a single source of truth.
- **`Styles/Fonts.xaml`** and **`Styles/Styles.xaml`** — text styles and implicit control styles, like a CSS stylesheet for the app.
- **`Fonts/`**, **`Images/`**, **`AppIcon/`**, **`Splash/`**, **`Raw/`** — font files, images, the app icon, and the splash screen, all shared by every platform."

## [Folder 8] — Platforms (expand it)

"**Platforms** is the only place with per-OS code — just the native bootstrap each OS requires: `MainActivity` and `MainApplication` for **Android**, `AppDelegate` and `Program` for **iOS** and **MacCatalyst**, the app manifest and package manifest for **Windows**, plus **Tizen**. I wrote no feature code here — 100% of the dashboard lives in the shared folders above, which is the whole point of MAUI."

---

## [Execution flow] — "How does a request travel?" (show a diagram or narrate over the code)

"In MVC the flow is: **request → Controller → business layer → repository → database**, and the server renders a view back. Here there's no request — the app is stateful — but the layers map almost one-to-one:

| ASP.NET MVC | This MAUI app |
|---|---|
| Route table | `AppShell` routes |
| Controller action | `ViewModel` command (`[RelayCommand]`) |
| Business/service layer | ViewModel logic + `Result<T>` handling |
| Repository interface | `IDashboardDataService` |
| Repository implementation | `StaticDashboardDataService` / `ApiDashboardDataService` |
| Database | in-memory data today, REST API tomorrow |
| Razor view rendering | XAML **data binding** (automatic) |

**Startup flow, step by step:**

1. **`MauiProgram.CreateMauiApp()`** builds the DI container — interfaces are wired to implementations.
2. **`App`** creates the window with **`AppShell`**, which shows **`MainPage`**.
3. DI constructs `MainPage` and **injects `MainViewModel`** into it; the page sets it as its `BindingContext`.
4. `OnAppearing` calls **`InitializeAsync`** → **`LoadDataCommand`** fires.
5. The ViewModel calls **`IDashboardDataService`** — it doesn't know or care if that's static data or an API — and `Task.WhenAll` fetches all six datasets **in parallel**.
6. Each call returns a **`Result<T>`**; on success the data lands in **`ObservableCollection`s**.
7. That's the end of the code path — the collections raise change notifications, and **data binding updates the XAML automatically**. Nothing ever tells the UI to refresh.

**User-interaction flow** — say I click a page number in the order table:

`Tap in XAML → Command binding → RelayCommand in MainViewModel → filter/slice the orders → ObservableCollection updates → the table re-renders itself.`

So the one-line difference from MVC: in MVC the server **re-renders the page per request**; in MVVM the View is **permanently bound** to the ViewModel, and every state change flows to the screen by itself."

---

## Quick demo checklist (do while speaking)

1. Launch app maximized — hover over the chart tabs, click **WEEKLY**.
2. Show folder tree in IDE (Views / Components / ViewModels / Models / Services / Converters / Resources).
3. Click **Last Month Summary** → show structured popup → Close.
4. Click the 🖨 print button → preview → **Print** → OS dialog appears → Cancel.
5. Drag the window narrow → hamburger + stacked layout → drag back wide.
