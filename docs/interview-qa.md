# MauiHighFidelityDashboard — Interview Q&A Master Guide

> Use this alongside `docs/interview-architecture.md` for a complete interview prep.

---

## Table of Contents

1. [Architecture & Design](#1-architecture--design)
2. [MVVM & Data Binding](#2-mvvm--data-binding)
3. [MAUI Framework & XAML](#3-maui-framework--xaml)
4. [Dependency Injection](#4-dependency-injection)
5. [Real API Integration (No SQLite)](#5-real-api-integration-no-sqlite)
6. [Error Handling & Result Pattern](#6-error-handling--result-pattern)
7. [Performance & Optimization](#7-performance--optimization)
8. [Testing](#8-testing)
9. [Component Design & Reusability](#9-component-design--reusability)
10. [Responsiveness & Cross-Platform](#10-responsiveness--cross-platform)
11. [Mock Interview — Walkthrough Script](#11-mock-interview--walkthrough-script)

---

## 1. Architecture & Design

### Q: "Walk me through your project architecture."

> It follows **Clean Architecture** with 4 layers:
>
> - **Domain** — Contains business models (`DashboardCard`, `OrderModel`), interfaces (`IDashboardDataService`), and the `Result<T>` pattern. Zero dependencies on any other layer.
> - **Infrastructure** — Data service implementations (`StaticDashboardDataService` for in-memory, `ApiDashboardDataService` for HTTP). Depends only on Domain.
> - **Core** — ViewModels and value converters. Depends on Domain and Infrastructure interfaces.
> - **Presentation** — XAML pages, reusable ContentView components, and style resources. Depends only on Core.
>
> The rule is: dependencies flow inward. Presentation → Core → Infrastructure → Domain. Domain knows nothing about UI or data access.

### Q: "Why did you choose Clean Architecture over a simpler structure?"

> **Three reasons:**
> 1. **Swapability** — The `IDashboardDataService` interface means I can switch from static data to a live API by changing one line in `MauiProgram.cs`. The ViewModels never know the difference.
> 2. **Testability** — I can mock `IDashboardDataService` in unit tests and test ViewModel logic without touching a database or network.
> 3. **Interview signal** — It demonstrates I understand enterprise patterns. For a production app, this structure scales to teams where different developers own different layers.

### Q: "What would you change if this were a production app with 50+ screens?"

> I'd introduce:
> - **Feature folders** — Group each feature (e.g., `Orders/`, `Analytics/`) with its own View, ViewModel, and local models instead of one flat folder per concern.
> - **Navigation service** — Abstract `Shell.Current.GoToAsync` behind an `INavigationService` so ViewModels can be unit-tested without a real Shell.
> - **Mediator pattern** — Use `WeakReferenceMessenger` (from CommunityToolkit.Mvvm) for cross-ViewModel communication (e.g., "order deleted" → refresh sidebar badge).
> - **Code generation** — Use `INotifyPropertyChanged` source generators or T4 templates for repetitive ViewModel properties.

### Q: "How did you decide which components to split into separate ContentViews?"

> I split when a section is:
> 1. **Visually distinct** — A card, chart, or table that looks self-contained on screen.
> 2. **Reusable** — `SummaryCardView` appears 4 times on the dashboard with different bindings.
> 3. **Complex on its own** — `OrderTableView` has search, pagination, sorting logic — it deserves its own file.
> 4. **Change-prone** — If the design changes the sidebar, I touch `SidebarView.xaml`, not `MainPage.xaml`.
>
> In total, 8 components. If a component has fewer than 20 lines of XAML and isn't reused, I keep it inline.

---

## 2. MVVM & Data Binding

### Q: "Explain how data flows from the service to the UI."

> ```
> IDashboardDataService.GetDashboardCardsAsync()
>     → Returns Result<IReadOnlyList<DashboardCard>>
>     → MainViewModel checks IsSuccess
>     → Populates ObservableCollection<DashboardCard>
>     → XAML {Binding DashboardCards} via compiled bindings
> ```
>
> All 5 data sources load in **parallel** using `Task.WhenAll`. Each method handles its own error — one failing doesn't block the others. The XAML uses `x:DataType` for compile-time binding validation.

### Q: "Why CommunityToolkit.Mvvm instead of manual INotifyPropertyChanged?"

> **Source generators.** `[ObservableProperty]` generates the `PropertyChanged` event, `SetProperty` call, and `OnXChanged` partial method — at compile time. `[RelayCommand]` generates the `ICommand` from an async method, including `CanExecute` and `NotifyCanExecuteChanged`.
>
> No runtime reflection. No boilerplate. The BaseViewModel is 12 lines of code.

### Q: "What's the difference between `ObservableCollection` and `List` in this context?"

> `ObservableCollection` fires `CollectionChanged` when items are added/removed, which MAUI's `CollectionView` and `BindableLayout` listen to automatically. A `List` would require manually re-assigning the binding source and calling `OnPropertyChanged` — causing the entire list to re-render.
>
> So for live data that changes: `ObservableCollection`. For static lookup data: `List` or array.

### Q: "How do compiled bindings improve performance?"

> With `x:DataType="vm:MainViewModel"`, MAUI generates IL code for property paths at compile time instead of using reflection at runtime. This means:
> - Faster binding resolution (~5-10x)
> - Type-checking at build time (a typo in a binding path breaks the build)
> - Less memory from no string-based property lookups

### Q: "How would you handle a real-time data feed (e.g., WebSocket)?"

> I'd create a `RealtimeDashboardDataService : IDashboardDataService` that internally uses a WebSocket client. When new data arrives:
> 1. It raises an event or calls a callback.
> 2. The ViewModel subscribes in the constructor and updates the relevant `ObservableCollection`.
> 3. The UI auto-updates from the binding.
>
> Alternatively, use `WeakReferenceMessenger` to publish a `DataUpdatedMessage` from the service and have the ViewModel receive it without a direct coupling.

---

## 3. MAUI Framework & XAML

### Q: "What MAUI features did you use that don't exist in Xamarin.Forms?"

> - **Compiled bindings** (`x:DataType`) — runtime binding validation.
> - **WinUI 3 backend** — better Windows performance and native window management (maximize on launch).
> - **Single project** — no separate `.Forms` project needed.
> - **`BlazorWebView`** (not used here but available) — embed Blazor components in MAUI.
> - **`GraphicsView` + `IDrawable`** — custom 2D rendering for the donut chart and spline chart.

### Q: "How did you implement the charts without a third-party library?"

> The **SplineChartView** and **TrafficChartView** use `GraphicsView` and custom `IDrawable` implementations. This avoids adding NuGet dependencies like Microcharts or LiveCharts. The `IDrawable.Draw(ICanvas)` method receives a canvas where I draw:
> - Paths/lines using `canvas.StrokeColor` / `canvas.DrawPath`
> - Filled segments for the donut chart using `canvas.FillArc`
>
> For production apps, I'd evaluate whether a library provides more features (animations, tooltips, interactions) before rolling custom.

### Q: "How are styles organized and applied?"

> In `Colors.xaml`: 20+ named colors with hex values.
> In `Styles.xaml`: Implicit styles for `Label`, `Frame`, `Button`, etc.
> In `Fonts.xaml`: Font family registrations.
>
> Styles are applied implicitly: every `<Label>` in the app automatically gets the `Label` style. Components can override via `Style="{StaticResource ...}"` for special cases.

### Q: "How does MAUI handle platform-specific code?"

> Three mechanisms:
> 1. **Conditional compilation** — `#if WINDOWS` / `#if ANDROID` for platform-specific logic (e.g., maximize window on Windows).
> 2. **Platform folders** — `Platforms/Windows/`, `Platforms/Android/` for startup code, permissions, etc.
> 3. **`OnPlatform` / `OnIdiom` in XAML** — `FontSize="{OnPlatform Default=16, Android=14}"` for per-platform adjustments.

---

## 4. Dependency Injection

### Q: "Explain your DI container setup."

> All in `MauiProgram.cs`:
> ```csharp
> builder.Services.AddSingleton<IDashboardDataService, StaticDashboardDataService>();
> builder.Services.AddSingleton<MainViewModel>();
> builder.Services.AddSingleton<MainPage>();
> builder.Services.AddTransient<DetailViewModel>();
> builder.Services.AddTransient<DetailPage>();
> ```
>
> - **Singletons**: Data service (one data source), MainViewModel and MainPage (one dashboard session).
> - **Transients**: DetailPage gets a fresh instance on each navigation.
>
> Constructor injection everywhere — no service locator pattern.

### Q: "Why is constructor injection better than the Service Locator pattern?"

> 1. **Explicit dependencies** — The constructor signature tells you exactly what a class needs.
> 2. **Testability** — You can pass mocks directly in unit tests.
> 3. **No hidden coupling** — Service locator (`App.Current.MainPage` or `IoC.Resolve`) hides dependencies making the code harder to understand and refactor.
> 4. **Container-agnostic** — The class doesn't reference Microsoft.Extensions.DI; it just has constructor parameters.

### Q: "What lifetime would you use for a database context?"

> For SQLite with MAUI: **`AddSingleton`** is fine because SQLite connections are thread-safe and MAUI apps are single-process. For Entity Framework Core or other ORMs: **`AddDbContext`** (scoped per request) is typical. Since there's no HTTP request scope in MAUI, scoped effectively becomes per-navigation or per-operation.

### Q: "How do you handle 'I need a parameter at runtime' (e.g., user ID from login)?"

> Option A: **Factory pattern** — Register `Func<YourParam, YourService>` and use the factory in the ViewModel.
> Option B: **Navigation parameters** — Pass data via Shell query parameters (`[QueryProperty]`) or `IQueryAttributable`.
> Option C: **Service with state** — The logged-in user info can be a singleton `UserSession` that gets set on login and injected where needed.
>
> In this app, I use `[QueryProperty]` to pass the section title to `DetailViewModel`.

---

## 5. Real API Integration (No SQLite)

> **This project does NOT use SQLite.** Data comes from a service layer. Here's how real API integration works:

### Q: "How would you connect this dashboard to a real backend API?"

> **Step 1: Define the contract**
> `IDashboardDataService` already defines all 5 methods. The API service implementation (`ApiDashboardDataService`) uses `HttpClient` via `System.Net.Http.Json`:
>
> ```csharp
> public class ApiDashboardDataService : IDashboardDataService
> {
>     private readonly HttpClient _http;
>
>     public ApiDashboardDataService(HttpClient http)
>     {
>         _http = http;
>     }
>
>     public async Task<Result<IReadOnlyList<DashboardCard>>> GetDashboardCardsAsync()
>     {
>         try
>         {
>             var data = await _http.GetFromJsonAsync<List<DashboardCard>>("api/dashboard/cards");
>             return Result<IReadOnlyList<DashboardCard>>.Success(data ?? []);
>         }
>         catch (Exception ex)
>         {
>             return Result<IReadOnlyList<DashboardCard>>.Failure($"Failed: {ex.Message}");
>         }
>     }
> }
> ```

> **Step 2: Register in DI**
> Swap the static service for the API service:
> ```csharp
> builder.Services.AddSingleton<IDashboardDataService>(sp =>
> {
>     var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };
>     return new ApiDashboardDataService(client);
> });
> ```

> **Step 3: Configure HttpClient**
> For production, use `IHttpClientFactory` with Polly retry policies:
> ```csharp
> builder.Services.AddHttpClient<IDashboardDataService, ApiDashboardDataService>(client =>
> {
>     client.BaseAddress = new Uri("https://api.example.com/");
> })
> .AddTransientHttpErrorPolicy(policy =>
>     policy.WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(Math.Pow(2, retry))));
> ```

### Q: "What endpoints does your API service expect?"

> | Method | Endpoint | Returns |
> |--------|----------|---------|
> | `GetDashboardCardsAsync` | `GET /api/dashboard/cards` | `DashboardCard[]` |
> | `GetActivitiesAsync` | `GET /api/dashboard/activities` | `ActivityModel[]` |
> | `GetOrdersAsync` | `GET /api/dashboard/orders` | `OrderModel[]` |
> | `GetTrafficSourcesAsync` | `GET /api/dashboard/traffic` | `TrafficModel[]` |
> | `GetSalesDataAsync` | `GET /api/dashboard/sales` | `SalesData[]` |

### Q: "How do you handle loading states and errors from the API?"

> 1. **Loading**: `IsBusy` flag in `BaseViewModel` guards against double-taps and can show a `ActivityIndicator`.
> 2. **Success**: Populate `ObservableCollection` → UI updates.
> 3. **Failure**: `Result<T>.Failure` → ViewModel skips that section (other sections still render). User can tap Refresh to retry.
> 4. **Empty**: Return `Success` with an empty list → UI shows "No data" state.

### Q: "Why not use SQLite for this project?"

> The data is **read-only from an external source** (dashboard metrics from a backend). SQLite would be used if:
> - We needed **offline-first** with local storage synced to server.
> - The data was **user-generated** (orders, notes) that must persist locally.
> - We had **complex local queries** across related tables.
>
> For a dashboard that displays API data, SQLite adds complexity (migrations, sync logic, stale data handling) with no benefit. The `Result<T>` pattern handles transient API failures gracefully.

### Q: "If you DID use SQLite, how would you integrate it?"

> I'd use `sqlite-net-pcl`:
> ```csharp
> public class SqliteDashboardDataService : IDashboardDataService
> {
>     private readonly SQLiteAsyncConnection _db;
>
>     public SqliteDashboardDataService(string dbPath)
>     {
>         _db = new SQLiteAsyncConnection(dbPath);
>         _db.CreateTableAsync<DashboardCard>();
>     }
>
>     public async Task<Result<IReadOnlyList<DashboardCard>>> GetDashboardCardsAsync()
>     {
>         var data = await _db.Table<DashboardCard>().ToListAsync();
>         return Result<IReadOnlyList<DashboardCard>>.Success(data);
>     }
> }
> ```
> Then register: `builder.Services.AddSingleton<IDashboardDataService>(new SqliteDashboardDataService(dbPath))`.
>
> The key: the **interface stays the same**. ViewModels never know the source changed from static → API → SQLite. That's the power of Clean Architecture.

### Q: "How would you implement caching for the API data?"

> Two approaches:
>
> **Option A: In-memory cache in the service**
> ```csharp
> public class CachedDashboardDataService : IDashboardDataService
> {
>     private readonly IDashboardDataService _inner;
>     private readonly ConcurrentDictionary<string, object> _cache = new();
>     private DateTime _lastFetch = DateTime.MinValue;
>     private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);
>
>     public async Task<Result<IReadOnlyList<DashboardCard>>> GetDashboardCardsAsync()
>     {
>         if (DateTime.UtcNow - _lastFetch < Ttl && _cache.TryGetValue("cards", out var cached))
>             return Result<IReadOnlyList<DashboardCard>>.Success((IReadOnlyList<DashboardCard>)cached);
>
>         var result = await _inner.GetDashboardCardsAsync();
>         if (result.IsSuccess) { _cache["cards"] = result.Data!; _lastFetch = DateTime.UtcNow; }
>         return result;
>     }
> }
> ```
> Register as `builder.Services.AddSingleton<IDashboardDataService, CachedDashboardDataService>()` wrapping the real API service — **decorator pattern**.
>
> **Option B: Pull-to-refresh** — The existing `RefreshCommand` already clears the data-loaded flag and reloads. With API, user pulls to refresh.

### Q: "How do you handle authentication with the API?"

> 1. Create a `TokenService` that stores the JWT/access token.
> 2. Use `DelegatingHandler` to attach the token to every request:
> ```csharp
> public class AuthHandler : DelegatingHandler
> {
>     private readonly TokenService _token;
>     public AuthHandler(TokenService token) => _token = token;
>
>     protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
>     {
>         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _token.GetTokenAsync());
>         return await base.SendAsync(request, ct);
>     }
> }
> ```
> 3. Register: `builder.Services.AddTransient<AuthHandler>()`
> 4. HttpClient factory includes the handler in the pipeline.

---

## 6. Error Handling & Result Pattern

### Q: "Why not just use try/catch everywhere?"

> Exceptions are **expensive** — they unwind the stack and create heap allocations. More importantly, they're **invisible** in the method signature. A method that throws is a hidden contract.
>
> `Result<T>` makes failure part of the **type system**:
> ```csharp
> Task<Result<IReadOnlyList<OrderModel>>> GetOrdersAsync();
> ```
> The caller sees it can fail and must handle both paths. No surprise exceptions.

### Q: "What happens when the static data service throws?"

> The static service uses `Task.FromResult(Result<T>.Success(...))` — it never throws. The `ApiDashboardDataService` catches exceptions inside the method and returns `Result.Failure`. The ViewModel checks `IsSuccess`:
> ```csharp
> var result = await _dataService.GetOrdersAsync();
> if (result.IsFailure) return;  // Skip silently, other sections still show
> ```
> In production, you'd log the error and optionally show a toast/banner.

### Q: "How would you show error messages to users?"

> I'd add a `ErrorMessage` observable property to `BaseViewModel`:
> ```csharp
> [ObservableProperty] private string _errorMessage = string.Empty;
> ```
> On failure, set it. In XAML, bind a banner:
> ```xml
> <Label Text="{Binding ErrorMessage}" IsVisible="{Binding ErrorMessage, Converter={StaticResource StringNotEmptyConverter}}" />
> ```
> The user sees the error, hits Refresh, and clears it.

---

## 7. Performance & Optimization

### Q: "How did you optimize the dashboard loading?"

> 1. **Parallel fetch** — `Task.WhenAll` loads all 5 data sources simultaneously.
> 2. **IsBusy guard** — Prevents double-tap on load/refresh.
> 3. **IsDataLoaded flag** — `InitializeAsync` only loads once, not on every page appearing.
> 4. **Compiled bindings** — `x:DataType` avoids runtime reflection in bindings.
> 5. **ObservableCollection** — Only updated items re-render, not the entire list.

### Q: "What would you do if the dashboard was slow to render?"

> 1. **Lazy loading** — Load visible sections first (header + KPI cards), then charts, then the table.
> 2. **Virtualization** — `CollectionView` with `ItemsLayout="VerticalList"` virtualizes long lists. The order table uses pagination (5 per page) instead of rendering all 30.
> 3. **Image optimization** — Use `Resize=True` on `MauiImage`.
> 4. **Reduce visual tree depth** — Deeply nested layouts are expensive. Flatten Grid/FlexLayout where possible.
> 5. **Profile with MAUI Tools** — Use Hot Reload and the MAUI Profiler to find rendering bottlenecks.

### Q: "How does MAUI handle 10,000 orders in the table?"

> **It won't render 10,000 items directly.** The order table already has **pagination** — 5 items per page. At 30 items, it's fine. For 10,000:
> - Keep pagination (server-side if API-backed)
> - Or use `CollectionView` with `RemainingItemsThreshold` for infinite scroll
> - Never use `StackLayout` inside `ScrollView` for large lists — use `CollectionView`

---

## 8. Testing

### Q: "How would you unit test MainViewModel?"

> Since `MainViewModel` depends on `IDashboardDataService` interface, I mock it:
> ```csharp
> [Test]
> public async Task LoadData_PopulatesCards()
> {
>     var mock = new Mock<IDashboardDataService>();
>     mock.Setup(s => s.GetDashboardCardsAsync())
>         .ReturnsAsync(Result<IReadOnlyList<DashboardCard>>.Success([
>             new() { Title = "Test", Amount = 100 }
>         ]));
>     // Set up remaining 4 methods...
>
>     var vm = new MainViewModel(mock.Object);
>     await vm.InitializeAsync();
>
>     Assert.That(vm.DashboardCards.Count, Is.EqualTo(1));
>     Assert.That(vm.DashboardCards[0].Title, Is.EqualTo("Test"));
> }
> ```
>
> No real services. No UI. Pure logic test in milliseconds.

### Q: "What about UI testing?"

> For MAUI, use `Microsoft.Maui.Testing` or Appium:
> ```csharp
> [Test]
> public void Dashboard_Loads_Cards()
> {
>     app.WaitForElement("WalletCard");
>     var card = app.FindElement("WalletCard");
>     Assert.IsTrue(card.Displayed);
> }
> ```
> In CI, this runs on a Windows agent (for Windows target) or Android emulator.

---

## 9. Component Design & Reusability

### Q: "How did you make SummaryCardView reusable?"

> It has **bindable properties**:
> ```xml
> <!-- SummaryCardView.xaml -->
> <ContentView>
>     <Frame>
>         <Label Text="{Binding Title}" />
>         <Label Text="{Binding Amount}" />
>     </Frame>
> </ContentView>
> ```
> Usage: `<components:SummaryCardView Title="Wallet" Amount="$4,567.53" />`
>
> The component doesn't know what data it displays — it receives it through bindings. The same view renders Wallet, Referral, Sales, and Earning cards with different data.

### Q: "How would you add a new component?"

> 1. Create `NewComponentView.xaml` + `.xaml.cs` in `Presentation/Components/`
> 2. Define `BindableProperty` for data input
> 3. Add styles in `Styles.xaml`
> 4. Place it in `MainPage.xaml` and bind to the ViewModel property

### Q: "Why use BindableProperty instead of {Binding} to ViewModel?"

> `BindableProperty` makes a component **self-contained and reusable**. The component doesn't know where the data comes from — it just exposes a contract. When `SummaryCardView` is used 4 times, each instance binds to a different source (`WalletCard`, `ReferralCard`, etc.) without the component caring.

---

## 10. Responsiveness & Cross-Platform

### Q: "How does your app handle different screen sizes?"

> Currently the sidebar is fixed at 200px and the layout uses a `Grid` with `ColumnDefinitions="200,*"`. For responsiveness:
> - `OnIdiom` in XAML adjusts sizes for Phone/Tablet/Desktop
> - On narrow screens, the sidebar could collapse to an overlay (hamburger menu)
> - The `ScrollView` ensures content scrolls vertically if too tall

### Q: "How would you make it adaptive for mobile?"

> ```csharp
> if (DeviceInfo.Idiom == DeviceIdiom.Phone)
> {
>     SidebarWidth = 0;  // Hidden
>     ShowHamburger = true;
>     GridColumns = "*"; // Single column
> }
> ```
> The ViewModel checks `DeviceInfo.Idiom` on init and sets `ColumnDefinitions` accordingly via binding.

### Q: "What MAUI challenges did you encounter?"

> - **Windows maximize on launch** — Requires WinRT interop (`Microsoft.UI.Windowing.AppWindow`), which only works on Windows. Guarded with `#if WINDOWS`.
> - **Compiled bindings** — `x:DataType` requires discipline. Forgetting it means fallback to reflection, which is slower and catches binding errors at runtime.
> - **Layout cycles** — Deeply nested layouts can cause multiple measure passes. Flattening helps.

---

## 11. Mock Interview — Walkthrough Script

> Practice this out loud. It covers the full 10-minute walkthrough.

**"Let me show you the dashboard application I built with .NET MAUI."**

**[RUN THE APP — launch the dashboard]**

"As the app launches, you see the dashboard with a sidebar, header, KPI cards, charts, a timeline, and an order table. Let me walk through the architecture."

**[OPEN THE SOLUTION FOLDER in VS]**

"The project uses Clean Architecture with 4 layers. Here in `Domain/` I have my business models — `DashboardCard`, `OrderModel`, `ActivityModel`, etc. — plus the `IDashboardDataService` interface and `Result<T>` pattern. The Domain has zero dependencies."

"In `Infrastructure/`, I have two implementations of that interface: `StaticDashboardDataService` which returns hardcoded in-memory data, and `ApiDashboardDataService` which makes HTTP calls to a backend."

"`Core/` has all my ViewModels and value converters. They depend only on the Domain interface, not on any concrete data source. That means I can swap between static data and a real API by changing one line in `MauiProgram.cs`."

"`Presentation/` has the XAML pages, 8 reusable ContentView components, and the style resources. The components use `BindableProperty` so they're fully reusable — `SummaryCardView` is used 4 times with different data."

**[POINT TO THE RUNNING APP]**

"You can see the sidebar navigation — clicking a menu item navigates to a detail page. The order table has search, pagination, add, delete, and a summary dialog. All data loads in parallel via `Task.WhenAll`."

"The charts are custom — built with `GraphicsView` and `IDrawable`, avoiding third-party chart libraries."

"For a real backend, I'd swap the DI registration to use `ApiDashboardDataService` with `IHttpClientFactory` and Polly retry policies. The `Result<T>` pattern means every service call explicitly handles success and failure — no unhandled exceptions. The same architecture works with SQLite too if needed — just add a `SqliteDashboardDataService` implementing the same interface."

"That's the overview. Happy to dive deeper into any component or pattern."

---

## Quick Reference: Key Talking Points

| Topic | Key phrase to say |
|-------|------------------|
| Architecture | "Dependency inversion — UI talks to interface, not implementation" |
| MVVM | "Source-generated via CommunityToolkit.Mvvm — no boilerplate" |
| DI | "Constructor injection — explicit dependencies, no service locator" |
| API integration | "Swap one line in MauiProgram — ViewModels never change" |
| No SQLite | "Data is from external API — SQLite adds complexity with no benefit for read-only dashboard data" |
| Error handling | "Result<T> makes failure paths explicit in the type system" |
| Components | "BindableProperty for reusable self-contained views" |
| Charts | "Custom GraphicsView + IDrawable — no third-party dependency" |
| Performance | "Parallel fetch, compiled bindings, pagination, IsBusy guard" |
| Testing | "Mock IDashboardDataService — unit test ViewModels without UI" |
