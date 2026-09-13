# eCommerce

A Clean Architecture eCommerce MVP using ASP.NET Core, command handlers, EF Core/SQL Server, and React TypeScript.

## Features

- Product catalog and product creation API
- Cart item and order command workflows
- Domain-first entities with stock validation
- EF Core SQL Server persistence and query indexes
- React TypeScript storefront
- Docker Compose SQL Server development dependency
- Mermaid database and UML diagrams
- xUnit domain tests

## Repository layout

- `src/Backend/ECommerce.Domain` - entities and business rules
- `src/Backend/ECommerce.Application` - commands, handlers, DTOs, and ports
- `src/Backend/ECommerce.Infrastructure` - EF Core context and repositories
- `src/Backend/ECommerce.Api` - HTTP controllers and dependency injection
- `src/Frontend` - Vite React TypeScript application
- `tests` - automated tests
- `docs` - database, architecture, API, UML, and operations documentation

## Prerequisites

.NET SDK 10, Node.js 22+, npm, Docker Desktop, and SQL Server (Docker is recommended).

## Run locally

1. Start SQL Server: `docker compose up -d sqlserver`
2. Restore/build backend: `dotnet restore ECommerce.slnx && dotnet build ECommerce.slnx`
3. Apply EF migrations after installing the tool: `dotnet tool install --global dotnet-ef`; then `dotnet ef migrations add InitialCreate --project src/Backend/ECommerce.Infrastructure --startup-project src/Backend/ECommerce.Api` and `dotnet ef database update --project src/Backend/ECommerce.Infrastructure --startup-project src/Backend/ECommerce.Api`.
4. Run API: `dotnet run --project src/Backend/ECommerce.Api`.
5. Run UI: `npm install --prefix src/Frontend && npm run dev --prefix src/Frontend`.
6. Set `VITE_API_URL` if the API uses a different HTTPS port.

## API summary

- `GET /api/products` lists active products.
- `POST /api/products` creates a product with `{ name, description, price, stockQuantity }`.
- `POST /api/cart/items` runs `AddToCartCommand` with `{ userId, productId, quantity }`.
- `POST /api/orders` runs `CreateOrderCommand` with `{ userId }`.
- Swagger is available in development.

## Architecture

HTTP controllers depend on Application handlers. Application depends on Domain and abstractions only. Infrastructure implements those abstractions with EF Core. This keeps business logic testable and prevents the domain from depending on SQL Server or ASP.NET Core.

## Production checklist

Add ASP.NET Core Identity/JWT, authorization policies, payment provider integration, outbox/event publishing, rate limiting, secret storage, structured logging, health checks, migrations in deployment, HTTPS certificates, and observability before production use. The current MVP deliberately uses a caller-provided `userId` to keep the sample workflow runnable without an identity provider.

See `docs/database.md`, `docs/architecture.md`, `docs/uml.md`, and `docs/api.md` for detailed design documentation.
