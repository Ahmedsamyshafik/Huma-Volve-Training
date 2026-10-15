# Store API — Educational ASP.NET Core Web API (3-Tier)

A small, intentionally simple Web API that demonstrates: 3-tier architecture, repository
pattern (no generic repository / unit of work), EF Core with SQL Server, soft delete with a
global query filter, `rowversion` optimistic concurrency, `SaveChanges` audit overrides,
server-side pagination, `AsNoTracking` + `Select()` projections, N+1 prevention, DTO
validation with Data Annotations, URL-segment API versioning (v1 vs v2) and versioned Swagger.

**Stack:** .NET 10, EF Core 10.0.13, SQL Server, Asp.Versioning 10.2.1, Swashbuckle 10.3.0, Serilog 10.0.0.

## Projects and dependency direction

```
Store.Api  ->  Store.Business  ->  Store.Data        (Store.Data references nothing)
```

| Project | Contains |
|---|---|
| `Store.Api` | Controllers, DTOs (with validation), API versioning, Swagger config, `Program.cs` |
| `Store.Business` | `ProductService`, `CategoryService`, `PagedResult<T>` |
| `Store.Data` | Entities, `AppDbContext`, entity configurations, seed data, repositories, read models |

## Setup

Prerequisites: .NET 10 SDK, SQL Server (LocalDB,Express, or full instance).

1. Connection string (defaults to integrated security on `localhost`):

   `Store.Api/appsettings.json` → `ConnectionStrings:DefaultConnection`

2. Run (applies the migration and seed data automatically on startup):

   ```bash
   cd Src/Store
   dotnet run --project Store.Api
   ```

   App listens on `http://localhost:5080` (see `Properties/launchSettings.json`).
   Swagger UI: `http://localhost:5080/swagger` (v1 and v2 documents).

   Manual migration alternative:

   ```bash
   dotnet ef database update --project Store.Data --startup-project Store.Api
   ```

## Endpoints

All routes are versioned by URL segment. Products exist in v1 and v2; categories in v1 only.

| Method | Route | Notes |
|---|---|---|
| POST | `/api/v1/products` | 201 + Location; 400 validation; 404 if category unknown |
| GET | `/api/v1/products?pageNumber=1&pageSize=10` | Items: `productId`, `name`, `price` |
| GET | `/api/v1/products/{id}` | Details **without** `categoryName` |
| PUT | `/api/v1/products/{id}` | Body must include original `rowVersion`; 409 on conflict |
| DELETE | `/api/v1/products/{id}` | Soft delete (204); 404 if missing |
| POST/GET/PUT/DELETE | `/api/v2/products[/{id}]` | Same services as v1, but responses **add `categoryName`** |
| POST | `/api/v1/categories` | 201 + Location |
| GET | `/api/v1/categories?pageNumber=1&pageSize=10` | Items: `id`, `name` |
| GET | `/api/v1/categories/{id}` | Includes `rowVersion` |
| PUT | `/api/v1/categories/{id}` | Body must include original `rowVersion`; 409 on conflict |
| DELETE | `/api/v1/categories/{id}` | Soft delete (204) |

Pagination: `pageNumber` ≥ 1, `pageSize` 1–100, otherwise 400. Responses carry
`totalCount`, `pageNumber`, `pageSize`, `totalPages`, `items`. Ordering is deterministic
(`Id`) before `Skip`/`Take` in SQL.

### Example requests

```bash
# v1 list (no categoryName)
curl "http://localhost:5080/api/v1/products?pageNumber=1&pageSize=10"

# v2 list (adds categoryName, single query with JOIN)
curl "http://localhost:5080/api/v2/products?pageNumber=1&pageSize=10"

# create
curl -X POST "http://localhost:5080/api/v1/products" -H "Content-Type: application/json" \
  -d '{"name":"Guitar","description":"Acoustic","price":199.99,"categoryId":1}'

# update (rowVersion comes from a prior GET; wrong/stale value -> 409)
curl -X PUT "http://localhost:5080/api/v1/products/26" -H "Content-Type: application/json" \
  -d '{"name":"Guitar Pro","price":249.99,"categoryId":1,"rowVersion":"AAAAAAAAB0A="}'

# soft delete
curl -X DELETE "http://localhost:5080/api/v1/products/26"
```

## Where each concept lives

