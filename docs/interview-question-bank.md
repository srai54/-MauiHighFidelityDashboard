# Interview Question Bank — 207 Questions

Two parts, every question with a concise answer:

- **Part 1 — 100 generic .NET MAUI questions** (fundamentals → performance)
- **Part 2 — 107 project-specific questions** about this dashboard's actual code (MVVM, `ObservableCollection`, services, custom charts, popups, responsive layout)

---

# Part 1 — Generic .NET MAUI (Q1–Q100)

## A. Fundamentals (Q1–Q10)

### 1. What is .NET MAUI?
**A:** .NET Multi-platform App UI — a cross-platform framework that builds native apps for Android, iOS, macOS, and Windows from one C#/XAML codebase, compiling to real native controls on each platform.

### 2. How does MAUI differ from Xamarin.Forms?
**A:** MAUI is its evolution: single project instead of one-per-platform, handler architecture instead of renderers, unified resource management (one icon/splash SVG), `MauiProgram` host builder with built-in DI, and it ships as a .NET workload.

### 3. What is the "single project" concept?
**A:** One `.csproj` multi-targets all platforms (`net10.0-android`, `net10.0-windows...`), holding shared code plus a `Platforms/` folder where platform-specific files are compiled only for their target.

### 4. What are handlers?
**A:** The bridge between a cross-platform control (`Button`) and its native counterpart (`AppCompatButton`, WinUI `Button`). Each handler has a property *mapper* you can append to, replacing the old renderer-subclassing model with lightweight per-property customization.

### 5. What is `MauiProgram.CreateMauiApp()`?
**A:** The composition root: it builds a `MauiApp` via `MauiApp.CreateBuilder()`, registering the `App`, fonts, third-party toolkits, services in the DI container, and platform lifecycle tweaks — everything the app needs before the first page shows.

### 6. What platforms and OS versions can MAUI target?
**A:** Android 5+ (API 21), iOS/Mac Catalyst 11+/15+ (per template), Windows 10 1809+, plus community Tizen; each project sets floors via `SupportedOSPlatformVersion`.

### 7. What is the relationship between .NET MAUI and .NET?
**A:** MAUI is a workload on top of the base .NET SDK — you get the full BCL, C# language versions, NuGet ecosystem, and one runtime story (Mono-based on mobile, CoreCLR on desktop).

### 8. What is `App` vs `AppShell` vs `MainPage`?
**A:** `App` (Application subclass) owns windows and global resources; `AppShell` defines the navigation structure (routes, flyout/tabs); pages are the actual screens hosted inside that structure.

### 9. What is XAML's role in MAUI, and is it mandatory?
**A:** XAML declaratively describes UI and is compiled (XAMLC) into IL; it's optional — the same UI can be built in pure C#, and community "Markup" extensions make fluent C# UI ergonomic.

### 10. What happens when you build a MAUI project for two targets?
**A:** MSBuild runs once per TFM, producing independent outputs: an APK/AAB for Android and a WinUI exe for Windows — shared code compiles into each with platform-conditional pieces included per target.

## B. App Startup & Lifecycle (Q11–Q20)

### 11. Describe the app startup sequence.
**A:** Platform entry point → `MauiProgram.CreateMauiApp()` builds the host and DI container → `App` is created → a `Window` with the root page (often `AppShell`) is created → the first page's constructor, then `OnAppearing`, fire.

### 12. What lifecycle events does a MAUI `Window` expose?
**A:** Created, Activated/Deactivated, Stopped/Resumed, Destroying — a cross-platform abstraction over Android activity states and Windows window events.

### 13. How do you hook raw platform lifecycle events?
**A:** `builder.ConfigureLifecycleEvents(events => events.AddWindows/AddAndroid(...))` — e.g. grabbing the WinUI window on `OnWindowCreated` to maximize it or remove the title bar.

### 14. `OnAppearing` vs the page constructor — when do you use which?
**A:** The constructor runs once for object creation (InitializeComponent, wiring); `OnAppearing` runs every time the page becomes visible — the right place to load/refresh data, guarded so it doesn't reload needlessly.

### 15. Why is `async void OnAppearing` tolerated but `async void` generally avoided?
**A:** Event handlers/overrides must be `void`, so `async void` is the accepted pattern there; the risk (unobserved exceptions, no awaiting) is contained by keeping the body a guarded call into awaitable `async Task` methods.

### 16. What is the generic host / DI container in MAUI?
**A:** The same `Microsoft.Extensions.DependencyInjection` used by ASP.NET Core: `builder.Services.AddSingleton/AddTransient/AddScoped`, with constructor injection into pages and view models that are themselves registered.

### 17. Singleton vs Transient for pages and view models?
**A:** Singleton keeps one instance (state survives navigation — good for a main dashboard); transient creates a fresh instance per resolve (good for detail pages parameterized per visit). Scoped has little meaning in a desktop/mobile app.

### 18. How do fonts get registered and used?
**A:** `builder.ConfigureFonts(fonts => fonts.AddFont("file.ttf", "Alias"))`, files live in `Resources/Fonts`; XAML uses `FontFamily="Alias"` — including icon fonts like Font Awesome where glyphs are text (`&#xf078;`).

### 19. What is a splash screen in MAUI and how is it generated?
**A:** `<MauiSplashScreen>` points at one SVG + background color; the build generates each platform's native splash (Android 12 splash API, Windows splash) — shown until the first frame renders.

### 20. How would you run startup work in parallel?
**A:** Kick off independent async loads and `await Task.WhenAll(...)` — e.g. loading cards, orders, and chart data concurrently instead of sequentially, cutting time-to-interactive.

## C. XAML & Layouts (Q21–Q30)

### 21. Name MAUI's main layout containers and when to pick each.
**A:** `Grid` (rows/columns, most control, best performance for complex forms), `VerticalStackLayout`/`HorizontalStackLayout` (simple linear flows), `FlexLayout` (wrapping/space distribution), `AbsoluteLayout` (proportional/manual positioning).

### 22. What's the difference between `StaticResource` and `DynamicResource`?
**A:** `StaticResource` resolves once at load and never re-evaluates; `DynamicResource` keeps a live link and updates the property if the resource changes at runtime (themes) — at a lookup cost.

### 23. What is `x:Static` used for?
**A:** Referencing a C# `static`/`const` member directly from XAML (`{x:Static charts:ChartTheme.SalesOnline}`) — resolved at compile time, ideal for sharing constants between code and markup without duplicating values.

### 24. What are `ColumnDefinitions="Auto,*,2*"` sizes?
**A:** `Auto` sizes to content, `*` shares remaining space proportionally (`2*` gets twice one `*`), and fixed numbers are device-independent units.

### 25. What is XAML compilation (XAMLC) and its benefits?
**A:** XAML compiles to IL at build time instead of being parsed at runtime — faster inflation, smaller memory, and compile-time errors for typos in properties/markup.

