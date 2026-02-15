# System Architecture Documentation

## Table of Contents

- [Overview](#overview)
- [Architecture Pattern](#architecture-pattern)
- [Layer Architecture](#layer-architecture)
- [Request Flow](#request-flow)
- [CQRS Pattern](#cqrs-pattern)
- [Database Architecture](#database-architecture)
- [API Design](#api-design)
- [Dependency Injection](#dependency-injection)
- [Future Enhancements](#future-enhancements)

## Overview

This project implements a Clean/Onion Architecture pattern for a .NET 10 Web API. The architecture promotes separation of concerns, testability, and maintainability by organizing code into concentric layers with strict dependency rules.

**Key Principles:**

- Dependencies point inward (outer layers depend on inner layers, never the reverse)
- Domain layer has no external dependencies
- Infrastructure concerns are isolated from business logic
- Business logic is framework-agnostic

## Architecture Pattern

### Onion Architecture Layers

```mermaid
graph TB
    subgraph "Presentation Layer"
        WebApi[WebApi<br/>Controllers, Middleware]
    end

    subgraph "Infrastructure Layer"
        Persistence[Persistence<br/>EF Core, DbContext, Migrations]
    end

    subgraph "Core - Application Layer"
        Application[Application<br/>Commands, Queries, Interfaces]
    end

    subgraph "Core - Domain Layer"
        Domain[Domain<br/>Entities, Value Objects]
    end

    WebApi --> Application
    WebApi --> Persistence
    Persistence --> Application
    Application --> Domain
    Persistence --> Domain

    style Domain fill:#4CAF50
    style Application fill:#2196F3
    style Persistence fill:#FF9800
    style WebApi fill:#9C27B0
```

### Dependency Flow

```mermaid
flowchart LR
    Domain["Domain<br/>(No Dependencies)"]
    Application["Application<br/>(Depends on Domain)"]
    Persistence["Persistence<br/>(Depends on Application & Domain)"]
    WebApi["WebApi<br/>(Depends on Application & Persistence)"]

    WebApi --> Persistence
    WebApi --> Application
    Persistence --> Application
    Persistence --> Domain
    Application --> Domain

    style Domain fill:#4CAF50,color:#fff
    style Application fill:#2196F3,color:#fff
    style Persistence fill:#FF9800,color:#fff
    style WebApi fill:#9C27B0,color:#fff
```

## Layer Architecture

### 1. Domain Layer (Core)

**Location:** `src/Core/Domain/`

**Purpose:** Contains enterprise business rules and entities. This is the heart of the application with zero external dependencies.

**Components:**

- `Common/BaseEntity.cs` - Base class for all entities with common properties (Id)
- `Entities/Product.cs` - Domain entities representing business objects

**Rules:**

- No dependencies on other layers
- No framework dependencies
- Pure C# classes
- Contains only business logic and domain rules

```mermaid
classDiagram
    class BaseEntity {
        <<abstract>>
        +int Id
    }

    class Product {
        +string Name
        +string Barcode
        +string Description
        +decimal Rate
    }

    BaseEntity <|-- Product
```

### 2. Application Layer (Core)

**Location:** `src/Core/Application/`

**Purpose:** Contains application business logic and orchestrates the flow of data. Implements CQRS pattern using MediatR.

**Components:**

- `Interfaces/` - Abstractions for infrastructure concerns (e.g., `IApplicationDbContext`)
- `Features/` - CQRS commands and queries organized by feature
- `DependencyInjection.cs` - Service registration for MediatR

**Dependencies:**

- Domain layer
- MediatR
- EntityFrameworkCore (abstractions only)

**Rules:**

- Depends only on Domain layer
- Defines interfaces for infrastructure
- No concrete infrastructure implementations
- Framework-agnostic business logic

### 3. Infrastructure Layer

**Location:** `src/Infrastructure/Persistence/`

**Purpose:** Implements data access and external service integrations. Contains concrete implementations of Application layer interfaces.

**Components:**

- `Context/ApplicationDbContext.cs` - EF Core DbContext implementation
- `Migrations/` - EF Core database migrations
- `DependencyInjection.cs` - Service registration for persistence

**Dependencies:**

- Application layer (implements its interfaces)
- Domain layer (for entity access)
- EntityFrameworkCore
- Database providers (SQLite currently)

**Rules:**

- Implements interfaces defined in Application layer
- Contains all database-specific code
- Manages migrations and database schema

### 4. Presentation Layer

**Location:** `src/Presentation/WebApi/`

**Purpose:** Exposes the application via RESTful API. Handles HTTP concerns, routing, and API versioning.

**Components:**

- `Controllers/` - API controllers
- `Controllers/BaseApiController.cs` - Base controller with MediatR integration
- `Program.cs` - Application entry point and DI configuration

**Dependencies:**

- Application layer (for commands/queries)
- Persistence layer (for DI registration)
- ASP.NET Core
- Swagger/OpenAPI
- API Versioning

**Rules:**

- Thin controllers that delegate to MediatR
- No business logic in controllers
- Handle only HTTP concerns (routing, status codes, etc.)

## Request Flow

### HTTP Request Lifecycle

```mermaid
sequenceDiagram
    participant Client
    participant Controller
    participant MediatR
    participant Handler
    participant DbContext
    participant Database

    Client->>Controller: HTTP POST /api/v1/product
    activate Controller

    Controller->>MediatR: Send(CreateProductCommand)
    activate MediatR

    MediatR->>Handler: Handle(command)
    activate Handler

    Handler->>DbContext: Products.Add(product)
    activate DbContext

    DbContext->>Database: INSERT INTO Products
    activate Database
    Database-->>DbContext: Success
    deactivate Database

    DbContext-->>Handler: Product Id
    deactivate DbContext

    Handler-->>MediatR: Return Id
    deactivate Handler

    MediatR-->>Controller: Return Id
    deactivate MediatR

    Controller-->>Client: HTTP 200 OK (id)
    deactivate Controller
```

### Layer Interaction Example

```mermaid
flowchart TD
    A[Client Request] --> B[ProductController]
    B --> C{MediatR}
    C --> D[CreateProductCommandHandler]
    D --> E[IApplicationDbContext]
    E --> F[ApplicationDbContext]
    F --> G[SQLite Database]

    G --> H[Return Product Id]
    H --> D
    D --> C
    C --> B
    B --> I[HTTP Response]

    style B fill:#9C27B0,color:#fff
    style D fill:#2196F3,color:#fff
    style E fill:#2196F3,color:#fff
    style F fill:#FF9800,color:#fff
    style G fill:#FF5722,color:#fff
```

## CQRS Pattern

This application implements Command Query Responsibility Segregation (CQRS) using MediatR.

### Command Flow (Write Operations)

```mermaid
flowchart LR
    A[HTTP POST/PUT/DELETE] --> B[Controller]
    B --> C[MediatR]
    C --> D[Command Handler]
    D --> E[DbContext]
    E --> F[Database Write]
    F --> G[Return Result]

    style A fill:#f44336,color:#fff
    style D fill:#2196F3,color:#fff
    style E fill:#FF9800,color:#fff
```

**Commands:**

- `CreateProductCommand` - Creates a new product
- `UpdateProductCommand` - Updates an existing product
- `DeleteProductByIdCommand` - Deletes a product

### Query Flow (Read Operations)

```mermaid
flowchart LR
    A[HTTP GET] --> B[Controller]
    B --> C[MediatR]
    C --> D[Query Handler]
    D --> E[DbContext]
    E --> F[Database Read]
    F --> G[Return Data]

    style A fill:#4CAF50,color:#fff
    style D fill:#2196F3,color:#fff
    style E fill:#FF9800,color:#fff
```

**Queries:**

- `GetAllProductsQuery` - Retrieves all products
- `GetProductByIdQuery` - Retrieves a single product by ID

### CQRS Benefits

1. **Separation of Concerns** - Read and write logic are separate
2. **Scalability** - Can optimize reads and writes independently
3. **Maintainability** - Each handler has a single responsibility
4. **Testability** - Easy to unit test individual handlers

## Database Architecture

### Entity Relationship Diagram

```mermaid
erDiagram
    Product {
        int Id PK
        string Name
        string Barcode
        string Description
        decimal Rate
    }
```

_Note: Currently, the application has a single entity. As the application grows, relationships between entities will be added here._

### Database Configuration

**Provider:** SQLite (development)
**Connection String:** `Data Source=onion.db`
**Migrations Assembly:** `Persistence`

**Configuration Location:**

- Connection string: `src/Presentation/WebApi/appsettings.json`
- DbContext setup: `src/Infrastructure/Persistence/DependencyInjection.cs`

```mermaid
flowchart TD
    A[appsettings.json] -->|Connection String| B[DependencyInjection]
    B -->|Configure| C[ApplicationDbContext]
    C -->|Implements| D[IApplicationDbContext]
    D -->|Used by| E[Command/Query Handlers]

    style A fill:#FFC107,color:#000
    style C fill:#FF9800,color:#fff
    style D fill:#2196F3,color:#fff
    style E fill:#2196F3,color:#fff
```

### Migration Strategy

```bash
# Create migration
cd src/Presentation/WebApi
dotnet ef migrations add MigrationName --project ../../Infrastructure/Persistence

# Apply migration
dotnet ef database update --project ../../Infrastructure/Persistence
```

## API Design

### Endpoint Structure

```mermaid
graph TD
    A[api/v1/product] --> B["GET - GetAll"]
    A --> C["GET /:id - GetById"]
    A --> D["POST - Create"]
    A --> E["PUT /:id - Update"]
    A --> F["DELETE /:id - Delete"]

    style A fill:#9C27B0,color:#fff
    style B fill:#4CAF50,color:#fff
    style C fill:#4CAF50,color:#fff
    style D fill:#2196F3,color:#fff
    style E fill:#FF9800,color:#fff
    style F fill:#f44336,color:#fff
```

### API Versioning

The API uses URL-based versioning via `Asp.Versioning.Mvc`.

**Current Version:** v1.0
**Route Template:** `api/v{version:apiVersion}/[controller]`

**Example Endpoints:**

- `GET /api/v1/product` - Get all products
- `GET /api/v1/product/1` - Get product by ID
- `POST /api/v1/product` - Create product
- `PUT /api/v1/product/1` - Update product
- `DELETE /api/v1/product/1` - Delete product

### Controller Pattern

All controllers inherit from `BaseApiController`:

```mermaid
classDiagram
    class BaseApiController {
        <<abstract>>
        #IMediator Mediator
    }

    class ProductController {
        +Create(command) IActionResult
        +GetAll() IActionResult
        +GetById(id) IActionResult
        +Update(id, command) IActionResult
        +Delete(id) IActionResult
    }

    BaseApiController <|-- ProductController

    note for BaseApiController "[ApiController]\n[Route api/v:version/controller]"
    note for ProductController "[ApiVersion 1.0]"
```

**Benefits:**

- Lazy-loaded MediatR instance
- Consistent routing across all controllers
- Reduced boilerplate code

## Dependency Injection

### Service Registration Flow

```mermaid
flowchart TD
    A[Program.cs] --> B[AddApplication]
    A --> C[AddPersistence]

    B --> D[Register MediatR]
    B --> E[Scan Application Assembly]

    C --> F[Register DbContext]
    C --> G[Register IApplicationDbContext]
    C --> H[Configure SQLite]

    style A fill:#9C27B0,color:#fff
    style B fill:#2196F3,color:#fff
    style C fill:#FF9800,color:#fff
```

### Registration Order

1. **Application Layer** (`AddApplication`)
   - MediatR with all handlers from Application assembly

2. **Persistence Layer** (`AddPersistence`)
   - `ApplicationDbContext` as scoped service
   - `IApplicationDbContext` → `ApplicationDbContext` mapping
   - SQLite provider configuration

3. **Presentation Layer** (in `Program.cs`)
   - Controllers
   - API Versioning
   - Swagger/OpenAPI
   - Middleware pipeline

## Future Enhancements

### Recommended Architectural Improvements

```mermaid
graph TB
    subgraph "Planned Infrastructure"
        Identity[Identity<br/>JWT Authentication]
        Caching[Caching<br/>Redis/In-Memory]
        Logging[Logging<br/>Serilog]
    end

    subgraph "Planned Application"
        Validation[FluentValidation<br/>Pipeline Behaviors]
        ResponseWrapper[Response Wrapper<br/>Standardized API Responses]
        ErrorHandling[Global Error Handling]
    end

    subgraph "Current Core"
        App[Application]
        Dom[Domain]
    end

    Identity --> App
    Validation --> App
    ResponseWrapper --> App
    Caching --> App
    Logging --> App
    ErrorHandling --> App

    style Identity fill:#FF9800,color:#fff
    style Caching fill:#FF9800,color:#fff
    style Logging fill:#FF9800,color:#fff
    style Validation fill:#2196F3,color:#fff
    style ResponseWrapper fill:#2196F3,color:#fff
```

### 1. JWT Authentication

- Add `src/Infrastructure/Identity` project
- Implement user management and token generation
- Integrate with existing API

### 2. Response Wrapper

- Standardize all API responses
- Include success/error status
- Consistent error messaging

```json
{
  "succeeded": true,
  "data": { "id": 1 },
  "errors": [],
  "message": "Product created successfully"
}
```

### 3. FluentValidation

- Add validation pipeline behavior to MediatR
- Validate commands before reaching handlers
- Return consistent validation errors

### 4. Structured Logging

- Replace default logging with Serilog
- Add structured logging to all layers
- Implement correlation IDs for request tracking

### 5. Database Migration

- Switch from SQLite to PostgreSQL for production
- Update `UseSqlite` → `UseNpgsql`
- Add `Npgsql.EntityFrameworkCore.PostgreSQL` package

### 6. Testing Strategy

```mermaid
graph TD
    A[Testing Pyramid] --> B[Unit Tests]
    A --> C[Integration Tests]
    A --> D[E2E Tests]

    B --> B1[Domain Tests]
    B --> B2[Handler Tests]

    C --> C1[API Tests]
    C --> C2[Database Tests]

    D --> D1[Full Request Flow]

    style A fill:#4CAF50,color:#fff
    style B fill:#2196F3,color:#fff
    style C fill:#FF9800,color:#fff
    style D fill:#9C27B0,color:#fff
```

**Recommended Test Projects:**

- `tests/Domain.UnitTests` - Test domain entities and logic
- `tests/Application.UnitTests` - Test handlers with mocked dependencies
- `tests/Application.IntegrationTests` - Test with real database (in-memory)
- `tests/WebApi.IntegrationTests` - Test API endpoints end-to-end

---

## Summary

This Onion Architecture implementation provides:

- ✅ **Clear Separation of Concerns** - Each layer has a well-defined responsibility
- ✅ **Testability** - Dependencies point inward, making mocking easy
- ✅ **Maintainability** - Changes to infrastructure don't affect business logic
- ✅ **Scalability** - CQRS pattern allows independent optimization
- ✅ **Framework Independence** - Core business logic is not tied to ASP.NET Core

The architecture is production-ready and can be extended with the recommended enhancements as the application grows.
