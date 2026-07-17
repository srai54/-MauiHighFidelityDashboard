# Explanation Video — Detailed Script

> One recording, three parts: **Introduction → every UI feature → the code flow**.
> Timings assume ~150 words/min for a **10–12 minute** video; they are cues, not hard stops.
> Each section gives you **[SHOW]** (what's on screen) and **"Say"** (the spoken line).

---

## PART 1 — INTRODUCTION [0:00–1:00]

**[SHOW]** The app running maximized on Windows.

> "Hi, I'm Rajib. This is a high-fidelity admin dashboard built with **.NET MAUI** —
> a single C# and XAML codebase that runs natively on **Windows and Android**
> (iOS and macOS build from the same project on a Mac).
> It recreates the assignment's design screenshot pixel-for-pixel."

> "The app follows a clean **MVVM architecture** with a dependency-injected service
> layer, so even though it runs on static data today, it's one line away from a real
> REST API. In this video I'll first walk through **every feature in the UI**, and
> then open the code and follow the **data flow from startup to screen**."

Quick orientation sweep with the mouse (don't click yet):

> "On the left — the navigation sidebar. Across the top — the header with monthly
> earnings and sales. Then the dashboard body: KPI summary cards, the sales chart,
> the traffic donut, analytics cards, a recent-activities timeline, and a fully
> interactive order table."

---

## PART 2 — UI FEATURES [1:00–6:30]

### 2.1 Sidebar navigation [1:00–1:30]

**[SHOW]** Hover the menu items, click one (e.g. *Widgets*), then navigate back.

> "The dark sidebar is a reusable component: logo, menu items, and section labels,
> exactly as in the design. Every item is clickable — clicking navigates through
> **Shell routing** to a detail page, passing the item's title as a query parameter,
> just like a route in a web app. Back navigation is built in."

### 2.2 Header + Last Month Summary popup [1:30–2:00]

**[SHOW]** Point at the earnings/sales figures, then click **Last Month Summary**.

> "The header shows the page title and this month's earnings and sales. The
> **Last Month Summary** button opens a structured popup — proper label–value rows:
> earnings, sales, new orders, refunds, top seller, best region, and a highlighted
> growth figure. It's a CommunityToolkit popup, not a bare alert."

Close the popup.

### 2.3 KPI summary strip [2:00–2:20]

**[SHOW]** Mouse across the four stat cards.

> "Four KPI cards — wallet balance, referral earnings, estimate sales, and earnings —
> each one is the **same reusable component** fed different data. On narrow screens
> this strip re-arranges into a 2-by-2 grid; I'll demo that at the end."

### 2.4 Sales chart with period tabs [2:20–2:50]

**[SHOW]** Click through **DAILY / WEEKLY / MONTHLY / YEARLY** tabs.

> "The sales spline chart is **drawn entirely by hand** with MAUI's `GraphicsView`
> and `IDrawable` — no third-party chart library. The gradient fill, the smooth
> spline, the axis labels — all custom drawing code. The tabs switch the dataset
> live, and the selected tab keeps its underline, matching the design."

### 2.5 Traffic donut chart [2:50–3:10]

**[SHOW]** Point at the donut and its legend.

> "Same story for the traffic donut — custom `IDrawable` drawing arcs per traffic
> source, with the legend bound to the same data. One data source, two views of it."

### 2.6 Analytics / revenue cards [3:10–3:40]

**[SHOW]** Point at the up/down arrows; click a card's filter and pick *Weekly*.

> "The analytics cards show each metric with a bold up-or-down arrow and a mini
> bar chart. These are stamped out by a **BindableLayout** — one template, repeated
> per item. The bounce-rate card has a period filter — Daily, Weekly, Monthly,
> Yearly — and picking one redraws its mini chart."

### 2.7 Activity timeline [3:40–4:00]

**[SHOW]** Scroll the timeline briefly.

> "The recent-activities timeline: color-coded dots by activity type, title and
> timestamp per entry — again one reusable component bound to a collection."

### 2.8 Order table — the workhorse [4:00–5:45]

Demo in this exact order:

1. **Search** — type `italy`, then a customer name, then clear.
   > "The search box filters **live** on every keystroke — across customer, country,
   > status, and invoice number — and the pagination and footer recalculate as you type."
2. **Status badges** — point at Delivered / Pending / etc.
   > "Each status renders as a colored badge — the colors come from small
   > **value converters**, so the color logic lives in one place."
3. **Pagination** — click page 2, then next/previous arrows.
   > "Real pagination: numbered pages, next and previous, and a footer that always
   > tells you 'Showing X to Y of Z entries'."
4. **Select + order info** — click a row, then the ℹ info button.
   > "Clicking a row selects it. The info button summarizes the **currently filtered**
   > orders — totals by status and total value."
5. **Add order** — click ➕, fill the popup, save.
   > "Add Order opens a form popup — customer, country, price, status. On save the
   > order gets the next invoice number, and the table **jumps to the last page** so
   > you immediately see what you just added."
6. **Delete order** — select a row, click 🗑, confirm.
   > "Delete asks for confirmation in a popup showing exactly which order you're
   > removing — and if nothing is selected, it tells you to pick a row first."
7. **Print** — click 🖨, show the preview, open the OS dialog, cancel.
   > "Printing is **real**, not a mock: the app builds an HTML report of the filtered
   > orders, shows a preview page, and hands it to the operating system — the WebView2
   > print dialog on Windows, Android's PrintManager on mobile. Paper or PDF."

### 2.9 Responsive layout [5:45–6:30]

**[SHOW]** Drag the window below ~980 px wide, open the hamburger, then restore.

> "Finally, responsiveness. Below a 980-pixel breakpoint the sidebar collapses into
> a **hamburger overlay**, the chart and cards restack into a single column, the KPI
> strip becomes a 2-by-2 grid, and the order table pans horizontally. Same behavior
> on a phone, a narrow window, or high display zoom — one layout system, all sizes."

---

## PART 3 — CODE FLOW [6:30–11:30]

### 3.1 Solution map [6:30–7:15]

**[SHOW]** The Solution Explorer, expanding folders as you name them.

```
MauiHighFidelityDashboard/
├── MauiProgram.cs      ← entry point + dependency-injection container
├── App.xaml(.cs)       ← application object, global resources, creates the window
├── AppShell.xaml(.cs)  ← Shell navigation container (the route table)
├── Views/              ← full pages (dashboard, detail, print preview, popups)
├── Components/         ← 8 reusable ContentViews the dashboard is composed from
├── ViewModels/         ← state + commands (the "controllers")
├── Models/             ← plain data classes + Result<T>
├── Services/           ← data access + printing; Interfaces/ holds the contracts
├── Converters/         ← IValueConverters used inside XAML bindings
├── Resources/          ← colors, fonts, styles, images — the design system
└── Platforms/          ← per-OS bootstrap only, zero feature code
```

> "Everything you saw in Part 2 lives in the **shared** folders — the `Platforms`
> folder only contains what each OS needs to boot. That's the point of MAUI:
> one codebase, native apps."

### 3.2 Startup flow — from launch to a populated screen [7:15–8:45]

**[SHOW]** `MauiProgram.cs`, then `MainViewModel.cs`.

```
MauiProgram.CreateMauiApp()        1. DI container built: interface → implementation
        ↓
App → AppShell → MainPage          2. Shell shows the dashboard page
        ↓
DI injects MainViewModel           3. constructor injection, set as BindingContext
        ↓
OnAppearing → InitializeAsync      4. LoadDataCommand fires
        ↓
Task.WhenAll(...)                  5. all seven datasets load in PARALLEL
        ↓
IDashboardDataService              6. the ViewModel calls the INTERFACE — it doesn't
        ↓                             know or care where the data comes from
StaticDashboardDataService         7. returns Result<T>: success + data, or failure + message
        ↓
ObservableCollections filled       8. collections raise CollectionChanged
        ↓
XAML data binding                  9. the UI updates itself — nothing calls "refresh"
```

> "In `MauiProgram` I register `IDashboardDataService` against
> `StaticDashboardDataService`, `IPrintService` against `PrintService`, plus the
> ViewModels and pages. The page receives its ViewModel through **constructor
> injection**. When the page appears, `InitializeAsync` loads all seven collections
> **in parallel** with `Task.WhenAll` — cards, revenue cards, activities, orders,
> pages, traffic, and sales points."

> "Every service call returns a **`Result<T>`** — success with data or failure with
> a message — so no exception ever crosses a layer boundary; the ViewModel just
> checks `IsSuccess`. And because the UI is bound to `ObservableCollection`s,
> filling them **is** rendering — MVVM's key difference from MVC: the View is
> permanently bound to the ViewModel, so every state change flows to the screen
> automatically through `INotifyPropertyChanged`."

### 3.3 One interaction, end to end [8:45–9:45]

**[SHOW]** `MainViewModel.cs` — `OnSearchTextChanged`, `RefreshOrdersView`, `AddOrderAsync`.

> "Take the live search as one concrete round-trip. The search box's `Text` is
> two-way-bound to a source-generated `SearchText` property from
> **CommunityToolkit.Mvvm**. Every keystroke triggers `OnSearchTextChanged`, which
> resets to page one and calls `RefreshOrdersView` — that filters the master list,
> slices the current page, rebuilds the page numbers, and updates the footer text.
> No code ever touches a UI control; the table re-renders purely through binding."

> "Add and delete follow the same pattern: a `[RelayCommand]` opens a popup,
> `await`s the user's result, mutates the master list, and calls the same
> `RefreshOrdersView`. One rendering path for every mutation — that's why the
> footer, badges, and pagination can never drift out of sync."

### 3.4 The service layer and API-readiness [9:45–10:30]

**[SHOW]** `Services/Interfaces/IDashboardDataService.cs`, then `ApiDashboardDataService.cs`.

> "The service layer is where this becomes production-shaped. `IDashboardDataService`
> is the contract; `StaticDashboardDataService` is the in-memory implementation used
> today. But `ApiDashboardDataService` is **already written** — an `HttpClient`
> implementation for the real REST API. Because the ViewModels only know the
> interface, going live is **one changed line** in `MauiProgram` — no ViewModel,
> View, or Model changes."

> "Printing works the same way: `IPrintService` is the contract, and `PrintService`
> holds the only platform-conditional code in the app — WebView2 printing on
> Windows, `PrintManager` on Android."

### 3.5 Converters, styles, and the design system [10:30–11:00]

**[SHOW]** `Converters/StatusColorConverter.cs`, then `Resources/Styles/Colors.xaml`.

> "Small formatting decisions live in **converters** — status-to-color for the
> badges, hex-to-color for model-driven accents, amount-to-currency. And every
> color, font, and control style is centralized in `Resources/Styles` as
> ResourceDictionaries — a single source of truth for the design, which is how
> the pixel-fidelity stays consistent across every component."

---

## CLOSING [11:00–11:30]

**[SHOW]** Back to the running app, full screen.

> "So that's the project: a pixel-faithful dashboard, every feature genuinely
> functional — navigation, custom-drawn charts, live search, pagination, add,
> delete, real printing, responsive layout — built on clean MVVM with an
> interface-driven service layer that's one line away from a real backend.
> One codebase, Windows and Android, native performance. Thanks for watching."

---

## Recording checklist (demo actions in order)

1. Launch maximized; mouse-sweep the layout while introducing.
2. Click a sidebar item → detail page → back.
3. **Last Month Summary** → popup → close.
4. Sales chart: click **WEEKLY**, then **MONTHLY**.
5. Analytics card filter → pick *Weekly*.
6. Order table: search `italy` → clear → page 2 → select row → ℹ info →
   ➕ add order (save) → select it → 🗑 delete (confirm) → 🖨 print → cancel OS dialog.
7. Resize below 980 px → hamburger + stacked layout → restore.
8. IDE: folder tree → `MauiProgram.cs` → `MainViewModel.cs` →
   `IDashboardDataService.cs` → `Colors.xaml` → back to the app.