### 26. What is a `ResourceDictionary` and merged dictionaries?
**A:** A keyed bag of reusable resources (colors, styles, templates); `App.xaml` merges dictionaries like `Colors.xaml` and `Styles.xaml` so every page can use them via `StaticResource`.

### 27. Implicit vs explicit styles?
**A:** An explicit style has `x:Key` and is applied by name; an implicit style has only `TargetType` and applies automatically to all instances of that type in scope.

### 28. What are triggers and visual states?
**A:** Triggers change properties declaratively on conditions (property/data/event triggers); the VisualStateManager groups state-based setters (Normal/PointerOver/Disabled) — both reduce code-behind for UI state.

### 29. `ContentPage` vs `ContentView`?
**A:** `ContentPage` is a navigable screen; `ContentView` is a reusable component you embed inside pages — the building block for custom composite controls with bindable properties.

### 30. How does `BindableLayout` differ from `CollectionView`?
**A:** `BindableLayout` stamps a template per item into any layout (no virtualization — fine for small lists like a legend); `CollectionView` virtualizes, scrolls, selects — for large data sets.

## D. Data Binding (Q31–Q42)

### 31. What is `BindingContext` and how does it propagate?
**A:** The object a binding resolves paths against; it flows down the visual tree, so setting it once on a page gives every child the same view model unless overridden.

### 32. Explain binding modes.
**A:** `OneWay` (source→target, default for most), `TwoWay` (both directions — inputs like `Entry.Text`), `OneTime` (once, no tracking), `OneWayToSource` (target→source).

### 33. What must a source object do for `OneWay` updates to appear?
**A:** Implement `INotifyPropertyChanged` and raise `PropertyChanged` with the property name — bindings subscribe and refresh the target when it fires.

### 34. What does `ObservableCollection<T>` add over `List<T>` in bindings?
**A:** It implements `INotifyCollectionChanged`, so item adds/removes/clears automatically update any bound `ItemsSource` UI without resetting the whole list.

### 35. When does replacing an `ObservableCollection` instance break bindings?
**A:** If the property doesn't raise `PropertyChanged`, the UI keeps watching the old instance; the common patterns are a read-only collection you `Clear()`+`Add()` into, or raising change notification on replacement.

### 36. What is `x:DataType` (compiled bindings)?
**A:** Declaring the binding context's type on a template/page lets the compiler generate strongly-typed binding code — compile-time path checking and much faster than reflection-based bindings.

### 37. What is a value converter?
**A:** An `IValueConverter` with `Convert`/`ConvertBack` that transforms data between source and target — e.g. a hex string to a `Color`, or a status string to a background brush — registered as a resource and referenced in the binding.

### 38. What is `StringFormat` in a binding?
**A:** Inline formatting without a converter: `{Binding Percentage, StringFormat='{0}%'}` — applies `string.Format` to the bound value.

### 39. How do you bind to another element instead of the BindingContext?
**A:** `{Binding Prop, Source={x:Reference SomeElement}}` — commonly `Source={x:Reference Root}` inside a `ContentView` to bind to its own bindable properties.

### 40. What is a fallback/`TargetNullValue` and when do bindings silently fail?
**A:** Bindings fail silently (debug output only) when the path doesn't resolve; `FallbackValue`/`TargetNullValue` supply display values for failure/null — useful while async data hasn't arrived.

### 41. What is `RelativeSource` binding?
**A:** Binding relative to the target's position — `AncestorType` walks up the tree (e.g. an item template reaching the page's view model to find a command), `Self` binds to the element itself.

### 42. How do you bind a command inside a `DataTemplate` to the page's view model?
**A:** Either `RelativeSource AncestorType={x:Type vm:MainViewModel}}` or `Source={x:Reference PageName}` with `Path=BindingContext.Command`, passing the item as `CommandParameter`.

## E. MVVM & CommunityToolkit.Mvvm (Q43–Q52)

### 43. Why MVVM in MAUI?
**A:** It separates view (XAML), presentation logic (view model), and data (model), making logic unit-testable without UI, enabling designer/developer parallelism, and matching MAUI's binding engine.

### 44. What does `ObservableObject` give you?
**A:** A base `INotifyPropertyChanged` implementation with `SetProperty` helpers and `OnPropertyChanged`, removing hand-written boilerplate.

### 45. What does `[ObservableProperty]` generate?
**A:** From a private field, the source generator emits a public property with change notification plus partial hook methods like `OnXxxChanged(value)` you can implement for side effects.

### 46. What does `[RelayCommand]` generate?
**A:** An `ICommand` property wrapping the method — async methods produce `IAsyncRelayCommand` with built-in execution tracking; optional `CanExecute` wiring disables buttons automatically.

### 47. What is `ICommand` and why do views prefer commands over event handlers?
**A:** An abstraction with `Execute`/`CanExecute` that views bind to; it keeps behavior in the view model (testable, reusable) instead of code-behind, and centralizes enable/disable logic.

### 48. How do view models usually receive dependencies?
**A:** Constructor injection: the DI container resolves registered services (`IDashboardDataService`) when constructing the view model, which is itself resolved when constructing the page.

### 49. What is the "IsBusy guard" pattern?
**A:** A boolean flag checked at command entry (`if (IsBusy) return;`) and toggled around the work — prevents re-entrant execution (double taps) and drives loading indicators.

### 50. How do you communicate between view models?
**A:** Loosely via a messenger (`WeakReferenceMessenger` publish/subscribe), or by sharing an injected service that holds shared state; avoid direct references between view models.

### 51. What are partial methods in the MVVM toolkit's generated code?
**A:** Hooks like `partial void OnSearchTextChanged(string value)` the generator declares and calls; implementing one runs logic on every property change without overriding the setter manually.

### 52. Why do source generators require `partial` classes?
**A:** Generators add code in a second file for the same type; only `partial` classes can be split across files, letting generated members merge with yours at compile time.

## F. Navigation (Q53–Q60)

### 53. What is Shell navigation?
**A:** URI-style routing (`await Shell.Current.GoToAsync("detail?title=Widgets")`) over a declared visual hierarchy, with route registration, query parameters, and back-stack semantics (`".."` to pop).

### 54. How do query parameters reach the target page?
**A:** Via `[QueryProperty]` attributes or `IQueryAttributable.ApplyQueryAttributes` on the page/view model — Shell parses the query string and injects values after navigation.

### 55. How do you pass a complex object during navigation?
**A:** `GoToAsync` overload with an `IDictionary<string, object>` navigation parameter — objects pass by reference through `ApplyQueryAttributes`, avoiding string serialization.

### 56. Absolute vs relative Shell routes?
**A:** Relative routes (`"detail"`) push onto the current stack; absolute (`"//main"`) reset to a declared root — `//` jumps affect the whole stack rather than stacking a new page.

### 57. How do you intercept or cancel back navigation?
**A:** Override `OnBackButtonPressed` (hardware/back UI) or Shell's `OnNavigating` with `Deferral`/cancel — e.g. to confirm discarding unsaved edits.