| Concept | File(s) |
|---|---|
| 3-tier layout / references | `Store.sln`, the three `.csproj` files |
| `BaseEntity` (audit, soft delete, rowversion) | `Store.Data/Entities/BaseEntity.cs` |
| Product/Category + one-to-many | `Store.Data/Entities/*.cs`, `Store.Data/Configurations/ProductConfiguration.cs` |
| `rowversion` as concurrency token | `*.Configuration.cs` (`IsRowVersion()`), migration `..._InitialCreate.cs` |
| Global query filter (soft delete) | `Store.Data/AppDbContext.cs` (`OnModelCreating` loop over `BaseEntity` types) |
| `SaveChanges`/`SaveChangesAsync` audit override (UTC timestamps, preserve `CreatedAt`, ExecuteUpdate note) | `Store.Data/AppDbContext.cs` |
| Repositories (entity-specific, `AppDbContext` injected, no generic repo/UoW) | `Store.Data/Repositories/ProductRepository.cs`, `CategoryRepository.cs` |
| `AsNoTracking()` on read-only queries | same repository files |
| `Select()` projections → only needed columns (no `Include`) | repository methods + read models in `Store.Data/Models/*.cs` |
| N+1 prevention (category name via JOIN in the same query) | `ProductRepository.GetListItemWithCategoryPageAsync`, `GetDetailsWithCategoryAsync` |
| Soft delete = `IsDeleted = true` (row kept, `ModifiedAt` stamped) | `ProductService.SoftDeleteAsync`, `CategoryService.SoftDeleteAsync` |
| Optimistic concurrency → 409 | `*Repository.UpdateAsync` (sets original row version), `catch (DbUpdateConcurrencyException)` in controllers |
| Pagination (`Skip`/`Take`, metadata, max page size) | `Store.Api/Common/Pagination.cs`, `Store.Business/Models/PagedResult.cs`, repository queries |
| Business logic (both API versions share it) | `Store.Business/Services/*.cs` |
| DTOs + Data Annotations validation → automatic 400 | `Store.Api/Dtos/**`, `[ApiController]` |
| v1 vs v2 contracts (`categoryName` added in v2) | `Store.Api/Dtos/Products/V1/*`, `V2/*`, `Store.Api/Controllers/V1/ProductsController.cs`, `V2/ProductsController.cs` |
| URL-segment versioning + version header | `Store.Api/Program.cs` (`AddApiVersioning` with `UrlSegmentApiVersionReader`) |
| Versioned Swagger | `Store.Api/Swagger/ConfigureSwaggerOptions.cs`, `Program.cs` |
| Seed data (6 categories, 25 products → 3 pages) | `Store.Data/Seed/SeedData.cs` (`HasData`) |
| Error handling: 400 / 404 / 409 only | DTO validation, `NotFound()` in controllers, `DbUpdateConcurrencyException` → `Conflict()` |
| Serilog (file-only sink, config in `appsettings.json`, Information level) | `Store.Api/appsettings.json` (`Serilog` section), `Store.Api/Program.cs` (`ReadFrom.Configuration` + `UseSerilog`) |

### Design note: why read models live in `Store.Data`

The dependency direction (Api → Business → Data) means `Store.Data` and `Store.Business`
cannot see API DTOs. Repositories therefore project (`Select`) into small read models in
`Store.Data/Models`, services return those, and controllers map them to the API DTOs in
`Store.Api/Dtos` (static `From` methods). Entities are never exposed from controllers, and
the API contract stays free to change per version without touching the data layer.

## Logging (Serilog)

Logging is handled by Serilog, configured **entirely from `Store.Api/appsettings.json`** (the
`Serilog` section) and loaded with `ReadFrom.Configuration(builder.Configuration)` in
`Program.cs`. There is **no console sink** - everything goes to a rolling text file.

- Sink: `File` only → `Store.Api/Logs/store-api-{date}.log`
- Rolling: daily, max 10 MB per file, 31 files retained
- Minimum level: `Information` (framework noise from `Microsoft.*` and `System.*` is capped at `Warning`)
- `UseSerilogRequestLogging()` writes one line per HTTP request (method, path, status, elapsed ms)
- `Log.CloseAndFlush()` in `finally` makes sure buffered writes reach the file on shutdown

```jsonc
"Serilog": {
  "Using": [ "Serilog.Sinks.File" ],
  "MinimumLevel": { "Default": "Information", "Override": { "Microsoft": "Warning" } },
  "WriteTo": [ { "Name": "File", "Args": { "path": "Logs/store-api-.log", "rollingInterval": "Day" } } ]
}
```

## Verified behaviour (actually run against SQL Server)

- Build: 0 errors, 0 warnings (.NET 10, EF Core 10.0.13)
- Migration applied on startup; seed produced 6 categories / 25 products (3 pages)
- CRUD for products (v1 + v2) and categories (v1): 201/200/204 + Location headers
- Pagination metadata, ordering, and 400s for `pageNumber=0`, `pageSize=0/101/-3`
- Validation 400s: empty/oversized names, negative price, `categoryId=0`
- 404s: missing ids, unknown `categoryId` on create, endpoints after soft delete
- Soft delete: row kept with `IsDeleted=1`, `ModifiedAt` set, `CreatedAt` unchanged; re-delete → 404
- Concurrency: stale `rowVersion` on PUT → 409, and the stale write did **not** overwrite
- v1 responses have no `categoryName`; v2 responses include it (list + details + create)
- `api-supported-versions: 1.0, 2.0` header; Swagger JSON for `/swagger/v1` and `/swagger/v2`
- Serilog writes Information-level entries to `Store.Api/Logs/store-api-{date}.log` (startup + one line per request) and nothing to the console

## Scope

Deliberately **not** included: generic repository, unit of work, CQRS/MediatR, AutoMapper,
FluentValidation, Clean/Onion Architecture, auth, caching, messaging, extra entities.
