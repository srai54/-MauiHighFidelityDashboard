# Client Demo Video — Recording Script

**Format:** ~4 minutes total — **2½–3 minutes** walking through every chart and KPI in the UI, then **~1 minute** high-level code walkthrough.
**Audience:** the client — professional tone, plain English in the UI part, only light technical language in the code part.
**Speaking pace:** the SAY blocks are written for a natural ~140 words/minute; if you finish a block early, let the screen breathe rather than rushing ahead.

---

## Before you record — checklist

- [ ] Windows app running **maximized** (`run.cmd` → option 1); do a `Ctrl`-refresh of your eyes over the data — it should show the dashboard exactly at the top.
- [ ] Android emulator booted with the app on the dashboard (`run.cmd` → option 2) — you'll show it for ~10 seconds.
- [ ] Editor (VS/VS Code) open behind the app with these files in tabs, left-to-right: `MainPage.xaml`, `MainViewModel.cs`, `Services/StaticDashboardDataService.cs`, `Charts/ChartData.cs`, `Components/SalesChartView.xaml.cs`.
- [ ] Close mail/chat notifications; hide the taskbar clock area if possible.
- [ ] Mic check — record 10 seconds, play it back.

---

## PART 1 — The Application (0:00 – 2:55)

### Opening (0:00 – 0:20)

**SHOW:** Windows app, full dashboard, mouse still.

**SAY:**
"Hi, I'm Shivanshu. This is the admin dashboard I built for the assignment — a pixel-accurate recreation of the reference design, developed in .NET MAUI. That means this exact same C# codebase runs natively on Windows, which you're seeing now, and on Android, which I'll show at the end. Let me walk you through the screen."

### KPIs — header and summary strip (0:20 – 0:55)

**SHOW:** Hover along the header numbers, click **Last Month Summary**, close it, then move across the four summary tiles.

**SAY:**
"At the top are the headline KPIs — current month earnings, three thousand four hundred sixty-eight dollars, and eighty-two sales. The 'Last Month Summary' button opens a popup with the previous month's key figures, so you can compare periods without leaving the page.

Below that is the summary strip — four business metrics, each with its own icon and color: Wallet Balance, Referral Earning, Estimate Sales, and total Earning. All of these come from a data service, not hardcoded labels — so when a real API is connected, these tiles update automatically."

### Sales chart (0:55 – 1:35)

**SHOW:** Click **WEEKLY**, then **MONTHLY**, back to **DAILY**. Click the **Online** legend dot (Store disappears), click it again (both return).

**SAY:**
"The main sales chart is completely custom-drawn — the smooth curves, the soft area fills, and the grid are rendered with MAUI's graphics engine, no third-party chart library.

There are two series — Online sales in blue, Store sales in orange. The tabs switch the period: Daily, Weekly, Monthly, Yearly — each with its own dataset and axis labels.

And the legend is interactive: clicking a series isolates it, so you can study Online sales alone; clicking again brings both back. Small touches like this make the dashboard feel like a product, not a mock-up."

### Traffic donut (1:35 – 1:55)

**SHOW:** Mouse circles the donut, then rests under the three percentages.

**SAY:**
"The Traffic card shows where visitors come from — Facebook, YouTube, and direct search. The donut and the percentage legend are driven by the same single data source, so a change in the numbers redraws the ring and updates the labels together — they can never disagree."

### Analytics cards (1:55 – 2:25)

**SHOW:** Sweep across the four cards; on **Bounce Rate**, click the **Monthly** chip, pick **Yearly** — value and squiggle both change; set it back.

**SAY:**
"This row holds four analytics cards, each a different mini-chart: Revenue Status with bar charts in two color themes, Page View with an area wave, and Bounce Rate with a dotted trend line.

Bounce Rate is interactive — this little period selector opens a picker, and choosing a period updates both the headline value and the curve together, because they come from the same record. All four cards are actually one reusable component with different data and colors plugged in."

### Activities and Order Status (2:25 – 2:55)

**SHOW:** Scroll to the bottom row. Type `italy` in search, clear it. Click **+ Add**, fill a quick order, save — table jumps to the last page showing it. Select that row, click delete, confirm. Click the **info** button, close. Hover the print button.

**SAY:**
"Recent Activities is a timeline of team actions. Next to it, the Order Status table is fully functional: live search across customer, country, and status; pagination — five rows per page; and full order management. I can add an order through this popup — notice the table jumps straight to the page showing it. I can select a row and delete it, with a confirmation step so nothing is removed by accident. The info button summarizes the current view — totals by status and overall value — and this button prints the filtered list."

*(Optional 10-second beat if time allows)* **SHOW:** Drag the window narrow → layout stacks, hamburger appears; then 5 seconds of the Android emulator.
**SAY:** "The layout is responsive — on a narrow window or a phone it reorganizes into a single column — and here's the same app running natively on Android."

---

## PART 2 — Code Walkthrough (2:55 – 3:55)

### Architecture in one screen (2:55 – 3:20)

**SHOW:** Editor — Solution Explorer expanded: `Views`, `Components`, `ViewModels`, `Models`, `Services`, `Charts`, `Converters`.

**SAY:**
"One minute on how it's built. The solution follows MVVM with a clean folder structure — Views and reusable Components for the UI, ViewModels for the logic, Models for the data shapes, and a Services layer behind an interface. Today the service returns demo data; swapping in the real REST API is a one-line change in the app's startup, because the ViewModels only know the interface."

### The generic chart layer (3:20 – 3:40)

**SHOW:** Open `Charts/ChartData.cs`, scroll slowly over `SalesByPeriod` and the mini-chart arrays.

**SAY:**
"All chart data and colors are declared once, in a global Charts layer. Every chart on the screen — the spline, the donut, the mini bars — is a small reusable drawing class fed from this file. So adding a new chart tomorrow means adding a few lines of data here and pointing a card at it — no new rendering code."

### Interaction code + wrap (3:40 – 3:55)

**SHOW:** `SalesChartView.xaml.cs` briefly (tab & legend handlers), then back to the running app.

**SAY:**
"Interactions like the period tabs and the legend filter live in small, focused component classes — each about a page of code.

So: one C# codebase, native on Windows and Android, pixel-faithful to the design, with production patterns throughout — dependency injection, MVVM, and a data layer that's API-ready. Thanks for watching — happy to dive deeper into any part."

---

## Shot list (for editing)

| # | Screen | Duration |
|---|---|---|
| 1 | Full dashboard, static | 20 s |
| 2 | Header KPIs + Last Month popup + summary strip | 35 s |
| 3 | Sales chart: tabs + legend solo | 40 s |
| 4 | Traffic donut | 20 s |
| 5 | Analytics cards + Bounce Rate period picker | 30 s |
| 6 | Search / add / delete / info / print on orders | 30 s |
| 7 | (Optional) narrow layout + Android emulator | 10 s |
| 8 | Solution explorer | 25 s |
| 9 | `ChartData.cs` scroll | 20 s |
| 10 | `SalesChartView.xaml.cs` + return to app | 15 s |

**Total ≈ 4:05** (3:55 without the optional beat).

## Delivery tips

- Record UI (Part 1) and code (Part 2) as **separate takes** — much easier to redo one.
- Do every mouse action **slowly**: click → pause one second → speak the result.
- If you fumble a line, pause two seconds and repeat the whole sentence — clean cut points for editing.
- Export at 1080p minimum; the chart hairlines blur below that.
