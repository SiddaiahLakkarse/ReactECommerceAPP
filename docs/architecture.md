# Architecture

```mermaid
graph LR
  UI[React TypeScript] --> API[ASP.NET Core API]
  API --> APP[Application command handlers]
  APP --> DOMAIN[Domain entities/rules]
  APP --> PORTS[Repository and unit-of-work ports]
  INFRA[Infrastructure EF Core] -.implements.-> PORTS
  INFRA --> DB[(SQL Server)]
```

The dependency direction is inward: Domain has no infrastructure dependency; Application references Domain and defines ports; Infrastructure references both; API composes everything. Commands make state changes explicit and are independently testable.
