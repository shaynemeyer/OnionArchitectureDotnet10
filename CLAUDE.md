# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET 10 Web API demonstrating Clean/Onion Architecture with a simple Product CRUD application.

## Architecture

The solution follows Onion Architecture with strict dependency rules:

**Core Layer (innermost)**

- `Domain/` - Entities and base classes (e.g., `BaseEntity`, `Product`)
- `Application/` - Business logic, MediatR handlers, and interfaces (e.g., `IApplicationDbContext`)
  - Uses CQRS pattern via MediatR
  - Commands: `CreateProductCommand`, `UpdateProductCommand`, `DeleteProductByIdCommand`
  - Queries: `GetAllProductsQuery`, `GetProductByIdQuery`

**Infrastructure Layer**

- `Persistence/` - EF Core implementation, database context, migrations
  - Implements `IApplicationDbContext` from Application layer
  - Uses SQLite with connection string in `appsettings.json`
  - Migrations assembly is set to `Persistence` assembly

**Presentation Layer (outermost)**

- `WebApi/` - ASP.NET Core controllers, API configuration
  - Uses API versioning (currently v1)
  - Controllers inherit from `BaseApiController` which provides lazy-loaded `IMediator`
  - Route pattern: `api/v{version:apiVersion}/[controller]`

**Dependency Flow**: Presentation → Infrastructure → Application → Domain

- Domain has no dependencies
- Application depends only on Domain
- Infrastructure depends on Application and Domain
- Presentation depends on Application and Persistence

## Common Commands

### Run the application

```bash
cd src/Presentation/WebApi
dotnet run
```

The API runs on:

- HTTPS: https://localhost:7130
- HTTP: http://localhost:5296
- Swagger UI available at: /swagger

### Build the solution

```bash
dotnet build
```

### Database Migrations

```bash
# Add a new migration
cd src/Presentation/WebApi
dotnet ef migrations add <MigrationName> --project ../../Infrastructure/Persistence

# Update database
dotnet ef database update --project ../../Infrastructure/Persistence
```

Note: EF Core tools commands must be run from the WebApi project since it references `Microsoft.EntityFrameworkCore.Design`.

## Key Patterns

### Adding New Features

1. Create entity in `Domain/Entities/`
2. Add DbSet to `IApplicationDbContext` interface
3. Implement in `ApplicationDbContext`
4. Create commands/queries in `Application/Features/<FeatureName>/`
5. Create controller in `WebApi/Controllers/v1/`
6. Run migrations

### MediatR Request/Handler Pattern

- Commands/Queries are records implementing `IRequest<TResponse>`
- Handlers are classes implementing `IRequestHandler<TRequest, TResponse>`
- Both defined in the same file under `Application/Features/`

### Controller Pattern

- Inherit from `BaseApiController`
- Use `[ApiVersion("1.0")]` attribute
- Access MediatR via `Mediator` property
- Return results wrapped in `Ok()` or other action results

## Configuration

- Connection strings in `appsettings.json`
- Currently using SQLite with database file `onion.db`
- To switch to PostgreSQL: Replace `UseSqlite` with `UseNpgsql` in `Persistence/DependencyInjection.cs` and add `Npgsql.EntityFrameworkCore.PostgreSQL` package
