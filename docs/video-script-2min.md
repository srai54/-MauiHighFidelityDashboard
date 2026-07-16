# 2-Minute Intro Video Script (ready to record)

> Speaking pace ~150 words/min → ~300 words total. Timings are cues, not hard stops.
> Screen plan: app running on Windows (maximized) → quick resize demo → VS/IDE folder tree → app again.

---

## [0:00–0:15] — Opening (show the running dashboard)

"Hi, I'm Rajib. This is a high-fidelity admin dashboard I built with **.NET MAUI** — a single C# and XAML codebase that runs on **Windows and Android**. It recreates the given design screenshot pixel-for-pixel: sidebar navigation, sales spline chart, KPI cards, analytics cards, an activity timeline, and a live order table."

## [0:15–0:45] — Structure & MVVM (show the folder tree)

"The project follows the **MVVM structure** from the assignment: **Views** for pages, **Components** for eight reusable ContentViews, **ViewModels**, **Models**, and styles centralized in **Resources/Styles** as ResourceDictionaries. On top of that I added a **Services** layer with its interfaces in a separate folder — that's what keeps it industry-grade: ViewModels talk only to `IDashboardDataService`, so today the data is a static in-memory service, and swapping in the real REST API is **one line** in the DI container in `MauiProgram` — no ViewModel changes."

## [0:45–1:15] — Logic & code flow (show a click or refresh)

"The flow is: the page loads, `MainViewModel.InitializeAsync` fires and fetches all datasets **in parallel** with `Task.WhenAll`. Every service call returns a **`Result<T>`** — success or failure, no unhandled exceptions — and the results land in `ObservableCollection`s, so the XAML updates itself through data binding. Commands come from **CommunityToolkit.Mvvm** source generators. The charts are fully custom — `GraphicsView` and `IDrawable`, no third-party chart library."

## [1:15–1:40] — Standout features (demo print + popup)

"Two features I want to highlight. **Printing is real**: the print button builds an HTML report of the filtered orders, shows a preview, and hands it to the OS — the WebView2 print dialog on Windows, Android's PrintManager on mobile — I can print to paper or PDF. And the **Last Month Summary** opens a structured popup with proper label-value rows."

## [1:40–2:00] — Responsiveness & close (resize the window live)

"Finally, the layout is **responsive**: below a 980-pixel breakpoint the sidebar collapses into a hamburger overlay, the cards restack into a single column, and the table pans horizontally — the same behavior on a phone, a small window, or 400% zoom. That's the app: one codebase, native performance, clean MVVM, and ready for a real backend. Thanks for watching."

---

## Quick demo checklist (do while speaking)

1. Launch app maximized — hover over the chart tabs, click **WEEKLY**.
2. Show folder tree in IDE (Views / Components / ViewModels / Models / Services / Converters / Resources).
3. Click **Last Month Summary** → show structured popup → Close.
4. Click the 🖨 print button → preview → **Print** → OS dialog appears → Cancel.
5. Drag the window narrow → hamburger + stacked layout → drag back wide.
