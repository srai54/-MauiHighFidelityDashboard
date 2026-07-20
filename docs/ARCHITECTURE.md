# HighFidelity.Ui — Frontend Architecture

Single reference doc for this repo (FE only — the backend is a separate repo, [HighFidelity-Api](https://github.com/srai54/HighFidelity-Api)). Covers folder structure, what every piece is for, the MVVM/Observable pattern, and a top-30 interview Q&A.

---

## 1. Folder structure

```
HighFidelity.Ui/
├── Views/            Pages and popups (full screens / modal overlays)
├── Components/        Reusable ContentViews embedded inside Views
├── ViewModels/         Presentation logic — state + commands, no UI types
├── Models/             Plain data classes (POCOs) — zero logic, zero UI knowledge
├── Services/           Data access + cross-cutting concerns, behind interfaces
├── Converters/         IValueConverter implementations used in XAML bindings
├── Charts/             Hand-written chart drawing (GraphicsView + IDrawable)
├── Converters, Charts, Components, Models, Services, ViewModels, Views  ← the MVVM layers
├── Platforms/          Per-OS entry points (Android/iOS/MacCatalyst/Tizen/Windows)
├── Resources/          Fonts, images, app icon, splash screen, XAML style dictionaries
├── App.xaml(.cs)       Application object — merges resource dictionaries, creates the window
├── AppShell.xaml(.cs)  Shell navigation host — registers routes
├── MauiProgram.cs      Composition root — DI container, fonts, platform handler tweaks
└── run.cmd             Interactive launcher script (Windows or Android)
```

**The dependency rule enforced throughout:** each layer only knows the layer directly below it.
`Views → ViewModels → Services (interfaces) → Models`. A View never talks to a Service directly; a ViewModel never references a XAML type (except for popups/navigation, which MAUI requires).

---

## 2. Folder-by-folder: what's in each, and why

### `Models/` — the data contracts
Plain classes with **no logic and no UI knowledge**. Every other layer shares these types:

| File | Purpose |
|---|---|
| `OrderModel` | One order-table row: invoice, customer, country, price, status, `IsSelected` (bulk-select checkbox state) |
| `DashboardCard` | Top summary card: title, amount, icon glyph, theme color |
| `RevenueCardItem` | Analytics card: title, value, chart type (Bar/Area/Line), background/accent hex |
| `ActivityModel` | Timeline entry: actor, action, time, icon |
| `TrafficModel` | Donut chart segment: source, percentage, color |
| `MenuItemModel`, `PageItem`, `SectionItem`, `SummaryStat` | Sidebar items, pagination chips, detail-page rows, popup summary stats |
| `Result<T>` | **The error-handling envelope.** `Result.Success(data)` or `Result.Failure("message")` — the ViewModel checks `IsSuccess`/`IsFailure` instead of a `try/catch` around every call |

**Why a `Result<T>` instead of throwing exceptions?** Exceptions crossing a service→ViewModel boundary make failure an *implicit* control-flow path (easy to forget to catch). `Result<T>` makes failure an explicit, visible part of the method signature — the caller cannot ignore it without deliberately skipping the check.

### `Services/` — data access, behind an interface
| File | Purpose |
|---|---|
| `Interfaces/IDashboardDataService.cs` | The contract: 5 reads (`GetDashboardCardsAsync`, `GetRevenueCardsAsync`, `GetActivitiesAsync`, `GetOrdersAsync`, `GetTrafficSourcesAsync`) + 2 writes (`AddOrderAsync`, `DeleteOrdersAsync`), all returning `Task<Result<T>>` |
| `ApiDashboardDataService.cs` | **The only implementation.** Wraps `HttpClient` calls to the separate backend repo — `GetFromJsonAsync` for reads, `PostAsJsonAsync`/`DeleteAsync` for writes — every call wrapped in try/catch converted to `Result.Failure` |
| `ApiSettings.cs` | Resolves the backend's base address per platform (`10.0.2.2` for the Android emulator vs `localhost` elsewhere) — kept separate from DI wiring so "which backend" is a one-file answer |
| `Interfaces/IPrintService.cs` / `PrintService.cs` | Platform print/export of the order table (WebView2 print dialog on Windows, `PrintManager` on Android) |

**Why an interface with one implementation instead of a concrete class?** Because ViewModels depend on `IDashboardDataService`, not `ApiDashboardDataService`, the data source is swappable in **one line in `MauiProgram.cs`** with zero ViewModel/View changes. That's not theoretical here — this app's backend was rewritten from a Dapper Minimal API to a layered EF Core API without touching a single ViewModel. It also means ViewModels are unit-testable against a fake implementation of the interface.

### `ViewModels/` — presentation logic
| File | Purpose |
|---|---|
| `BaseViewModel` | Shared `IsBusy` + `Title` observable properties, inherited by every ViewModel |
| `MainViewModel` | The workhorse — dashboard state, `ObservableCollection`s, search/pagination, add/delete order commands, print, sidebar navigation |
| `DetailViewModel` | Populates the sidebar's "detail" pages (Widgets, UI Elements, Tables, etc.) from an in-memory catalog keyed by section name |

`MainViewModel` constructor takes `IDashboardDataService` and `IPrintService` — **constructor injection**, wired by the DI container in `MauiProgram.cs`. `InitializeAsync()` runs once per page lifetime and calls `LoadDataCommand`, which loads all five datasets **concurrently** with `Task.WhenAll` rather than five sequential awaits.

### `Views/` — full screens and popups
| File | Purpose |
|---|---|
| `MainPage.xaml` | The dashboard itself — sidebar, header, KPI cards, revenue cards, orders table, activity feed, traffic donut. Responsive layout logic lives in its code-behind. |
| `DetailPage.xaml` | Generic sidebar-navigation detail page, driven by `DetailViewModel` |
| `PrintPreviewPage.xaml` | Print preview + OS print dialog |
| `AddOrderPopup`, `ConfirmDeletePopup`, `OrderSummaryPopup`, `PeriodPickerPopup`, `LastMonthSummaryPopup` | `CommunityToolkit.Maui` popups (`ShowPopupAsync`) — modal overlays for the orders toolbar and stats |

### `Components/` — reusable pieces embedded inside Views
`SidebarView`, `DashboardHeaderView`, `SummaryCardView`, `RevenueCardView`, `ActivityTimelineView`, `OrderTableView`, `SalesChartView`, `TrafficChartView` — each a `ContentView` bound to a slice of `MainViewModel`'s state, so `MainPage.xaml` composes them rather than inlining hundreds of lines of markup.

### `Charts/` — hand-drawn charts, no chart library
| File | Purpose |
|---|---|
| `ChartTheme.cs` | Shared colors/stroke widths for chart drawing |
| `ChartData.cs` | Chart-specific data shapes (points, series) distinct from `Models/` — these never leave the UI layer |
| `ChartGeometry.cs` | Math helpers: spline interpolation, coordinate mapping |
| `Drawables/SplineChartDrawable.cs`, `DonutChartDrawable.cs`, `MiniBarChartDrawable.cs`, `MiniAreaChartDrawable.cs`, `MiniLineChartDrawable.cs` | Each implements `IDrawable` and is rendered inside a `GraphicsView` — pixel-level control over every chart, no third-party dependency |

### `Converters/` — XAML value converters
`AmountToCurrencyConverter`, `HexToColorConverter`, `StatusBackgroundConverter`, `StatusColorConverter` — each implements `IValueConverter` so XAML bindings can transform a model value (a hex string, a status string) into a UI-ready value (a `Color`, a formatted string) without putting UI logic in the ViewModel. Example: `HexToColorConverter` turns `"#F7284A"` into a MAUI `Color` via `Color.FromArgb`, falling back to grey if the string is null/empty — this is *why* every color in `Models/` is stored as a hex string, not a `Color`: it keeps the Models layer UI-framework-agnostic.

### `Platforms/` — per-OS entry points
`Android/MainActivity.cs`, `iOS/AppDelegate.cs`, `MacCatalyst/AppDelegate.cs`, `Windows/App.xaml.cs`, `Tizen/Main.cs` — the minimum glue each platform's runtime requires to boot into `MauiProgram.CreateMauiApp()`. Platform-conditional tweaks (Android Entry underline removal, WinUI focus-visual overrides, launch-maximized on Windows) live in `MauiProgram.cs` itself, guarded by `#if ANDROID` / `#if WINDOWS`.

### `Resources/` — fonts, images, styles
`Styles/Colors.xaml`, `Styles/Fonts.xaml`, `Styles/Styles.xaml` are merged into `App.xaml` so every page/component shares one design system. `Fonts/fa-solid-900.ttf` is Font Awesome Solid, registered as `"FontAwesome"` in `MauiProgram.cs` — icon glyphs throughout the app (`Models.DashboardCard.Icon`, etc.) are Unicode private-use-area characters rendered through this font, not images.

### Root files
| File | Purpose |
|---|---|
| `MauiProgram.cs` | **Composition root.** Registers fonts, DI services (`IDashboardDataService`, `IPrintService`, ViewModels, Pages), platform handler tweaks. The single place deciding "what implementation backs each interface." |
| `App.xaml(.cs)` | Merges resource dictionaries, creates the root window with `AppShell` |
| `AppShell.xaml(.cs)` | Shell navigation host — registers the `"detail"` route so `Shell.Current.GoToAsync("detail?title=...")` works |
| `HighFidelity.Ui.csproj` | Multi-targets `net10.0-windows10.0.19041.0` + `net10.0-android` (+ iOS/MacCatalyst when built on macOS) |
| `run.cmd` | Interactive picker: builds+runs Windows, or boots the Android emulator (if needed) and deploys |

---

## 3. MVVM in this app, concretely

```
Views (XAML)                    ← what the user sees; binds to a ViewModel via BindingContext
    ↕ data binding, no code-behind logic beyond visual concerns
ViewModels                      ← state + commands; MainViewModel, DetailViewModel
    ↓ constructor injection (interfaces only)
Services (IDashboardDataService, IPrintService)   ← data access, behind an interface
    ↓ Result<IReadOnlyList<T>>
Models                          ← plain data (POCOs), shared by every layer
```

- **Model** — `Models/*.cs`. No logic, no UI types, no knowledge of who consumes them.
- **View** — `Views/*.xaml` + `Components/*.xaml`. Declarative markup; code-behind is reserved for things XAML genuinely can't express (chart drawing, responsive re-layout math, platform print calls) — never business logic.
- **ViewModel** — `ViewModels/*.cs`. Owns state (`ObservableCollection`s, `ObservableProperty` fields) and behavior (`RelayCommand`s). Knows nothing about `Page`, `Button`, or any MAUI control type (with the practical exception of showing a popup or navigating, which needs `Shell.Current`).

**Why this separation matters in practice:** a ViewModel can be unit-tested with a fake `IDashboardDataService` and no UI thread at all. A designer can rework `MainPage.xaml` without touching a single line of C# logic. And — proven in this project — the entire backend can be rewritten (Dapper → EF Core, monorepo → separate repo) without a single ViewModel or View change, because the boundary is the *interface*, not the *implementation*.

---

## 4. `CommunityToolkit.Mvvm` — the Observable pattern used here

This app uses source generators from `CommunityToolkit.Mvvm` instead of hand-writing `INotifyPropertyChanged` boilerplate.

### `ObservableObject` + `[ObservableProperty]`
```csharp
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;
}
```
- `ObservableObject` implements `INotifyPropertyChanged` for you.
- `[ObservableProperty]` on a private field **generates** a public property (`IsBusy`, `Title`) that raises `PropertyChanged` automatically whenever it's set. Without this, you'd hand-write a backing field + getter + setter + `OnPropertyChanged()` call for every single bindable property.
- The field is `partial` under the hood — the source generator writes the property in a separate generated file at compile time; nothing about this is reflection-based or runtime-reflected, so it's AOT/trimming friendly (the `#pragma warning disable MVVMTK0045` in this codebase is silencing a WinRT-specific AOT-compatibility warning, not disabling the feature).

### `[RelayCommand]`
```csharp
[RelayCommand]
private async Task LoadDataAsync() { ... }
```
Generates a public `LoadDataCommand` property of type `IAsyncRelayCommand`, which XAML binds to (`Command="{Binding LoadDataCommand}"`) and which `InitializeAsync()` can also call directly (`await LoadDataCommand.ExecuteAsync(null)`). Handles the `CanExecute`/re-entrancy plumbing so a command bound to a button can't be double-invoked mid-execution without extra code.

### `ObservableCollection<T>`
```csharp
public ObservableCollection<DashboardCard> DashboardCards { get; } = [];
public ObservableCollection<OrderModel> Orders { get; } = [];
```
Unlike `[ObservableProperty]` (which notifies when the *property itself* is reassigned), `ObservableCollection<T>` raises `CollectionChanged` when items are **added, removed, or the list is cleared** — which is what a bound `CollectionView`/`ListView` in XAML listens to, to insert/remove visual rows without a full rebind. This is why every `Load*Async` method does `collection.Clear()` then `foreach (var x in data) collection.Add(x)` rather than reassigning the property — replacing the reference would break the XAML binding, which is bound to the original collection instance.

**Why both matter together:** a single keystroke in the orders search box (`OnSearchTextChanged`, generated from `[ObservableProperty] private string _searchText`) triggers `RefreshOrdersView()`, which clears and refills the `Orders` ObservableCollection — the `CollectionView` re-renders automatically. No code anywhere calls `.Refresh()` or manually touches a UI control; MVVM's core promise is that state changes are the *only* thing that drives rendering.

---

## 5. Top 30 interview questions

**1. Why MVVM instead of code-behind for everything?**
Testability and separation: ViewModels hold state/logic with zero UI-framework dependency, so they're unit-testable and reusable across pages; code-behind is reserved for things XAML can't express.

**2. What does `[ObservableProperty]` actually generate?**
A public property with a getter/setter that calls `OnPropertyChanging`/`OnPropertyChanged` around the field assignment — full `INotifyPropertyChanged` boilerplate, written by a Roslyn source generator at compile time, not via reflection.

**3. Why `ObservableCollection<T>` instead of `List<T>` for bound data?**
`List<T>` raises no notification when mutated; a `CollectionView` bound to it wouldn't know to re-render. `ObservableCollection<T>` raises `CollectionChanged` on Add/Remove/Clear, which the binding infrastructure listens to.

**4. Why does `RefreshOrdersView` clear-and-refill the collection instead of reassigning it?**
The XAML binding holds a reference to the *original* `ObservableCollection` instance. Reassigning the property (`Orders = newList`) wouldn't update the UI unless the property itself is also `[ObservableProperty]` — clearing and refilling mutates the same instance the View is already bound to.

**5. What is `[RelayCommand]` and what problem does it solve?**
It generates an `IRelayCommand`/`IAsyncRelayCommand` wrapping the annotated method, so XAML can bind `Command="{Binding XCommand}"` without hand-writing `ICommand` implementations, and it handles re-entrancy (a running async command won't fire again on a second tap by default).

**6. Why does `IDashboardDataService` return `Result<T>` instead of throwing?**
Explicit, visible failure handling. The signature itself tells the caller "this can fail" — a ViewModel checks `IsSuccess`/`IsFailure` rather than relying on a `try/catch` it might forget to write.

**7. Why is `Result<T>`'s constructor private with static factory methods?**
`Success(data)`/`Failure(error)` are the only two valid states, and making the constructor private forces every instance through one of those two factories — you can't accidentally construct an invalid half-success/half-failure state.

**8. Why does `MainViewModel` take its dependencies through the constructor rather than a service locator?**
Constructor injection makes dependencies explicit and testable — you can see everything a class needs by reading its constructor, and swap in a fake in a unit test without touching a static container.

**9. Where is the DI container configured, and what's registered?**
`MauiProgram.cs` — `IDashboardDataService` → `ApiDashboardDataService` (singleton, holds one `HttpClient`), `IPrintService` → `PrintService`, `MainViewModel`/`MainPage` as singletons (one dashboard state for the app's lifetime), `DetailViewModel`/`DetailPage` as transient (fresh instance per navigation).

**10. Why is `MainViewModel` a singleton but `DetailViewModel` transient?**
The dashboard's state (loaded orders, search text, pagination) should persist across the app's lifetime — re-navigating to it shouldn't re-fetch everything. The detail page's catalog view is cheap and per-navigation-scoped, so a fresh instance each time is simpler and avoids stale section state.

**11. Why does the interface have 5 read methods instead of one generic `GetAsync<T>`?**
Each resource (cards, revenue cards, activities, orders, traffic) has a distinct shape and, for orders, distinct write operations (`AddOrderAsync`/`DeleteOrdersAsync`) — a single generic method would need runtime type-switching internally, trading compile-time clarity for false genericity with no real reuse benefit at 5 endpoints.

**12. Why does `LoadDataCommand` use `Task.WhenAll` instead of five sequential `await`s?**
The five datasets are independent — sequential awaits would serialize five network round-trips for no reason. `Task.WhenAll` fires all five concurrently and only proceeds once every one completes (or the first one throws).

**13. What happens if the API is unreachable?**
`ApiDashboardDataService` catches the exception and returns `Result.Failure(message)` for that specific dataset — it doesn't crash the app or block the other four `Task.WhenAll` calls.

**14. Why is `IsBusy` on `BaseViewModel` instead of `MainViewModel`?**
Any ViewModel that does async work needs a busy flag for spinner/disable-button UI; putting it on the shared base means every ViewModel gets it for free and consistently.

**15. Why are chart colors/values stored as hex strings in `Models/`, not `Color`?**
`Color` is a MAUI UI type — putting it on a Model would leak a UI-framework dependency into the data layer (and into a future non-MAUI client, like a web dashboard consuming the same API). `HexToColorConverter` does the hex→`Color` translation only at the XAML-binding boundary.

**16. Walk through what `HexToColorConverter` does.**
`IValueConverter.Convert` receives the bound hex string, returns `Color.FromArgb(hex)` if it's non-empty, else a fallback grey — so a null/malformed color never crashes a binding, it just renders a neutral color.

**17. Why does `ConvertBack` throw `NotSupportedException`?**
The binding is one-way (Model → UI); there's no scenario where a user picks a `Color` in the UI and it needs converting back to a hex string, so implementing it would be dead code that could silently mask a binding-mode bug if ever mis-set to `TwoWay`.

**18. Why hand-write chart drawing (`IDrawable`) instead of using a charting library?**
Full control over pixel-level rendering to match the reference design exactly (spline smoothing, exact stroke widths, custom gradients) without being constrained by a third-party library's API or adding a dependency for what's fundamentally a few geometry calculations per chart.

**19. What's the difference between `Charts/ChartData.cs` and `Models/`?**
`Models/` are shared, framework-agnostic data contracts used across Services/ViewModels/Views. `ChartData.cs` holds UI-only shapes (plotted points, series) derived from Models purely for rendering — they never cross the service boundary.

**20. Why does `AppShell` register a `"detail"` route in its constructor instead of XAML?**
`Routing.RegisterRoute` wires up on-demand/lazy navigation (`Shell.Current.GoToAsync("detail?title=...")`) with query-parameter support, which plain XAML `ShellContent` declarations don't give you as flexibly for a single reusable detail page serving 15+ different sections.

**21. How does `DetailPage` render 15+ completely different sections from one page?**
`DetailViewModel.LoadSection(title)` looks up an in-memory `Catalog` dictionary keyed by section name and populates one `ObservableCollection<SectionItem>` — the page's XAML is generic and only cares about the collection, not which section it represents.

**22. Why is `IPrintService` its own interface rather than a method on `IDashboardDataService`?**
Different responsibility (rendering/exporting vs. data access) and different volatility (print is the one place with real platform-conditional code — WebView2 vs. Android `PrintManager`) — mixing it into the data interface would force every data-service implementation (including a future test fake) to also implement printing.

**23. What's the single biggest reason ViewModels never reference `HttpClient` or SQL types directly?**
The dependency rule: ViewModels depend only on `IDashboardDataService`. If a ViewModel ever imported `System.Net.Http` directly, swapping implementations would require ViewModel changes — defeating the entire point of the interface.

**24. Why does `MainProgram`'s Android/Windows platform code live behind `#if ANDROID`/`#if WINDOWS` in one shared file rather than per-platform partial classes?**
At the scale of a few handler tweaks (Entry underline removal, focus-visual overrides, maximize-on-launch), one file with compiler directives is easier to scan than hunting across `Platforms/*/` for related-but-scattered logic; partial classes would earn their place with substantially more platform-specific code.

**25. What would break if `Orders` were a `List<OrderModel>` instead of `ObservableCollection<OrderModel>`?**
Adding/removing orders (Instant Add, Bulk Delete) wouldn't visually update the bound `CollectionView` at all — the list would change in memory but the screen would show stale data until some other full rebind occurred.

**26. Why does `MainViewModel` keep a private `_allOrders` list *and* a public `Orders` ObservableCollection?**
`_allOrders` is the full unfiltered/unpaginated dataset; `Orders` is the paginated + search-filtered slice actually bound to the UI. Keeping them separate means filtering/paging logic (`RefreshOrdersView`) never has to re-fetch from the service — it just re-slices what's already in memory.

**27. How would you unit-test `MainViewModel.LoadDataCommand` without a real backend?**
Inject a fake `IDashboardDataService` (e.g. a test double returning canned `Result.Success`/`Result.Failure` values) through the constructor, call `InitializeAsync()`, and assert on the resulting `ObservableCollection` contents — no HTTP, no UI thread, no MAUI runtime needed.

**28. Why is `Result<T>.Data` nullable, and how does that interact with `IsSuccess`?**
`Data` is `null` in the failure case; `IsSuccess`/`IsFailure` are computed from whether `ErrorMessage` is set, not from `Data` being null — so a caller must check the flag before dereferencing `Data`, and the type system (via nullable reference types) nudges toward that with a `?`.

**29. What's the actual object graph MAUI builds when `MainPage` appears?**
DI resolves `MainPage(MainViewModel vm)`; `MainViewModel`'s constructor resolves `IDashboardDataService`/`IPrintService` from the same container; the page sets `vm` as its `BindingContext`; `OnAppearing` calls `await vm.InitializeAsync()`, which (once) runs `LoadDataCommand`.

**30. Why keep `DetailViewModel`'s section catalog in code instead of a JSON file or the API?**
It's demo/showcase content for the sidebar's secondary pages (Widgets, UI Elements, etc.) — not real app data — so there's no persistence or backend need; a `Dictionary` literal is the simplest thing that could possibly work, and moving it to JSON/API would be premature generality for content that never changes at runtime.
