# Architecture

## Dependency flow

```mermaid
graph LR
  UI[React TypeScript] --> API[ASP.NET Core API]
  API --> AUTH[Identity and JWT]
  API --> APP[Application command handlers]
  APP --> DOMAIN[Domain entities and rules]
  APP --> PORTS[Repository and unit-of-work ports]
  INFRA[Infrastructure EF Core] -.implements.-> PORTS
  INFRA --> DB[(SQL Server or Development InMemory)]
```

The dependency direction is inward:

- **Domain** contains products, carts, orders, stock rules, and status values.
- **Application** defines commands, handlers, DTOs, repositories, and unit-of-work ports.
- **Infrastructure** implements EF Core repositories, Identity stores, SQL Server, and the Development in-memory provider.
- **API** composes services, validates JWTs, exposes controllers, and applies CORS.
- **Frontend** provides route-aware pages and calls public or authenticated API endpoints.

## Command handler flow

```mermaid
sequenceDiagram
  participant UI as React UI
  participant API as Controller
  participant Handler as Application handler
  participant Repo as Repository port
  participant DB as EF Core database
  UI->>API: Send command request
  API->>Handler: Create command
  Handler->>Repo: Load and validate aggregate
  Repo->>DB: Execute query/update
  DB-->>Repo: Return result
  Repo-->>Handler: Domain result
  Handler-->>API: DTO
  API-->>UI: JSON response
```

## Authentication boundary

Registration and login are handled by ASP.NET Core Identity. Successful authentication returns a JWT containing the user identifier. The API uses `[Authorize]` for customer cart, order, and checkout operations. The current admin screen is a UI foundation; production admin operations should use role claims and `[Authorize(Roles = "Admin")]`.

## Development versus production

Development uses `UseInMemoryDatabase("ECommerceDevelopment")` and seeds sample products so the app can run without Docker. Non-development environments use the configured SQL Server connection string and should apply EF migrations during deployment.
