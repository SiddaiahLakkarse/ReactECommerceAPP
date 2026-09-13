# eCommerce Clean Architecture

An end-to-end eCommerce MVP built with .NET 10, ASP.NET Core Web API, Clean Architecture, command handlers, EF Core, SQL Server, JWT authentication, and React TypeScript.

## Current capabilities

- Product catalog backed by `GET /api/products`
- Domain validation for prices, stock, carts, and orders
- Command handlers for product creation, cart updates, and checkout
- ASP.NET Core Identity user registration and login
- JWT bearer authentication
- Authenticated cart and order-history endpoints
- React routes for catalog, product details, cart, checkout, orders, login, registration, account, admin, and not-found pages
- Development seed products and EF Core in-memory database fallback
- SQL Server configuration for non-development environments
- Mermaid database, architecture, and sequence diagrams
- xUnit domain tests

## Repository layout

- `src/Backend/ECommerce.Domain` - entities and business rules
- `src/Backend/ECommerce.Application` - commands, handlers, DTOs, and ports
- `src/Backend/ECommerce.Infrastructure` - EF Core, repositories, Identity stores, and providers
- `src/Backend/ECommerce.Api` - controllers, JWT setup, CORS, and dependency injection
- `src/Frontend` - Vite React TypeScript storefront
- `tests` - automated tests
- `docs` - API, database, architecture, and UML documentation
- `ECommerce.slnx` - Visual Studio solution for the .NET projects

## Prerequisites

- .NET SDK 10
- Node.js 22+ and npm
- Docker Desktop for SQL Server mode
- Visual Studio 2026 or the .NET CLI

## Run locally

Development mode does not require Docker because the API uses an EF Core in-memory database and seeds three products on startup.

```powershell
# Terminal 1
 dotnet run --project src/Backend/ECommerce.Api --launch-profile http

# Terminal 2
 npm install --prefix src/Frontend
 npm run dev --prefix src/Frontend
```

Open `http://localhost:5173`. The API runs at `http://localhost:5016` and the frontend defaults to `http://localhost:5016/api`.

For SQL Server mode:

```powershell
docker compose up -d sqlserver
dotnet ef migrations add InitialCreate --project src/Backend/ECommerce.Infrastructure --startup-project src/Backend/ECommerce.Api
dotnet ef database update --project src/Backend/ECommerce.Infrastructure --startup-project src/Backend/ECommerce.Api
```

Use a non-Development environment so the API selects the SQL Server provider. Keep the JWT key and database password in user secrets or environment variables.

## React routes

- `/` - product catalog
- `/products/:id` - product details
- `/cart` - shopping cart
- `/checkout` - checkout form
- `/orders` - order history
- `/login` and `/register` - JWT authentication screens
- `/account` - authenticated account state
- `/admin` and `/admin/products` - administration UI foundation

## API summary

### Public

- `GET /api/products` - list active products
- `POST /api/products` - create a product; authorization should be restricted before production
- `POST /api/auth/register` - register `{ email, password, displayName }`
- `POST /api/auth/login` - login `{ email, password }`

### Authenticated

Send `Authorization: Bearer <token>`.

- `GET /api/customer/cart` - get the current user's cart
- `POST /api/customer/cart/items` - add `{ productId, quantity }`
- `GET /api/customer/orders` - list the current user's orders
- `POST /api/orders` - create an order from the authenticated user's cart

Swagger/OpenAPI is available in Development.

## Architecture

Dependencies point inward: Domain contains business rules; Application defines use cases and ports; Infrastructure implements persistence and Identity; API composes the system. React communicates through HTTP and stores the JWT locally for the current demo flow.

## Production checklist

- Move JWT keys and connection strings to secure configuration.
- Add refresh tokens, email verification, lockout, password reset, and rate limiting.
- Add Admin role provisioning and `[Authorize(Roles = "Admin")]` product CRUD.
- Connect the checkout screen to server order creation and a real payment provider.
- Add ProblemDetails mapping for validation, stock conflicts, and empty carts.
- Add EF migrations, health checks, structured logging, observability, HTTPS, pagination, and integration tests.

See `docs/api.md`, `docs/database.md`, `docs/architecture.md`, and `docs/uml.md` for detailed documentation.