### 58. What is modal navigation?
**A:** Presenting a page over the stack without Shell chrome (`Navigation.PushModalAsync` or Shell presentation mode) — for flows that must complete/cancel rather than freely navigate away.

### 59. Shell vs `NavigationPage` — when would you skip Shell?
**A:** Shell suits app-wide flyout/tab structures with deep links; plain `NavigationPage` gives finer manual control for simple stacks or when Shell's structure/URI model doesn't fit.

### 60. How does dependency injection interact with Shell navigation?
**A:** Pages registered in DI (with their view models) are resolved by Shell when navigating if registered via routing + DI-aware factories; otherwise Shell instantiates via the default constructor.

## G. Controls & UI Patterns (Q61–Q70)

### 61. `CollectionView` vs `ListView`?
**A:** `CollectionView` is the modern replacement: better virtualization, flexible layouts (grid/horizontal), `EmptyView`, selection modes, no mandatory cells — `ListView` remains only for legacy.

### 62. What is a `DataTemplate` and a `DataTemplateSelector`?
**A:** A template stamps UI per data item; a selector returns different templates per item at runtime (e.g. different bubble layouts for sent vs received messages).

### 63. What is a `Border` vs `Frame`?
**A:** `Border` is the modern container for stroke/corner shape (`StrokeShape="RoundRectangle 6"`) with better performance; `Frame` is legacy with shadow baked in.

### 64. What is `GraphicsView`?
**A:** A canvas control that renders an `IDrawable` via `Microsoft.Maui.Graphics` — you implement `Draw(ICanvas, RectF)` for fully custom, cross-platform 2D drawing (charts, gauges), calling `Invalidate()` to redraw.

### 65. What are gesture recognizers?
**A:** Attachable objects (`TapGestureRecognizer`, swipe, pan, pinch, pointer) that add interaction to any view — e.g. making a legend row tappable without needing a Button.

### 66. How do tooltips work on desktop targets?
**A:** `ToolTipProperties.Text` attached property shows a native hover tooltip on Windows/Mac — a desktop affordance mobile ignores (no hover).

### 67. What is a `Brush` vs a `Color` property?
**A:** Colors are flat values; brushes describe painting (solid, linear/radial gradient). Some properties are brush-typed (`Stroke` on Shapes) and need a `SolidColorBrush` wrapper even for flat colors.

### 68. What are MAUI Shapes (`Path`, `Polygon`...)?
**A:** Vector primitives drawn in the visual tree; `Path.Data` takes geometry mini-language strings (`M7,15.5 L7,2.2...`) — resolution-independent icons/arrows without images.

### 69. How do you show alerts and action sheets?
**A:** `Page.DisplayAlert` / `DisplayActionSheet` (awaitable native dialogs); for custom layouts, toolkit popups replace them.

### 70. What does the CommunityToolkit.Maui add?
**A:** Production niceties the box lacks: `Popup` with results, `Snackbar`/`Toast`, behaviors, extra converters, `MediaElement`, drawing view — registered via `.UseMauiCommunityToolkit()`.

## H. Platform Integration (Q71–Q80)

### 71. How do you write platform-specific code in shared files?
**A:** `#if ANDROID / WINDOWS / IOS` preprocessor blocks (the TFM defines the symbol), used for handler tweaks or platform APIs — kept small and centralized.

### 72. How does the `Platforms/` folder work?
**A:** Files under `Platforms/Android`, `Platforms/Windows` etc. compile only into that TFM — entry points (`MainActivity`, `App.xaml` WinUI host) and platform services live there without `#if` noise.

### 73. How do you customize a native control without a custom renderer?
**A:** Append to the handler's mapper: `EntryHandler.Mapper.AppendToMapping("key", (handler, view) => ...)` — runs for every instance, letting you set native properties (e.g. remove Android's underline).

### 74. What is .NET MAUI Essentials functionality?
**A:** Built-in device APIs: `Preferences`/`SecureStorage`, `Connectivity`, `Geolocation`, `Clipboard`, `Share`, sensors, `AppInfo` — cross-platform wrappers with per-platform permission requirements.

### 75. How are runtime permissions handled?
**A:** Declare in the platform manifest, then `Permissions.RequestAsync<Permissions.LocationWhenInUse>()` at the moment of use, branching on the returned status.

### 76. `Preferences` vs `SecureStorage`?
**A:** `Preferences` stores small plain key-values (settings); `SecureStorage` encrypts via Keystore/Keychain/DPAPI — for tokens and secrets, never for bulk data.

### 77. How do you detect the platform/idiom at runtime?
**A:** `DeviceInfo.Platform` (Android/WinUI/iOS) and `DeviceInfo.Idiom` (Phone/Tablet/Desktop), plus XAML `OnPlatform`/`OnIdiom` markup for per-platform values.

### 78. How would you display web content or call JS?
**A:** `WebView` for full pages; `HybridWebView` for hosting a local SPA with two-way JS↔C# messaging.

### 79. What is the app's main/UI thread rule and how do you marshal to it?
**A:** UI objects are touched only on the main thread; background work dispatches updates via `MainThread.BeginInvokeOnMainThread` or `Dispatcher.Dispatch` — `await` after `Task.Run` returns you to the captured context in page code.

### 80. How do platform lifecycle differences show up on Windows vs Android?
**A:** Windows apps close (window destroyed → process ends); Android activities can be backgrounded/killed anytime, so state you must keep goes to storage on `Stopped`/`Deactivated`, not memory.

## I. Performance & Quality (Q81–Q90)

### 81. Top causes of slow MAUI list UIs?
**A:** Non-virtualized layouts for big data (BindableLayout), heavy item templates (deep nesting), uncompiled bindings, and oversized images not downsampled.

### 82. How do compiled bindings help performance?
**A:** They replace reflection path resolution with generated strongly-typed accessors — faster binding setup/updates plus compile-time validation.

### 83. What is trimming and what risk does it bring?
**A:** Release-time removal of unused IL to shrink apps; reflection-only usage (bindings without x:DataType, serializers, DI by name) can be over-trimmed, causing runtime `MissingMethod`/null template bugs — mitigated with annotations or trimmer config.

### 84. JIT vs AOT on Android/iOS in MAUI?
**A:** Android debug uses JIT (+Fast Deployment); Release can add profiled AOT to cut startup JIT cost. iOS mandates full AOT — no JIT allowed by the platform.

### 85. How do you diagnose a memory leak in a page?
**A:** Watch for pages not GC'd after navigation: static/event subscriptions not detached (`CollectionChanged`, messenger), long-lived services holding view references; use profilers/`WeakReference` checks and unsubscribe in `OnDisappearing`/handlers.

### 86. Why prefer `Grid` definitions over deep nested stacks?
**A:** Each layout level adds a measure/arrange pass; one Grid with rows/columns replaces several nested stacks, reducing layout cost and visual tree depth.

