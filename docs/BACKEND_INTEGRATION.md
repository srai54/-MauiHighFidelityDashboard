# Backend Integration — HighFidelity.Api + SQL LocalDB

This document explains **what** was built, **why** it was built that way, and **what changes** it brings to the app. It ends with interview questions & answers on the topic.

---

## 1. What was done

| Piece | Location | Purpose |
|---|---|---|
| ASP.NET Core Minimal Web API | `Api/HighFidelity.Api/` | Serves dashboard data as JSON over HTTP |
| SQL schema + dummy data script | `Api/database/seed.sql` | Creates 5 tables in the `HighFidelity` LocalDB database and inserts sample rows |
| Frontend switch to API | `MauiProgram.cs` | DI now registers `ApiDashboardDataService` (HTTP) instead of `StaticDashboardDataService` (hard-coded) |
| Android cleartext permission | `Platforms/Android/AndroidManifest.xml` | Allows the emulator to call the dev API over plain HTTP |

### Architecture (data flow)

```
SQL LocalDB (HighFidelity)          ASP.NET Core Minimal API              .NET MAUI app
┌────────────────────┐   Dapper    ┌──────────────────────────┐   HTTP   ┌─────────────────────────┐
│ DashboardCards     │ ──────────▶ │ GET /api/dashboard/cards │ ───────▶ │ ApiDashboardDataService │
│ RevenueCards       │             │ .../revenue-cards        │  JSON    │  → Result<T>            │
│ Activities         │             │ .../activities           │          │  → MainViewModel        │
│ Orders             │             │ .../orders               │          │  → XAML bindings        │
│ TrafficSources     │             │ .../traffic              │          └─────────────────────────┘
└────────────────────┘             └──────────────────────────┘
```

### API endpoints

| Endpoint | Table | Returns |
|---|---|---|
| `GET /api/dashboard/cards` | `DashboardCards` | 4 summary cards (wallet, referral, sales, earning) |
| `GET /api/dashboard/revenue-cards` | `RevenueCards` | 4 analytics cards (bar/area/line mini-charts) |
| `GET /api/dashboard/activities` | `Activities` | 5 timeline entries |
| `GET /api/dashboard/orders` | `Orders` | 30 orders (invoice, customer, country, price, status) |
| `GET /api/dashboard/traffic` | `TrafficSources` | 3 donut-chart segments |
| `POST /api/dashboard/orders` | `Orders` | Inserts an order; the **database** assigns `Id` (identity) and `Invoice` (`MAX+1`); returns the created row (`201 Created`) |
| `DELETE /api/dashboard/orders?ids=3&ids=17` | `Orders` | Bulk delete by primary key; returns `{"deleted": n}` |
| `GET /health` | — | Liveness probe `{"status":"ok"}` |

### Write path (Instant Add / Bulk Delete)

UI actions persist to the database first, and only update the screen after the API confirms:

```
Add button → AddOrderPopup → MainViewModel.AppendOrderAsync
  → IDashboardDataService.AddOrderAsync
  → POST /api/dashboard/orders → Dapper INSERT ... OUTPUT INSERTED.*
  → created row (with DB-assigned Id + Invoice) returned → added to the list → last page shown
```

If the API call fails, the ViewModel shows the error and the UI stays unchanged — the list can never drift out of sync with the database. Bulk delete follows the same pattern, keyed on `Id` (not `Invoice`, which the reference screenshot intentionally repeats and so is not unique).

### Insert commands (dummy data)

The full script is [`Api/database/seed.sql`](../Api/database/seed.sql) — it is **idempotent** (`DROP TABLE IF EXISTS` + `CREATE` + `INSERT`), so it can be re-run any time to reset data. Example insert:

```sql
INSERT INTO dbo.Orders (Invoice, Customer, Country, Price, Status) VALUES
(12386, N'Charly Dues', N'Brazil', 299, N'Process'),
(12386, N'Marko',       N'Italy', 2642, N'Open');
```

Font Awesome icons are stored as Unicode codepoints so the app renders them directly:

```sql
INSERT INTO dbo.DashboardCards (Title, Amount, AmountDisplay, Icon, ThemeColorHex)
VALUES (N'Wallet Ballance', 4567.53, N'$4,567.53', NCHAR(0xF521), '#F7284A');  -- crown glyph
```

Run it with:

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -d HighFidelity -i Api\database\seed.sql
```

## 2. How to run everything

```powershell
# 1. Seed the database (once, or to reset data)
sqlcmd -S "(localdb)\MSSQLLocalDB" -d HighFidelity -i Api\database\seed.sql

# 2. Start the API (stays running on http://localhost:5199)
dotnet run --project Api\HighFidelity.Api

# 3. Run the MAUI app (separate terminal)
dotnet build MauiHighFidelityDashboard.csproj -f net10.0-windows10.0.19041.0 -t:Run
```

Android emulator: the app automatically uses `http://10.0.2.2:5199/` (the emulator's alias for the host's localhost).

## 3. Why it was done this way

