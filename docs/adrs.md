# Architecture Decision Records (ADRs)

## About this document

This document records the significant architectural decisions of the Livva Platform REST API (livva-web-services). Each record follows the lightweight format proposed by Michael Nygard: **Context**, **Decision**, **Consequences**, plus a **Status** and **Date**.

Guidelines for maintaining this log:

- One decision per record; records are short and focused on the *why*.
- Records are immutable once **Accepted**. To change a decision, add a new ADR that **Supersedes** the old one and update the old one's status.
- Statuses: `Proposed`, `Accepted`, `Deprecated`, `Superseded by ADR-XXX`.
- Number records sequentially and never reuse numbers.

## Index

| ADR | Title | Status |
|-----|-------|--------|
| [ADR-001](#adr-001--organize-the-code-by-bounded-context-with-layered-ddd-structure) | Organize the code by bounded context with a layered DDD structure | Accepted |
| [ADR-002](#adr-002--persist-with-ef-core-on-mysql-using-repository-and-unit-of-work) | Persist with EF Core on MySQL using Repository and Unit of Work | Accepted |
| [ADR-003](#adr-003--use-snake_case-database-naming-and-automatic-audit-timestamps) | Use `snake_case` database naming and automatic audit timestamps | Accepted |
| [ADR-004](#adr-004--use-lowercase-kebab-case-routes) | Use lowercase kebab-case routes | Accepted |
| [ADR-005](#adr-005--report-errors-as-rfc-7807-problemdetails) | Report errors as RFC 7807 `ProblemDetails` | Accepted |
| [ADR-006](#adr-006--localize-user-facing-text-with-resx-resources) | Localize user-facing text with `.resx` resources | Accepted |
| [ADR-007](#adr-007--use-built-in-openapi-with-scalar-and-swagger-ui) | Use built-in OpenAPI with Scalar and Swagger UI | Accepted |

---

## ADR-001 — Organize the code by bounded context with a layered DDD structure

- **Status:** Accepted
- **Date:** 2026-11-08

### Context
Livva covers several business areas, defined as bounded contexts in the project report (livva-project-report, chapter IV). We need a structure that keeps domain rules independent from frameworks and prevents unrelated features from becoming entangled.

### Decision
Group code by bounded context (plus a `Shared` context for cross-cutting abstractions). Inside each context use the layers `Domain`, `Application`, `Infrastructure` and `Interfaces`.

### Consequences
Each context can evolve independently and new team members can locate code by feature. It adds some folder depth for small features.

---

## ADR-002 — Persist with EF Core on MySQL using Repository and Unit of Work

- **Status:** Accepted
- **Date:** 2026-11-08

### Context
The course stack uses MySQL. The domain layer must not depend on EF Core.

### Decision
Use Entity Framework Core 10 with the official `MySql.EntityFrameworkCore` provider. Expose persistence through `IBaseRepository<TEntity>` and `IUnitOfWork` interfaces defined in `Shared.Domain.Repositories`.

### Consequences
Application services depend on abstractions and are easy to unit test with mocks. Schema changes are managed through EF Core migrations.

---

## ADR-003 — Use `snake_case` database naming and automatic audit timestamps

- **Status:** Accepted
- **Date:** 2026-11-08

### Context
MySQL conventions favor `snake_case` identifiers, and most aggregates need creation and update timestamps.

### Decision
Apply `UseSnakeCaseNamingConvention()` (plural table names with Humanizer) and set `CreatedAt` / `UpdatedAt` for every `IAuditableEntity` through `AuditableEntityInterceptor`.

### Consequences
Consistent schema naming without per-entity configuration; entities only need to implement `IAuditableEntity`.

---

## ADR-004 — Use lowercase kebab-case routes

- **Status:** Accepted
- **Date:** 2026-11-08

### Context
REST URLs should be predictable for the frontend team.

### Decision
Enable lowercase URLs and register `KebabCaseRouteNamingConvention` so controller names map to kebab-case routes under `/api/v1`.

### Consequences
Routes like `/api/v1/order-items` are generated automatically from controller names.

---

## ADR-005 — Report errors as RFC 7807 `ProblemDetails`

- **Status:** Accepted
- **Date:** 2026-11-08

### Context
Clients need a uniform error format.

### Decision
Register `AddProblemDetails()` and the exception handler, and return localized `ValidationProblemDetails` for invalid model state.

### Consequences
All errors share the `application/problem+json` shape.

---

## ADR-006 — Localize user-facing text with `.resx` resources

- **Status:** Accepted
- **Date:** 2026-11-08

### Context
Livva targets Spanish-speaking users in Peru and Latin America, and the course requires English and Spanish support.

### Decision
Store messages in `Resources/SharedResource.*.resx` and support the cultures `en`, `en-US`, `es`, `es-PE` and `es-419`, with `en` as default.

### Consequences
Responses follow the request `Accept-Language` header. Every new message key must be added to all resource files.

---

## ADR-007 — Use built-in OpenAPI with Scalar and Swagger UI

- **Status:** Accepted
- **Date:** 2026-11-08

### Context
The team needs interactive API documentation for testing and for the frontend integration.

### Decision
Generate the document with `Microsoft.AspNetCore.OpenApi` (`/openapi/v1.json`) and expose it through Scalar (`/scalar`) and Swagger UI (`/swagger`).

### Consequences
No Swashbuckle generator is needed; only its UI package is used.