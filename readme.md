# Onion Architecture in .net 10

A simple example of Clean Architecture in .NET 10.

---

## Run it

```bash
cd src/Presentation/WebApi
dotnet run
```

---

## Potential enhancements

Add **JWT Authentication** as a second Infrastructure project (`src/Infrastructure/Identity`).

- Add a **Response Wrapper** in the Application layer to standardise all API responses.
- Set up **Serilog** for structured logging.
- Introduce **FluentValidation** pipeline behaviours in MediatR for request validation.
- Switch to **PostgreSQL** by replacing `UseSqlite` with `UseNpgsql` and the `Npgsql.EntityFrameworkCore.PostgreSQL` package.