1. **The frontend was already prepared for this.** `IDashboardDataService` existed with two implementations — static and API. The API's routes were designed to match `ApiDashboardDataService`'s expected endpoints exactly (`api/dashboard/cards`, etc.), so **zero ViewModel/XAML changes** were needed. This is the payoff of coding against an interface.
2. **Minimal API instead of MVC controllers** — five read-only endpoints don't justify controller classes, attribute routing, and filters. Minimal APIs are the modern ASP.NET Core default for small services: less ceremony, same middleware pipeline.
3. **Dapper instead of EF Core** — the queries are five flat `SELECT`s with no relationships, no change-tracking, no migrations story needed (the schema lives in one SQL file). Dapper maps rows to DTOs in one line and is faster to set up and execute. EF Core would earn its place once writes/relationships appear.
4. **DTOs duplicated in the API instead of sharing the Models project** — the MAUI csproj multi-targets `net10.0-windows/android`; referencing it from a plain `net10.0` web project would fail. Records mirroring the model property names keep the contract explicit, and System.Text.Json's case-insensitive binding on the client links them.
5. **Connection-per-request with `SqlConnection`** — ADO.NET connection pooling makes opening a connection per request cheap and safe under concurrency (a shared singleton connection would serialize requests and break).
6. **`Result<T>` failure path untouched** — if the API is down, `ApiDashboardDataService` catches the exception and returns `Result.Failure`, which the ViewModel already surfaces. The app degrades gracefully rather than crashing.
7. **Fixed port 5199 in `appsettings.json`** — a deterministic port means the MAUI base address never drifts after a `dotnet new`-style relaunch.

## 4. What changes this brings

- **Before:** all dashboard data was hard-coded in `StaticDashboardDataService.cs`; changing a number meant recompiling the app.
- **After:** data lives in SQL tables. Update a row in SSMS, restart/refresh the app, and the UI reflects it — no rebuild. The app is now a real 3-tier system (UI → API → DB).
- The static service is **still in the codebase** as an offline/demo fallback — swap one DI line in `MauiProgram.cs` to go back.
- New requirement: the API must be running for the dashboard to show data; otherwise the existing error-handling path shows the failure message.

---

## 5. Interview Questions & Answers

**Q1. Why does the MAUI app talk to an interface (`IDashboardDataService`) instead of the API client directly?**
Dependency inversion. ViewModels depend on the abstraction, so the data source can be swapped (static → HTTP → cached) in one DI registration without touching ViewModels or XAML. It also makes ViewModels unit-testable with a fake service.

**Q2. Why `10.0.2.2` on Android instead of `localhost`?**
Inside the Android emulator, `localhost` is the emulator's own loopback, not the host PC's. Google reserves `10.0.2.2` as an alias for the host machine's loopback interface. (Physical devices need the PC's LAN IP instead.)

**Q3. Why did HTTP calls need `android:usesCleartextTraffic="true"`?**
Since Android 9 (API 28), cleartext (non-HTTPS) traffic is blocked by default. The dev API runs on plain HTTP, so the manifest opts in. In production you'd use HTTPS and remove the flag (or use a network security config scoped to debug builds).

**Q4. Minimal API vs. controllers — when would you pick each?**
Minimal APIs: small services, few endpoints, low ceremony — routes and handlers in `Program.cs`. Controllers: large surface areas that benefit from grouping, model binding conventions, filters, and inheritance. Both share the same middleware/DI/hosting.

**Q5. Dapper vs. EF Core — why Dapper here?**
Read-only, flat queries with no navigation properties. Dapper is a micro-ORM: you write SQL, it maps rows to objects — minimal overhead, no migrations/change-tracker to configure. EF Core pays off with complex object graphs, LINQ, migrations, and unit-of-work writes.

**Q6. Is opening a new `SqlConnection` per request expensive?**
No — ADO.NET pools connections per connection string. `new SqlConnection` + `Open` typically grabs an already-open pooled connection. The anti-pattern is the opposite: a shared long-lived connection, which isn't thread-safe under concurrent requests.

**Q7. How does JSON casing work between the API and the app?**
ASP.NET Core serializes to camelCase by default (`title`, `amountDisplay`). `HttpClient.GetFromJsonAsync` uses `JsonSerializerDefaults.Web`, which is case-insensitive on deserialization, so camelCase JSON binds to the PascalCase C# properties without attributes.

**Q8. What happens in the UI if the API is down?**
`ApiDashboardDataService` wraps every call in try/catch and returns `Result<T>.Failure(message)` instead of throwing. The ViewModel checks `IsFailure` and surfaces the error state — the app never crashes on network failure.

**Q9. Why store Font Awesome icons as `NCHAR(0xF521)` in the database?**
The glyphs are Unicode private-use-area characters. Storing the actual character in an `NVARCHAR` column means the API returns it in JSON as a normal string, and the MAUI `Label` with `FontFamily="FontAwesome"` renders it directly — no mapping table needed on the client.

**Q10. How would you take this to production?**
HTTPS with a real certificate; authentication (JWT bearer); move the connection string to environment variables/Key Vault; EF Core migrations or DbUp for schema versioning; paging on `/orders`; response caching; health checks wired to the orchestrator; retry policy (Polly) + `IHttpClientFactory` on the client instead of a raw `HttpClient`.

**Q11. Why `IHttpClientFactory` over `new HttpClient` in real apps?**
Long-lived `HttpClient` instances avoid socket exhaustion but pin DNS results; `IHttpClientFactory` recycles the underlying handlers, solving both, and adds named/typed clients plus Polly integration points. Here a single singleton `HttpClient` for the app's lifetime is acceptable, but the factory is the production answer.

**Q12. Why is the seed script idempotent, and what's the trade-off?**
`DROP TABLE IF EXISTS` + recreate means anyone can reset to a known state with one command — ideal for demos/tests. The trade-off: it destroys data, so it's a dev-only pattern; production uses incremental migrations instead.