### 87. How do you handle images efficiently?
**A:** Use `Resizetizer` (`MauiImage`) to bake correct densities, request sizes near display size, cache remote images, and prefer vectors (SVG→per-density PNG at build) for icons.

### 88. What's the strategy for app startup performance?
**A:** Do less before first frame: defer non-critical service init, load data async after `OnAppearing`, keep `MauiProgram` lean, enable AOT where JIT dominates startup.

### 89. How do you unit-test MAUI view models?
**A:** Keep view models UI-free: inject fakes for service interfaces, assert property changes/command effects — plain xUnit/NUnit tests, no MAUI runtime needed.

### 90. How do you log and surface errors in production?
**A:** `Microsoft.Extensions.Logging` providers configured in `MauiProgram` (Debug locally; AppCenter/Sentry-style sinks in production), global handlers (`AppDomain.UnhandledException`, `TaskScheduler.UnobservedTaskException`) to capture crashes.

## J. Data, Async & Deployment Basics (Q91–Q100)

### 91. How is `HttpClient` used properly in MAUI apps?
**A:** One long-lived instance (or `IHttpClientFactory` pattern) injected via DI with `BaseAddress` set; helpers like `GetFromJsonAsync<T>` deserialize responses using System.Text.Json.

### 92. Where do MAUI apps store local data?
**A:** `FileSystem.AppDataDirectory` for files/SQLite databases, `Preferences` for small settings, `SecureStorage` for secrets, `FileSystem.CacheDirectory` for regenerable content.

### 93. What is the repository/service abstraction and why bother in a small app?
**A:** View models depend on interfaces (e.g. a data-service interface) rather than concrete HTTP/static implementations — enabling seamless source swaps (mock → API) and testability, at the cost of one small indirection.

### 94. `Task.WhenAll` vs sequential awaits?
**A:** `WhenAll` runs independent async operations concurrently and awaits all — total time ≈ slowest one; sequential awaits sum every latency. Use `WhenAll` when operations don't depend on each other.

### 95. What is `ConfigureAwait(false)` and does it matter in MAUI?
**A:** It skips resuming on the captured (UI) context; useful in library-layer code, but in page/view-model code you usually *want* the UI context back, so it's used sparingly there.

### 96. How do you cancel in-flight async work?
**A:** Pass a `CancellationToken` from a `CancellationTokenSource` through the async chain; cancel on navigation-away/new request (e.g. search-as-you-type), catching `OperationCanceledException`.

### 97. What artifacts does each platform ship?
**A:** Android: signed APK (sideload) or AAB (Play). Windows: unpackaged exe folder or MSIX package. iOS: IPA via Apple signing. Each carries the version from `ApplicationDisplayVersion`/`ApplicationVersion`.

### 98. What's the difference between debug deployment and store distribution on Android?
**A:** Debug builds are debug-signed, JIT, fast-deployment — installable only via adb/dev tooling; store builds are release-signed with your keystore, trimmed/optimized, with an incremented `versionCode`.

### 99. How do you support app themes (light/dark)?
**A:** `AppThemeBinding` per property or theme dictionaries keyed by `Application.RequestedTheme`, listening to `RequestedThemeChanged` — plus platform manifest opt-ins where needed.

### 100. How do you localize a MAUI app?
**A:** RESX resource files per culture with generated accessors bound in XAML (via markup extension or static class), setting `CultureInfo` per device; images/text direction handled with `FlowDirection`.

---

# Part 2 — This Project (Q101–Q207)

Questions grounded in this repository's real code — file names cited so you can open and revise.

## A. Architecture & Project Structure (Q101–Q110)

### 101. Walk through this solution's folder structure and each folder's responsibility.
**A:** `Models` (POCOs like `OrderModel`), `ViewModels` (MVVM logic), `Views` (pages + popups), `Components` (reusable `ContentView`s like `SalesChartView`), `Services` (+`Interfaces`) for data/print, `Converters` (XAML value converters), `Charts` (global chart data/theme/geometry + drawables), `Resources` (colors, styles, fonts), `Platforms` (per-OS bootstrap).

### 102. What design pattern does the app follow end-to-end for data display?
**A:** Service → ViewModel → XAML binding: `IDashboardDataService` returns `Result<IReadOnlyList<T>>`, `MainViewModel` copies data into `ObservableCollection`s, and pages/components bind to those collections — no view touches a service directly.

### 103. Why does the app register `IDashboardDataService` as a singleton (`MauiProgram.cs`)?
**A:** The dashboard's data source is stateless demo data shared app-wide; a single instance avoids re-allocating and makes a later swap to the HTTP implementation (also naturally shared) a one-line change.

### 104. How would you switch this app from static data to a real API?
**A:** In `MauiProgram.cs`, replace the `StaticDashboardDataService` registration with `ApiDashboardDataService` fed by an `HttpClient` with a `BaseAddress` — nothing else changes because both implement `IDashboardDataService`.

### 105. What is the `Charts/` layer introduced in the refactor and why is it "industry level"?
**A:** A config-driven chart architecture: `ChartData` (every dataset declared once), `ChartTheme` (single source for chart colors), `ChartGeometry` (shared math), and five generic drawables. Adding a chart is a data change, not new rendering code — single source of truth + separation of concerns.

### 106. How do you add a brand-new chart to this app tomorrow?
**A:** 1) Declare its values in `Charts/ChartData.cs`; 2) in the view, assign an existing drawable, e.g. `MyCanvas.Drawable = new MiniLineChartDrawable(color, ChartData.MyNewValues);` — theme, geometry, and rendering are already there.

### 107. Why keep the deliberate "Revinue Status" typo (`StaticDashboardDataService.cs`)?
**A:** The assignment is a high-fidelity replication of a reference template screenshot; matching it exactly — including its typos — is the acceptance criterion, so correctness here means visual fidelity, not spelling.

### 108. Why are chart datasets in a static class instead of the data service?
**A:** They're presentation demo-data tied to chart shape (normalized 0–1 curves, per-tab series), not business entities; `ChartData` gives all views one import with zero async ceremony, while true business data (orders, cards) still flows through the service.

### 109. What does `run.cmd` solve?
**A:** `dotnet run` can't pick between multiple TFMs; the script asks Windows-or-Android, then runs the right `dotnet build -t:Run -f <tfm>` — including auto-starting/boot-polling the Pixel 7 emulator for Android.

### 110. What target frameworks does this project build, and why conditionally?
**A:** On Windows: `net10.0-windows10.0.19041.0` + `net10.0-android`; on non-Windows: android/ios/maccatalyst — conditioned in the csproj because WinUI needs Windows and Apple targets need macOS.

## B. MVVM & CommunityToolkit.Mvvm in This Code (Q111–Q122)

