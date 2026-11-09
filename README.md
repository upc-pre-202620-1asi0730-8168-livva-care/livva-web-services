# Livva Platform (livva-web-services)

## Overview

Livva Platform is the ASP.NET Core RESTful API of **Livva Care**, described in the [project report](https://github.com/upc-pre-202620-1asi0730-8168-livva-care/livva-project-report). It is the backend consumed by the [livva-web-application](https://github.com/upc-pre-202620-1asi0730-8168-livva-care/livva-web-application) frontend.

The project follows a Domain-Driven Design structure separated into bounded contexts, with command/query services and Entity Framework Core persistence.

## Domain-Driven Design approach

- **Bounded contexts**
  - `Shared`: cross-cutting abstractions used by every context, such as repository contracts, unit of work, auditing, and reusable application patterns.
  - Business contexts (to be added): each one with its own `Domain`, `Application`, `Infrastructure` and `Interfaces` layers.
- **Domain layer**: aggregates, entities, value objects, commands and queries.
- **Application layer**: command/query services returning a shared `Result<TValue, TError>` pattern.
- **Interfaces layer**: REST controllers, resources and assemblers.
- **Infrastructure layer**: EF Core repositories on MySQL, `snake_case` naming and audit timestamps through a save-changes interceptor.

## Technologies

- C# 14 and .NET 10 (`net10.0`)
- ASP.NET Core Web API (REST controllers + built-in OpenAPI generation)
- Scalar.AspNetCore and Swagger UI for interactive API documentation
- Entity Framework Core 10 + MySQL provider (`MySql.EntityFrameworkCore`)
- Humanizer for naming conventions
- JWT Bearer authentication and BCrypt password hashing (packages ready for the IAM context)
- Localization with `.resx` resources (`en`, `en-US`, `es`, `es-PE`, `es-419`)
- xUnit, Moq and `Microsoft.AspNetCore.Mvc.Testing` for tests
- PlantUML (architecture diagrams in `docs/`)

## Project structure

```
livva-web-services/
├── LivvaPlatform.API/                  # Web API project
│   ├── IdentityAndProfileManagement/   # Users, profiles and authentication
│   ├── InsuranceOfferingAndApplications/ # Insurers, products, vehicles, applications, beneficiaries
│   ├── Resources/                      # Localization (.resx)
│   └── Shared/                         # Shared bounded context
├── LivvaPlatform.API.Tests/            # Unit tests
├── LivvaPlatform.API.IntegrationTests/ # Integration tests
└── docs/                               # User stories, ADRs, class diagram
```

## Documentation

- Technical stories: [`docs/user-stories.md`](docs/user-stories.md)
- Architecture decisions: [`docs/adrs.md`](docs/adrs.md)
- Class diagram: [`docs/class-diagram.puml`](docs/class-diagram.puml)

## Getting started

### Prerequisites

- .NET SDK 10
- A reachable MySQL instance

### 1) Restore dependencies

```bash
dotnet restore livva-web-services.sln
```

### 2) Configure your connection string

The committed `appsettings.Development.json` uses a placeholder password (`TU_PASSWORD`). Each teammate sets their own local connection string with **user secrets**, which override `appsettings` in Development and are never committed:

```bash
cd LivvaPlatform.API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;user=root;password=<your-password>;database=livva-platform"
dotnet user-secrets set "TokenSettings:Secret" "<a-random-secret-with-at-least-32-characters>"
```

The JWT signing secret is required; the API does not start without it. The database `livva-platform` and its tables are created automatically on startup through EF Core migrations.

### 3) Run the API

```bash
dotnet run --project LivvaPlatform.API
```

### 4) Explore the API

- Scalar: `http://localhost:5033/scalar`
- Swagger UI: `http://localhost:5033/swagger`
- OpenAPI document: `http://localhost:5033/openapi/v1.json`

### Database initialization

Pending migrations are applied on startup via `Database.Migrate()`. Add and commit a migration every time the data model changes (the `dotnet-ef` tool is restored with `dotnet tool restore`):

```bash
dotnet tool restore
dotnet ef migrations add <Name> --project LivvaPlatform.API --output-dir Shared/Infrastructure/Persistence/EFC/Migrations
```

### Authentication

1. Register an account with `POST /api/v1/authentication/sign-up`.
2. Sign in with `POST /api/v1/authentication/sign-in` to obtain a JWT.
3. Send the token in the `Authorization: Bearer <token>` header. In Scalar or Swagger UI, use the Authorize/Authentication button.

### Deployment on Railway

The `Dockerfile` listens on the port provided by Railway in the `PORT` environment variable. Configure these variables in the API service:

| Variable | Value |
|---|---|
| `MYSQLHOST`, `MYSQLPORT`, `MYSQLUSER`, `MYSQLPASSWORD`, `MYSQLDATABASE` | References to the Railway MySQL service variables |
| `JWT_SECRET` | A random secret with at least 32 characters |
| `Cors__AllowedOrigins` | Comma-separated allowed origins (for example, `https://livva-web-application.web.app`). Empty allows any origin. |

### Production-style configuration

`LivvaPlatform.API/appsettings.Production.json` supports environment variable expansion:

- `%MYSQLHOST%` and `%MYSQLPORT%`
- `%MYSQLUSER%` and `%MYSQLPASSWORD%`
- `%MYSQLDATABASE%`
- `%JWT_SECRET%` (in `appsettings.json`, section `TokenSettings`)

Set `ASPNETCORE_ENVIRONMENT=Production` before running with production settings.

## Build and test

```bash
dotnet build livva-web-services.sln
dotnet test livva-web-services.sln
```

## Team

Developed by the Livva Care team, course *Aplicaciones Web* (1ASI0730), UPC 2026-20.