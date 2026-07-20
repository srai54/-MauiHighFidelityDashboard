# Backend Integration — HighFidelity.Api (EF Core, layered) + SQL Server

This document explains **what** was built, **why** it was built that way, and **what changes** it brings to the app. It ends with interview questions & answers on the topic.

> **Revision history:** the API started as a Dapper-based Minimal API (read-only) living in this repo's `Api/` folder. It was rebuilt as a layered enterprise-style service — Controllers → Services → Repositories → EF Core → SQL Server — with full CRUD, validation, retry, centralized error handling, and a real DB-connectivity health check. It has now been **split into its own repository**, [srai54/HighFidelity-Api](https://github.com/srai54/HighFidelity-Api), with full git history preserved via `git subtree split`. This repo (the MAUI app) no longer contains any backend source.

---

## 1. Three-tier separation — now across two repos

| Tier | Location | Responsibility |
|---|---|---|
| **Frontend (FE)** | *this repo* — `MauiHighFidelityDashboard.csproj` | .NET MAUI UI, ViewModels, `IDashboardDataService` consumer. Knows nothing about SQL or EF — only HTTP + JSON via `ApiDashboardDataService`. |
| **Backend (BE)** | separate repo — [srai54/HighFidelity-Api](https://github.com/srai54/HighFidelity-Api) | ASP.NET Core Web API. Owns HTTP routing, validation, and business rules. Knows nothing about MAUI, XAML, or the FE's view models. |
| **Database (DB)** | SQL Server `HighFidelity` (LocalDB in dev) | Owns the schema and data. Reached only through EF Core from the BE — the FE never touches SQL directly. |

```
┌────────────── FE (this repo) ──┐   ┌──────── BE (github.com/srai54/HighFidelity-Api) ─────────┐   ┌── DB ──┐
│ MainViewModel                  │   │ Controllers/DashboardController   ← HTTP in/out, thin     │   │        │
│   ↓ IDashboardDataService       │   │        ↓                                                  │   │ SQL    │
│ Services/ApiDashboardDataService│──▶│ Services/DashboardService         ← validation, rules      │──▶│ Server │
│   (HttpClient, JSON, over the   │   │        ↓                                                  │   │        │
│    network — no shared source)  │   │ Repositories/DashboardRepository  ← EF Core queries        │   │        │
│ Services/ApiSettings            │   │        ↓                                                  │   │        │
│   (which base URL to hit)       │   │ Data/AppDbContext → DbSet<T> → SQL Server                 │   │        │
└─────────────────────────────────┘   └────────────────────────────────────────────────────────────┘   └────────┘
```

Each arrow is an interface (`IDashboardDataService` on the FE side, `IDashboardService`/`IDashboardRepository` on the BE side), so any layer can be swapped or unit-tested with a fake in place of its neighbor. Crossing the repo boundary is now a hard requirement, not just a convention — the only thing connecting them is the HTTP contract in §2.

For the backend's internal folder layout (Controllers/Services/Repositories/Data/Models/DTOs/Mappings/Migrations), see the [HighFidelity-Api README](https://github.com/srai54/HighFidelity-Api#architecture).

---

## 2. API endpoints

| Endpoint | Table | Returns |
|---|---|---|
| `GET /api/dashboard/cards` | `DashboardCards` | 4 summary cards (wallet, referral, sales, earning) |
| `GET /api/dashboard/revenue-cards` | `RevenueCards` | 4 analytics cards (bar/area/line mini-charts) |
| `GET /api/dashboard/activities` | `Activities` | 5 timeline entries |
| `GET /api/dashboard/orders` | `Orders` | All orders (invoice, customer, country, price, status) |
| `GET /api/dashboard/traffic` | `TrafficSources` | 3 donut-chart segments |
| `POST /api/dashboard/orders` | `Orders` | Validates input, inserts a row; the **database** assigns `Id` (identity) and `Invoice` (`MAX+1`); returns `201 Created` with the new row |
| `DELETE /api/dashboard/orders?ids=3&ids=17` | `Orders` | Bulk delete by primary key; returns `{"deleted": n}` |
| `GET /health` | — | **Readiness** probe — actually calls `Database.CanConnectAsync()`; `200` + `{"status":"ok","database":"connected"}`, or `503` + `"degraded"/"unreachable"` if SQL Server can't be reached |

### Write path (Instant Add / Bulk Delete)

```
Add button → AddOrderPopup → MainViewModel.AppendOrderAsync
  → IDashboardDataService.AddOrderAsync
  → POST /api/dashboard/orders
      → DashboardController.AddOrder            (parses request, maps exceptions to 400)
      → DashboardService.AddOrderAsync          (validates: non-empty customer/country, price > 0)
      → DashboardRepository.AddOrderAsync       (MAX(Invoice)+1, EF Core Add + SaveChangesAsync)
  → created row (DB-assigned Id + Invoice) returned → appended to the list → last page shown
```

If any layer fails — validation, or the database being unreachable — the ViewModel shows the error and the on-screen list stays untouched. It can never drift from what's actually in SQL Server. Bulk delete follows the same path, keyed on `Id` (not `Invoice`, which the reference screenshot intentionally repeats across rows and so is not unique).

---

## 3. Why it was done this way

1. **Controllers + layered architecture over a flat Minimal API.** Once the API grew real business rules (order validation, DB-assigned invoice numbers, bulk delete with confirmation), a flat `Program.cs` full of `app.MapGet(...)` lambdas would mix HTTP concerns with business rules in the same place. Splitting into **Controller → Service → Repository** means: the controller only translates HTTP ↔ calls; the service is where validation and future rules (credit checks, audit logging, permissions) belong; the repository is the only place that knows EF Core exists. Each is independently unit-testable behind its interface.
2. **EF Core over Dapper.** The API is no longer read-only — it has real writes (`AddOrderAsync`, `DeleteOrdersAsync`) with a business rule (server-assigned invoice number) and will likely grow more relationships over time. EF Core's `DbContext`/`DbSet<T>` gives change tracking, `SaveChangesAsync` transactions, and a migrations story for free — Dapper would mean hand-writing all of that. `AsNoTracking()` is applied to every read-only `Get*Async` query specifically because those rows are never updated in the same context — it skips EF's change-tracking snapshot for a straightforward perf win with no behavior change.
3. **Manual DTO mapping (`EntityMappings`), no AutoMapper.** Five entities, one property renaming apart (none, actually — they're identical shapes). A reflection-based mapper adds a dependency, a convention to learn, and a debugging step (“why didn’t this field map?”) for zero benefit at this scale. Explicit `ToDto()` extension methods are a one-line, compiler-checked, greppable alternative.
4. **DTOs are a separate type from EF entities.** Controllers never return `Order` (the EF entity) directly — they return `OrderDto`. This avoids ever accidentally serializing an EF Core proxy object (which can carry lazy-loading references) and keeps the wire contract stable even if the entity's internal shape changes.
5. **`EnableRetryOnFailure()` on the SQL Server connection.** Transient errors (a momentary network blip, SQL Server briefly recycling a connection) are common enough in real deployments that retrying 2-3 times before failing is standard practice — it costs one line and turns "occasional 500 for no visible reason" into "occasionally 200ms slower."
6. **Centralized exception handling (`UseExceptionHandler`).** Without it, an unhandled exception (e.g., the database is down) returns a raw ASP.NET Core error page or an abrupt connection reset — not something a client should have to parse. Now every unexpected failure returns the same JSON shape (`{"error": "...", "detail": "..." }`), with `detail` only populated in Development so production never leaks internals.
7. **A health check that actually checks something.** The original `/health` returned a hardcoded `{"status":"ok"}` regardless of whether SQL Server was reachable — a liveness probe, not a readiness probe. It now calls `Database.CanConnectAsync()` so an orchestrator (or a person debugging "why is the dashboard blank") gets a real answer.
8. **FE-side `ApiSettings.cs`.** The platform-specific base URL (`10.0.2.2` for the Android emulator vs `localhost` elsewhere) used to live inline inside `MauiProgram.cs`'s DI registration. It's now its own file so "which backend am I talking to" is a one-file answer, separate from "how is DI wired" — the FE's equivalent of the BE's `appsettings.json`.
9. **`IDashboardDataService` on the FE is unchanged.** Because the FE was always coded against an interface rather than concrete HTTP calls, this entire backend rewrite — Dapper → EF Core, Minimal API → Controllers — required **zero FE changes** beyond the config extraction in point 8. That's the payoff of the original abstraction.

---

## 4. What changes this brings

- **Before this revision:** the API was a flat Dapper Minimal API, read-only for writes (Instant Add/Bulk Delete only touched an in-memory list).
- **Now:** full CRUD persists to SQL Server through a layered, testable architecture with input validation, transient-fault retry, and centralized error handling — the shape a real production team would recognize and extend.
- **Still unchanged:** the FE→BE HTTP contract (`api/dashboard/...` routes and JSON shapes), so the app didn't need touching except for the `ApiSettings` extraction.
- **New operational behavior:** `/health` now genuinely reflects DB connectivity; a 500 from any endpoint returns structured JSON instead of a raw error page; transient SQL blips retry instead of failing the request outright.

---

## 5. Interview Questions & Answers

**Q1. Why Controllers instead of Minimal API routes here?**
Once the API has real business logic (validation, server-assigned invoice numbers) and multiple responsibilities per resource (5 reads + create + delete on Orders), grouping them in a controller class with attribute routing keeps related actions together and gives a natural home for layering (constructor-injected service) versus a growing pile of `app.MapGet` lambdas in `Program.cs`.

**Q2. Walk through the layers for `POST /api/dashboard/orders`.**
`DashboardController.AddOrder` deserializes the body and catches `ArgumentException` → 400. It calls `DashboardService.AddOrderAsync`, which validates (non-empty customer/country, price > 0) and would host any future business rule (credit limits, audit logging). That calls `DashboardRepository.AddOrderAsync`, which computes `MAX(Invoice)+1`, adds the entity via `AppDbContext`, and calls `SaveChangesAsync()` — the only place that talks EF Core.

**Q3. Why does the repository return EF entities but the controller returns DTOs?**
Separation of "what the database looks like" from "what the wire contract looks like." `EntityMappings.ToDto()` converts between them in the service layer. This means the JSON contract stays stable even if the entity shape changes, and an EF Core change-tracking proxy is never accidentally serialized.

**Q4. Why `AsNoTracking()` specifically on the five `Get*Async` methods?**
EF Core builds a change-tracking snapshot of every entity it loads by default, so it can detect modifications for a future `SaveChangesAsync()`. These queries are pure reads — the results are mapped to DTOs and returned; nothing is ever mutated on them in the same `DbContext` — so tracking is pure overhead. `AsNoTracking()` skips it.

**Q5. What does `EnableRetryOnFailure()` protect against, and what doesn't it protect against?**
It retries a query automatically (with backoff, capped at 3 attempts / 5s max delay here) when EF Core's SQL Server provider classifies the failure as transient — a dropped connection, a brief unavailability. It does **not** retry on business errors like a constraint violation or bad SQL — those are deterministic and would just fail the same way three times.

**Q6. Why does `/health` call the database instead of just returning 200?**
A process that's running but can't reach SQL Server is not actually healthy from the caller's point of view — the dashboard would load and show nothing. `Database.CanConnectAsync()` gives a true readiness signal; the endpoint returns `503` if it fails, which a real deployment's orchestrator (or load balancer) would use to pull the instance out of rotation.

**Q7. What does the centralized exception handler protect against that a try/catch in each controller action wouldn't?**
Any *unexpected* exception — one nobody thought to catch locally, like the DB connection dropping mid-query — still needs a sane HTTP response instead of an ASP.NET Core default error page (which can leak stack traces) or a raw connection reset. `UseExceptionHandler` is the single backstop; expected errors (like `ArgumentException` from validation) are still caught locally in the controller because *that* response should be `400`, not a generic `500`.

**Q8. Why manual mapping instead of AutoMapper?**
At five 1:1-shaped entity/DTO pairs, AutoMapper's convention-based reflection mapping adds a dependency and a "magic" step that's harder to debug than a compiler-checked one-line `new(...)` extension method. AutoMapper earns its place when there are dozens of mappings or non-trivial transformations — not here.

**Q9. Why does `IDashboardDataService` on the MAUI side need zero changes when the whole backend was rewritten?**
Because the FE was always coded against an interface abstraction, not concrete HTTP/Dapper details. As long as the *HTTP contract* (routes, JSON shapes) stays the same, the backend's internal implementation — Dapper vs EF Core, Minimal API vs Controllers — is invisible to the client. This is the core payoff of programming to an interface.

**Q10. Why does `Order.Invoice` allow duplicates, and why is deletion keyed on `Id` instead?**
The reference UI screenshot the app is pixel-matching repeats invoice `12386` across five rows on purpose (template data). `Id` is the EF Core-managed identity primary key and is always unique, so all mutation (delete, and any future update) is keyed on it — never on `Invoice`.

**Q11. How would you take this further toward production?**
HTTPS with a real certificate; JWT bearer authentication; move the connection string to environment variables/Key Vault instead of `appsettings.json`; paging on `/orders`; response caching for the read endpoints; `IHttpClientFactory` + Polly on the FE instead of a single long-lived `HttpClient`; structured logging/telemetry (Serilog + Application Insights); real EF Core migrations going forward instead of the one-time no-op baseline.

**Q12. Why is the `InitialCreate` migration's `Up()` empty?**
The schema already existed from `database/seed.sql` before EF Core migrations were introduced. Rather than have EF try to (re)create tables that are already there, the baseline migration is a deliberate no-op — it just registers the migration in `__EFMigrationsHistory` so future schema changes produce normal, incremental migrations from a known starting point.

**Q13. Why split the backend into its own repository instead of keeping it in a subfolder here?**
A subfolder (monorepo) is the right call while one person owns both sides and they change together in lockstep — which is how this project started. A separate repo earns its place once the backend becomes its own concern: it can be versioned, deployed, and released on its own schedule independent of the mobile app's build; it can be consumed by more than one client in the future (a web dashboard, another mobile app) without dragging MAUI/XAML into an unrelated checkout; and access control can differ (backend infra secrets vs. mobile app signing keys). The split preserved full git history for the `Api/` folder via `git subtree split`, so [HighFidelity-Api](https://github.com/srai54/HighFidelity-Api)'s log still shows the real Dapper → EF Core evolution rather than starting from a single flattened commit.

**Q14. What was *not* chosen, and why?**
Putting the two projects on separate **branches** of the same repo was considered and rejected — branches are mutually exclusive checkouts of one codebase over time (feature/release branches), but the FE and BE need to run *simultaneously* as two independent processes talking over HTTP. Branches can't model "two things that coexist," only "two versions of one thing," so that structure would have made it impossible to have both checked out at once.