### 111. What does `MainViewModel` inherit and why (`BaseViewModel.cs`)?
**A:** `BaseViewModel` derives from the toolkit's `ObservableObject`, adding shared `IsBusy`/`Title` state — giving every view model change notification and common status without repetition.

### 112. Explain `[ObservableProperty] private string _searchText;` in `MainViewModel`.
**A:** The source generator creates a public `SearchText` property raising `PropertyChanged`, plus a `partial void OnSearchTextChanged(string value)` hook — which this VM implements to reset paging and re-filter orders on every keystroke.

### 113. Why is there a `#pragma warning disable MVVMTK0045` around the observable fields?
**A:** The toolkit warns that `[ObservableProperty]` fields aren't AOT-friendly on WinRT scenarios; the team acknowledged and suppressed it deliberately for this assignment rather than switching to partial properties.

### 114. How does `LoadDataCommand` get created and what triggers it?
**A:** `[RelayCommand]` on `LoadDataAsync` generates `LoadDataCommand` (`IAsyncRelayCommand`); `InitializeAsync` executes it from `MainPage.OnAppearing`, guarded by `IsDataLoaded` so it runs once.

### 115. Why does `LoadDataAsync` use `Task.WhenAll` over six loaders?
**A:** Cards, revenue cards, activities, orders, and traffic are independent; parallel awaiting makes total load time ≈ the slowest call instead of the sum — faster dashboard readiness.

### 116. How does the search feature work end-to-end?
**A:** `Entry` two-way binds `SearchText` → generated setter fires `OnSearchTextChanged` → `_currentPage=1` + `RefreshOrdersView()` → `FilteredOrders` LINQ matches customer/country/status/invoice case-insensitively → `Orders` collection is rebuilt for the current page.

### 117. Why do computed properties like `WalletCard => DashboardCards.ElementAtOrDefault(0)` need manual `OnPropertyChanged` calls?
**A:** They're getter-only projections — nothing notifies when the underlying collection loads. After repopulating, the VM raises `OnPropertyChanged(nameof(WalletCard))` etc. so the four summary tiles re-bind.

### 118. What's the pattern behind `SelectOrderCommand` toggling `IsSelected`?
**A:** Single-selection: clear the flag on all orders, then set the tapped one (or leave all cleared if it was already selected — a toggle), storing `_selectedOrder` for the delete flow; `OrderModel.IsSelected` notifies so row highlight updates.

### 119. How does `AddOrderAsync` receive results from the popup?
**A:** `ShowPopupAsync(new AddOrderPopup())` returns the popup's result object; the VM pattern-matches a tuple `(string customer, string country, decimal price, string status)` and appends only when the popup completed successfully.

### 120. Why does `AppendOrder` set `_currentPage = int.MaxValue`?
**A:** `RefreshOrdersView` clamps the page into valid range; `int.MaxValue` therefore lands on the *last* page, guaranteeing the just-added order (appended at the end) is visible immediately.

### 121. How is "show info" (`ShowOrderInfoAsync`) computed?
**A:** It aggregates the *filtered* orders — counts per status (`Open`/`Process`/`On Hold`) and total value via LINQ — and passes them with the search text into `OrderSummaryPopup`, so the summary reflects what the user currently sees.

### 122. Where does the "Last Month Summary" data live and why is that OK?
**A:** A static readonly list of `SummaryStat` records in `MainViewModel` feeding `LastMonthSummaryPopup` — acceptable because it's fixed demo content for one popup; if it became dynamic it would move behind the service like the rest.

## C. ObservableCollection & Binding Specifics (Q123–Q134)

### 123. Why are all VM collections declared `public ObservableCollection<T> X { get; } = [];`?
**A:** Read-only auto-properties guarantee the *instance* never changes — bindings subscribe once and never go stale; loaders mutate contents (`Clear`+`Add`) so `INotifyCollectionChanged` drives UI updates.

### 124. Why `Clear()` + `foreach Add()` instead of assigning a new collection?
**A:** Assignment would require property-change notification and re-subscription by every binding; mutating in place reuses the existing binding pipeline and keeps the pattern uniform across loaders.

### 125. What UI consumes `Orders` and how does paging interact with it?
**A:** The order table (`OrderTableView`) binds `Orders`, which always holds *only the current page* (5 rows); paging/search rebuild it, so the UI never needs to know about the full `_allOrders` list.

### 126. Why keep `_allOrders` as a plain `List<OrderModel>` alongside the bound collection?
**A:** It's the full in-memory dataset used for filtering/paging math — it never binds to UI, so a lightweight `List` is enough; `Orders` is the small observable window the UI shows.

### 127. How do `PageNumbers` stay in sync with data changes?
**A:** Every `RefreshOrdersView()` recomputes total pages and rebuilds `PageNumbers` with `PageItem(number, isCurrent)` records — add/delete/search all funnel through the same method, so paging can't drift.

### 128. Why did `TrafficChartView` need `INotifyCollectionChanged` handling for the donut (`TrafficChartView.xaml.cs`)?
**A:** The bound `TrafficSources` instance exists (empty) before data loads; the `ItemsSource` property changes once, then items arrive as `Add` events — subscribing to `CollectionChanged` redraws the donut when data lands, which a one-time property callback would miss.

### 129. In that view, why unsubscribe from the old collection in the property-changed callback?
**A:** If the consumer ever swaps collections, keeping the old subscription would leak the view (event source holds the handler) and trigger redraws from a dead source — the unhook-old/hook-new pattern is standard event hygiene.

### 130. The donut renders segments in *reverse* list order — why?
**A:** The service lists sources in legend order (Facebook 34, Youtube 55, Direct 11) while the reference draws clockwise from 12 o'clock starting with the smallest (yellow 11) — reversing reconciles one data order with both presentations, keeping a single source of truth.

### 131. How does the legend inside `TrafficChartView.xaml` get its items now?
**A:** `BindableLayout.ItemsSource="{Binding ItemsSource, Source={x:Reference Root}}"` — bound to the component's own bindable property rather than the page's BindingContext, making the component self-contained and reusable.

### 132. What's the role of `x:DataType="models:TrafficModel"` in that template?
**A:** Compiled bindings: `Percentage`, `Source`, `SegmentColorHex` resolve against the known type at compile time — typo-safe and faster than reflection bindings.

### 133. `OrderModel.IsSelected` must raise change notifications — why?
**A:** Row highlight binds to it; when `SelectOrder` flips flags on model instances already displayed, only `INotifyPropertyChanged` on the model makes the existing rows restyle without rebuilding the collection.

### 134. When would you replace `BindableLayout` in this app with `CollectionView`?
**A:** If a list grew beyond trivial size (say hundreds of activities) — `BindableLayout` creates all views eagerly with no virtualization; for the current 3–5-item legend/timeline it's the lighter, simpler tool.

## D. Services, DI & the Result Pattern (Q135–Q146)

