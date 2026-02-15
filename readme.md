# Onion Architecture in .NET 10

A production-ready example of Clean/Onion Architecture in .NET 10, demonstrating best practices for building maintainable, testable, and scalable web APIs.

## Features

- ✅ **Clean Architecture** - Clear separation of concerns with dependency inversion
- ✅ **CQRS Pattern** - Using MediatR for command/query separation
- ✅ **Domain-Driven Design** - Entity-based domain modeling
- ✅ **RESTful API** - Well-structured HTTP endpoints
- ✅ **API Versioning** - URL-based versioning support
- ✅ **Entity Framework Core** - Code-first database approach
- ✅ **Swagger/OpenAPI** - Interactive API documentation

## Quick Start

```bash
# Clone and navigate to the project
cd OnionArchitecture

# Restore dependencies
dotnet restore

# Run the application
cd src/Presentation/WebApi
dotnet run
```

Access Swagger UI at: `https://localhost:7130/swagger`

## Documentation

Comprehensive documentation is available in the [`docs/`](./docs) folder:

- **[Architecture Guide](./docs/ARCHITECTURE.md)** - System design, layers, patterns, and diagrams
- **[Development Guide](./docs/DEVELOPMENT.md)** - Setup, workflows, and coding patterns
- **[API Reference](./docs/API.md)** - Endpoint documentation and examples
- **[CLAUDE.md](./CLAUDE.md)** - Guidance for Claude Code when working with this repository

## Project Structure

```
src/
├── Core/
│   ├── Domain/              # Business entities (no dependencies)
│   └── Application/         # Business logic, CQRS handlers
├── Infrastructure/
│   └── Persistence/         # EF Core, database context
└── Presentation/
    └── WebApi/              # API controllers, endpoints
```

## Technology Stack

- **.NET 10** - Modern .NET framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM and migrations
- **MediatR** - CQRS pattern implementation
- **SQLite** - Lightweight database (easily swappable)
- **Swagger** - API documentation

## Architecture

This project follows **Onion Architecture** principles:

```
┌─────────────────────────────────┐
│     Presentation (WebApi)       │
├─────────────────────────────────┤
│   Infrastructure (Persistence)  │
├─────────────────────────────────┤
│     Application (CQRS)          │
├─────────────────────────────────┤
│      Domain (Entities)          │
└─────────────────────────────────┘
```

Dependencies point **inward** - the Domain layer has no external dependencies, making the business logic framework-agnostic and highly testable.

See [Architecture Documentation](./docs/ARCHITECTURE.md) for detailed diagrams and explanations.

## Common Commands

```bash
# Build the solution
dotnet build

# Run tests (when added)
dotnet test

# Create a migration
cd src/Presentation/WebApi
dotnet ef migrations add <MigrationName> --project ../../Infrastructure/Persistence

# Apply migrations
dotnet ef database update --project ../../Infrastructure/Persistence
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/v1/product` | Get all products |
| `GET` | `/api/v1/product/{id}` | Get product by ID |
| `POST` | `/api/v1/product` | Create a new product |
| `PUT` | `/api/v1/product/{id}` | Update a product |
| `DELETE` | `/api/v1/product/{id}` | Delete a product |

See [API Documentation](./docs/API.md) for detailed request/response examples.

## Potential Enhancements

The architecture is designed for extensibility. Consider these enhancements:

- **JWT Authentication** - Add `src/Infrastructure/Identity` project
- **Response Wrapper** - Standardize all API responses
- **Serilog** - Structured logging with correlation IDs
- **FluentValidation** - Request validation pipeline behaviors
- **PostgreSQL** - Production-ready database (replace SQLite)
- **Unit Tests** - Test domain logic and handlers
- **Integration Tests** - Test API endpoints end-to-end

See [Architecture - Future Enhancements](./docs/ARCHITECTURE.md#future-enhancements) for detailed implementation guidance.

## Learning Resources

- [Onion Architecture by Jeffrey Palermo](https://jeffreypalermo.com/2008/07/the-onion-architecture-part-1/)
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)

## License

This is an educational example project demonstrating architectural patterns.
