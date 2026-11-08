# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project intends to adhere to [Semantic Versioning](https://semver.org/spec/v2.0.0.html) once versioned releases are tagged.

## [Unreleased]

### Added

- Identity and Profile Management bounded context: `users` and `user_profiles` tables, sign-up and sign-in with BCrypt password hashing and JWT access tokens, and the `/api/v1/users` and `/api/v1/user-profiles` endpoints (GET, POST and PATCH).
- Initial EF Core migration, applied automatically on startup.
- JWT bearer security scheme in the OpenAPI document for Scalar and Swagger UI.
- Railway deployment support: the container listens on `PORT`, the production connection string uses the Railway MySQL variables, and allowed CORS origins are configurable.
- Unit and integration tests for the Identity and Profile Management bounded context.

## [0.1.0] - 2026-11-08

### Added

- ASP.NET Core Web API project `LivvaPlatform.API` on .NET 10 and C# 14.
- `Shared` bounded context: `Result<TValue, TError>` pattern, `IAuditableEntity`, `IBaseRepository<TEntity>`, `IUnitOfWork`, `BaseRepository`, `UnitOfWork` and `AppDbContext`.
- EF Core 10 with the MySQL provider (`MySql.EntityFrameworkCore`), `snake_case` plural naming convention and automatic audit timestamps.
- Lowercase kebab-case route naming convention.
- RFC 7807 `ProblemDetails` error responses with localized messages.
- Localization with `.resx` resources (`en`, `en-US`, `es`, `es-PE`, `es-419`).
- OpenAPI document at `/openapi/v1.json`, Scalar at `/scalar` and Swagger UI at `/swagger`.
- CORS policy allowing any origin (to be restricted to the web application domain).
- Unit test (`LivvaPlatform.API.Tests`) and integration test (`LivvaPlatform.API.IntegrationTests`) projects.
- Dockerfile, `global.json`, README and documentation in `docs/`.