### 135. What does `Result<T>` look like and why use it (`Models/Result.cs`)?
**A:** A wrapper with `IsSuccess`/`IsFailure`, `Data`, and `Error` created via `Success(data)`/`Failure(msg)` — it makes failures part of the return type so loaders do `if (result.IsFailure) return;` instead of try/catch at every call site.

### 136. Where do exceptions actually get caught in the data flow?
**A:** At the boundary: `ApiDashboardDataService.GetListAsync<T>` wraps the HTTP+JSON call in try/catch and converts any exception into `Result.Failure` — view models never see exceptions, only results.

### 137. Explain the generic `GetListAsync<T>` refactor in `ApiDashboardDataService`.
**A:** Five endpoint methods had identical try/catch/deserialize bodies; they collapsed into one private generic helper taking `(endpoint, resourceName)`, with each public method a one-line delegation — DRY, uniform error messages, ~half the file.

### 138. Why does the interface return `IReadOnlyList<T>` instead of `List<T>`?
**A:** Callers only need to read; the read-only contract prevents consumers mutating service-owned data and leaves the implementation free to return any list-shaped type.

### 139. Why `Task.FromResult` in `StaticDashboardDataService`?
**A:** The interface is async (ready for real I/O); the static implementation has data in memory, so it returns an already-completed task — same signature, zero thread overhead.

### 140. What would change to add caching to the API service?
**A:** Decorate or extend `GetListAsync<T>`: check a memory cache keyed by endpoint before the HTTP call and store successful results with an expiry — one place, all five endpoints inherit it (a benefit of the generic helper).

### 141. Why is `GetRevenueCardsAsync` on the service now instead of hardcoded in the VM?
**A:** Consistency of the data flow: every dashboard dataset comes through `IDashboardDataService`, so the API swap covers revenue cards too and `MainViewModel` stays orchestration-only (the refactor moved the four card definitions into `StaticDashboardDataService`).

### 142. How is `MainViewModel` itself constructed at runtime?
**A:** DI: `MainPage` is registered and takes `MainViewModel` in its constructor, which takes `IDashboardDataService` + `IPrintService` — the container resolves the chain when Shell needs the page.

### 143. What does `IPrintService` abstract and why keep it separate from the data service?
**A:** Printing/exporting the filtered orders (`PrintOrdersAsync(orders, name)`) — an output concern with platform-specific implementation, unrelated to data retrieval; separate interfaces keep responsibilities single.

### 144. How would you unit-test `MainViewModel.RefreshOrdersView` logic?
**A:** Inject a fake `IDashboardDataService` returning a known order list, run `InitializeAsync`, then assert `Orders` contents, `PageNumbers`, and `OrdersFooterText` for scenarios: search filters, page clamps, add-jumps-to-last-page, delete refresh.

### 145. Why was the dead `SalesData`/`GetSalesDataAsync` path removed in the refactor?
**A:** The VM loaded `SalesDataPoints` that no XAML ever bound — the sales chart uses richer per-period series from `ChartData`. Dead code costs comprehension and maintenance; removing model + interface method + loaders tightened the seam.

### 146. `HttpClient.GetFromJsonAsync<List<T>>` — what does it do and what nulls must you handle?
**A:** GET + status check + System.Text.Json deserialize in one call; it can return `null` (e.g. literal `null` body), hence the `data ?? []` guard before wrapping in `Result.Success`.

## E. Custom Charts: GraphicsView & Drawables (Q147–Q161)

### 147. Which charts in this dashboard are custom-drawn and with what?
**A:** All of them — sales spline chart, traffic donut, and three mini charts (bar/area/line) — implemented as `IDrawable`s rendered by `GraphicsView`, no chart library involved.

### 148. Why draw charts manually instead of using a library like Microcharts?
**A:** Pixel-fidelity to the reference template (exact grid, fills, marker style) plus zero dependency risk; `Microsoft.Maui.Graphics` is built in and gives complete control at the cost of writing the math.

### 149. Walk through `SplineChartDrawable.Draw` (`Charts/Drawables/SplineChartDrawable.cs`).
**A:** Compute plot rect with margins → draw horizontal gridlines + y labels every 5 up to `yMax` → vertical gridline + label per x tick → for each `SplineSeries`, map values to points, fill the under-curve area at the series' alpha, then stroke the spline at width 2.

### 150. How does `ChartGeometry.BuildSpline` create a smooth curve?
**A:** Cubic Bézier per segment: control points offset horizontally by `(x2−x1)/tension` from each endpoint at the endpoints' own y — yielding smooth horizontal-tangent transitions; tension 2.2 (big chart) vs 2.0 (mini) tunes roundness.

### 151. Why does `SplineSeries` carry `FillAlpha` and the series list draw in order?
**A:** Each series paints a translucent area fill under its own stroke; painter's order means the *last* series lands on top — the view passes Store first, Online second to match the reference layering.

### 152. How does the DAILY/WEEKLY/MONTHLY/YEARLY tab switch work (`SalesChartView.xaml.cs`)?
**A:** Tap handlers restyle tabs (active style + underline visibility), reset the legend filter, then `ApplyPeriod(label)` looks up `ChartData.SalesByPeriod[period]`, builds the series list respecting show-flags, assigns a new drawable, and calls `Invalidate()`.

### 153. Explain the legend "solo" toggle behavior.
**A:** Tapping Online/Store solos that series (the other hides, opacity dims its legend row); tapping the soloed one again restores both — implemented with `_showOnline/_showStore` flags and re-rendering; the filter intentionally persists across tab switches.

