# Database design

The production database is SQL Server `ECommerceDb`. Development uses an EF Core in-memory database named `ECommerceDevelopment` and seeds sample products at API startup.

## Business tables

- `Products`: catalog data, price, active flag, and stock. Index `(IsActive, Name)` supports catalog reads.
- `Carts`: one active cart per user, enforced by a unique index on `UserId`.
- `CartItems`: cart/product quantities, enforced unique by `(CartId, ProductId)`.
- `Orders`: checkout header, status, total, and UTC creation time. Index `(UserId, CreatedUtc)` supports order history.
- `OrderItems`: product name and price snapshots preserve historical order values.

## Identity tables

ASP.NET Core Identity adds the standard user and security tables, including `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, claims, logins, tokens, and role claims. `AppUser` adds `DisplayName`. User IDs are GUIDs and are used by cart/order ownership.

## Query optimization

Use `AsNoTracking()` for catalog and order-history reads. Keep predicates aligned with `(IsActive, Name)` and `(UserId, CreatedUtc)` indexes. Use UTC timestamps, inspect actual execution plans for large catalogs, paginate public lists, and avoid lazy loading/N+1 queries. The cart read explicitly loads cart items; order history reads only the authenticated user's rows.

## Migration workflow

```powershell
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project src/Backend/ECommerce.Infrastructure --startup-project src/Backend/ECommerce.Api
dotnet ef database update --project src/Backend/ECommerce.Infrastructure --startup-project src/Backend/ECommerce.Api
```

The current Development `EnsureCreated` path is intended for local startup only. Use migrations for SQL Server environments.

## ER diagram

```mermaid
erDiagram
  ASPNET_USERS ||--o| CARTS : owns
  ASPNET_USERS ||--o{ ORDERS : places
  PRODUCTS ||--o{ CART_ITEMS : contains
  CARTS ||--o{ CART_ITEMS : has
  ORDERS ||--o{ ORDER_ITEMS : contains
  PRODUCTS ||--o{ ORDER_ITEMS : snapshots
  ASPNET_USERS { uuid Id PK string Email string DisplayName }
  PRODUCTS { uuid Id PK string Name decimal Price int StockQuantity bool IsActive }
  CARTS { uuid Id PK uuid UserId FK UK }
  CART_ITEMS { uuid Id PK uuid CartId FK uuid ProductId FK int Quantity }
  ORDERS { uuid Id PK uuid UserId FK decimal Total string Status datetime CreatedUtc }
  ORDER_ITEMS { uuid Id PK uuid OrderId FK uuid ProductId FK decimal UnitPrice int Quantity }