### 154. How does the donut drawable build each segment (`DonutChartDrawable.cs`)?
**A:** For each segment: sweep = pct/100×360; construct a closed `PathF` walking the outer arc in small angular steps then the inner arc back (polygon approximation), fill with the segment color, advance the start angle — starting at −90° (12 o'clock).

### 155. Why approximate arcs with line steps instead of `AddArc`?
**A:** The stepped polygon (≥12 steps, ~2°/step) renders visually identically, keeps the math explicit/portable, and was preserved during refactor to guarantee zero pixel drift — rewriting to arc APIs risks subtle platform differences for no visible gain.

### 156. Where do the mini bar/area values come from and what do they represent?
**A:** `ChartData.RevenueBarValues` (valley profile ending on the tallest bar) and `ChartData.PageViewAreaValues` (peaks-and-valleys wave) — normalized 0–1 heights transcribed from the reference screenshot.

### 157. How does the Bounce Rate card keep chart and headline in sync?
**A:** `ChartData.BounceRateByPeriod` maps each period to a `PeriodSnapshot(Curve, Value)`; picking a period in the popup sets both `CardValue` and the line drawable from the same record — one lookup, two UI updates.

### 158. What triggers a `GraphicsView` to repaint?
**A:** Assigning a new `Drawable` doesn't repaint by itself reliably — the views call `Invalidate()` after assignment (tab switch, legend toggle, accent change, donut data arrival) to force `Draw` on the next frame.

### 159. Why are drawables constructed with data instead of reading globals inside `Draw`?
**A:** Constructor injection makes them *generic and reusable* (any values/colors), keeps `Draw` pure rendering, and means the same `MiniLineChartDrawable` can serve tomorrow's new card by passing different `ChartData` values.

### 160. How does XAML consume `ChartTheme` and why is that better than duplicated hex strings?
**A:** `{x:Static charts:ChartTheme.SalesOnline}` on the legend dots — the same constants the drawable strokes with; before the refactor the hexes existed in both `Colors.xaml` and drawable code, risking drift on any palette change.

### 161. How would you animate these charts?
**A:** Drive a 0→1 progress field on the drawable (e.g. scale sweep angles or y values by it) and repeatedly `Invalidate()` from a timer/`Animation` — or animate the `GraphicsView` itself (opacity/translation) for cheap entrance effects.

## F. Reusable Components & BindableProperty (Q162–Q171)

### 162. What reusable components does this app define?
**A:** `SidebarView`, `DashboardHeaderView`, `SummaryCardView`, `SalesChartView`, `TrafficChartView`, `RevenueCardView`, `ActivityTimelineView`, `OrderTableView` — each a `ContentView` used by `MainPage`.

### 163. Why does `RevenueCardView` declare six `BindableProperty`s?
**A:** Title/value/subtitle/background/accent/chart-type must be *bindable per instance* from `MainPage.xaml` (four cards, four datasets) — regular CLR properties can't participate in MAUI bindings or styles.

### 164. Anatomy of a `BindableProperty.Create` call?
**A:** Property name (`nameof(CardTitle)`), property type, declaring type, default value, and optional `propertyChanged` callback — plus a CLR wrapper property calling `GetValue`/`SetValue`.

### 165. Why do `AccentColor` and `ChartType` have `propertyChanged` callbacks but `CardTitle` doesn't?
**A:** Title/value/subtitle are consumed by XAML bindings that update automatically; accent and chart type drive *code* (choosing the visible layout, rebuilding drawables, tinting arrow Paths), so the callback calls `UpdateChart()`.

### 166. What does `x:Name="Root"` + `Source={x:Reference Root}` achieve inside a component's own XAML?
**A:** The component's internal elements bind to the component's *own* bindable properties instead of the inherited page BindingContext — self-contained templating (`{Binding CardTitle, Source={x:Reference Root}}`).

### 167. How does `RevenueCardView` render three different chart types from one control?
**A:** Three pre-declared layouts (`BarLayout`/`AreaLayout`/`LineLayout`) toggled by `IsVisible` per `ChartType`, each with its own `GraphicsView` — a simple visibility strategy over template swapping, easy to read and fast for three fixed variants.

### 168. Why are the card arrows `Path` shapes instead of a "↑" text glyph?
**A:** The comment in the XAML says it: the arrow glyph falls back to a different font and ignores bold, breaking fidelity — a stroked vector `Path` renders identically everywhere and takes the accent color via `Stroke`.

### 169. Why does the Page View area chart use negative margins (`Margin="-18,6,-18,-16"`)?
**A:** The reference wave bleeds to the card edges; negative margins cancel the card's padding so the `GraphicsView` reaches the border, and the `Border` clips the overflow to the rounded shape.

### 170. How does `SummaryCardView` get its data with no bindable properties?
**A:** Via `BindingContext` assignment per instance: `MainPage.xaml` sets `BindingContext="{Binding WalletCard}"` etc., so its internal bindings (`Title`, `AmountDisplay`, colors) resolve against a `DashboardCard`.

### 171. When do you choose BindableProperties vs BindingContext for a component — and which does `TrafficChartView` use now?
**A:** BindableProperties when the parent maps *multiple different sources* onto one component or you need change callbacks (RevenueCardView); BindingContext when the component mirrors one model object (SummaryCardView). `TrafficChartView` now uses an `ItemsSource` bindable property — the collection-friendly variant of the first approach.

## G. Popups & User Interactions (Q172–Q181)

### 172. What popup infrastructure does the app use?
**A:** CommunityToolkit.Maui `Popup` subclasses (`AddOrderPopup`, `ConfirmDeletePopup`, `PeriodPickerPopup`, `LastMonthSummaryPopup`, `OrderSummaryPopup`) shown with `page.ShowPopupAsync(...)`, which awaits until the popup closes and returns its result.

### 173. How does a popup return a typed result?
**A:** It calls `Close(result)` with a payload; the awaited `ShowPopupAsync` returns that object — this app pattern-matches, e.g. `if (result is (string c, string co, decimal p, string s))` for add-order, `is not true` for delete confirmation.

### 174. Why does `AddOrderAsync` pattern-match a tuple instead of a model?
**A:** The popup deliberately returns raw user input; the VM owns constructing `OrderModel` (assigning the next invoice number, defaults) — creation rules stay in one place.

### 175. How is the delete flow made safe?
**A:** Two gates: no selection → informational alert ("tap a row first"); with selection → `ConfirmDeletePopup` showing the order, deleting only on an explicit `true` result — destructive action requires explicit confirmation.

### 176. What Windows-specific popup issues did this project hit (see memory/comments)?
**A:** Two WinUI quirks: a `Picker` inside a popup fights light-dismiss (fixed by disabling light-dismiss so the dropdown works), and unconstrained popup height clips footers (fixed by capping popup height to the window).

### 177. Why did `PeriodPickerPopup` replace a native `Picker` approach for the Bounce Rate chip?
**A:** Reliability and fidelity on WinUI: a styled popup with four options avoids the underline/light-dismiss issues entirely and matches the design's look; the chip then updates label, tooltip, value, and chart from the choice.

### 178. How do tooltips improve discoverability here?
**A:** `ToolTipProperties.Text` on interactive-but-subtle elements (legend rows "Show only Online sales", the period chip) tells desktop users these are clickable — compensating for flat design giving no affordance.

### 179. Where does the app use `DisplayAlert` instead of a popup, and why?
**A:** The "select a row first" hint before delete — a simple informational message where a native alert is adequate; custom popups are reserved for styled or result-returning flows.

### 180. What is `Shell.Current.CurrentPage` used for in the VM commands?
**A:** `ShowPopupAsync` is an extension on `Page`; commands resolve the active page at call time (null-guarded) rather than storing page references in the VM — keeping the VM UI-agnostic except for this one seam.

### 181. How would you unit-test logic around popups?
**A:** Extract decisions into testable methods (e.g. `AppendOrder` is separate from the popup call) and/or wrap popup interaction behind an interface (`IDialogService`) faked in tests — asserting VM state changes for each simulated result.

## H. XAML Resources, Styles & Converters (Q182–Q189)

### 182. What converters exist and what does each do?
**A:** `HexToColorConverter` (hex string → `Color` for model-driven colors), `StatusColorConverter`/`StatusBackgroundConverter` (order status → badge text/background colors), `AmountToCurrencyConverter` (decimal → currency string).

### 183. Why do models store `ThemeColorHex`/`SegmentColorHex` strings instead of `Color`?
**A:** Keeps models UI-framework-agnostic (serializable from an API, no MAUI dependency); XAML converts at the edge via `HexToColorConverter` — noted explicitly in `RevenueCardItem`'s doc comment.

### 184. Where are converters registered and how are they consumed?
**A:** As shared resources in `App.xaml`'s dictionary; XAML references `Converter={StaticResource HexToColorConverter}` inside bindings anywhere in the app.

### 185. How is the status badge colored per row?
**A:** The `Status` string binds twice with different converters — text color and background — mapping Open/Process/On Hold to the template's green/blue/orange palette in one reusable place.

### 186. What lives in `Colors.xaml` vs `Styles.xaml` vs `ChartTheme.cs` after the refactor?
**A:** `Colors.xaml`: app design tokens (sidebar, text tiers, primary orange). `Styles.xaml`: reusable styles (card borders, tab label styles, section titles). `ChartTheme.cs`: chart-only palette shared by XAML and drawables — chart color keys were removed from `Colors.xaml` to keep one source.

### 187. What are `TabActiveStyle`/`TabInactiveStyle` and how are they swapped?
**A:** Label styles for the chart period tabs; `SalesChartView` swaps them in code on tap (`Application.Current.Resources[...]`) along with the underline `BoxView` visibility — style-driven state without duplicating fonts/colors.

### 188. Why `FontAwesome` glyphs instead of image icons?
**A:** One TTF gives crisp, tintable, resolution-independent icons via text (`Icon = ""` style codes in service data) — no per-density assets, colorable through `TextColor`, tiny footprint.

### 189. What is the risk of string-keyed resources and how does this project mitigate it?
**A:** Typos fail at runtime, not compile time; mitigations here: compiled bindings for data (`x:DataType`), `x:Static` for chart constants (compile-checked), and a small, curated resource dictionary reviewed against the design.

## I. Responsive Layout & Platform Quirks (Q190–Q199)

### 190. How does the dashboard adapt to narrow windows/phones (`MainPage.xaml.cs`)?
**A:** A `SizeChanged` handler compares width to a 980-unit breakpoint (guarded by a nullable flag to avoid rework); narrow mode swaps grid shapes via `SetGridShape`, stacks cards 2×2, drops dividers, and turns the sidebar into a hidden overlay behind a hamburger button.

### 191. Why manipulate `Grid.ColumnDefinitions` in code instead of two XAML layouts?
**A:** One visual tree reused in both modes avoids duplicating ~200 lines of card markup and keeps state (entered search text, chart selections) intact across breakpoint changes — only geometry changes.

### 192. How does the overlay sidebar work in narrow mode?
**A:** Column 0 collapses to width 0; the sidebar moves to column 1 with fixed 200 width, `ZIndex=10`, hidden by default, toggled by the hamburger; a top inset keeps menu items below the floating button.

### 193. Why does the column math use `0.85*` three times and `*` once in the analytics row?
**A:** So the fourth card aligns exactly under the Traffic card: the top row is `2.55* : 1*`, and 3 × 0.85 = 2.55 — the comment in the XAML documents this deliberate proportion.

### 194. What does `MainPage.OnAppearing`'s `ScrollToAsync(0,0,false)` fix?
**A:** WinUI auto-scrolls the first focusable control (the search box) into view on launch, opening the page mid-scroll; dispatching a scroll-to-origin re-anchors the dashboard at the top.

### 195. What are the `DASH_TEST_DETAIL`/`DASH_TEST_PRINT` environment hooks?
**A:** Temporary automation seams: when set, `OnAppearing` navigates to the detail page or triggers the print command — letting scripted UI verification exercise flows without manual clicks.

### 196. How is the Windows Entry underline removed, and why is it tricky?
**A:** A handler mapping overrides WinUI TextBox theme resources — but only inside the control's `Loaded` event and including *rest* state keys; setting resources before the control joins the visual tree crashes with `COMException 0x800F0902`, and missing rest-state keys leaves the underline until hover.

### 197. What equivalent tweak does Android need?
**A:** Material `EditText`/`Picker` draw their own underline: mappers set `BackgroundTintList` to transparent so form fields match the flat bordered design.

### 198. Why does the Windows app launch maximized?
**A:** A lifecycle event grabs the native window → `AppWindow` → `OverlappedPresenter.Maximize()` — the dashboard is designed for full-desktop proportions like the reference screenshot.

### 199. `Sidebar.WidthRequest = -1` — what does that mean?
**A:** −1 resets `WidthRequest` to "unset," returning the sidebar to auto/fill sizing from the layout (desktop column width) after narrow mode had forced a fixed 200.

## J. Orders Table: Search, Paging & CRUD (Q200–Q207)

### 200. Describe the paging model.
**A:** Constant `PageSize = 5`; `RefreshOrdersView` computes `totalPages = ceil(filtered/5)`, clamps `_currentPage`, fills `Orders` with `Skip/Take`, rebuilds `PageNumbers`, and formats `OrdersFooterText` ("Showing X to Y of Z entries").

### 201. How are next/previous guarded against running off the ends?
**A:** They just increment/decrement — `RefreshOrdersView`'s `Math.Clamp(_currentPage, 1, totalPages)` makes out-of-range values legal, centralizing bounds logic instead of scattering `if` checks.

### 202. Why does searching reset to page 1?
**A:** The filtered set shrinks/changes — staying on page 4 of 2 pages would show emptiness; resetting in `OnSearchTextChanged` always lands the user on the first page of *new* results.

### 203. How is the new invoice number chosen in `AppendOrder`?
**A:** `_allOrders.Count == 0 ? 12412 : Max(Invoice) + 1` — continues from the template's seed range (last seed is 12411) and stays unique even after deletions.

### 204. What happens to search state when adding an order, and why?
**A:** `SearchText` is cleared before refreshing — otherwise the just-added order could be filtered out and invisible; clearing plus jump-to-last-page guarantees the user sees what they created.

### 205. Footer says "Showing 0 to 0 of 0 entries" — how does the code produce that?
**A:** With zero filtered rows, `start` is special-cased to 0 (`total == 0 ? 0 : ...`) and `end = min(page*5, 0) = 0` — the guard avoids the nonsensical "Showing 1 to 0".

### 206. How would you move this table to server-side paging?
**A:** Extend the service method to take `(page, pageSize, search)` returning items + total count; `RefreshOrdersView` becomes async delegating filtering/paging to the API, with the same bound collections — UI unchanged.

### 207. The print button prints *filtered* orders — argue for/against.
**A:** For: WYSIWYG — users print what they've narrowed to, and the summary popup matches the same scope. Against: someone may expect the full ledger; a print dialog offering "current view / all orders" would remove the ambiguity — a good discussion answer.

---

*Prep tip: for Part 2, open the cited file while rehearsing each answer — interviewers for this assignment will ask "show me where."*
