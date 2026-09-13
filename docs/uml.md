# UML diagrams

## Authentication and checkout sequence

```mermaid
sequenceDiagram
  actor Customer
  participant UI as React UI
  participant Auth as AuthController
  participant Identity as ASP.NET Identity
  participant API as Customer/Orders API
  participant Handler as Application handler
  participant DB as EF Core database
  Customer->>UI: Submit login or registration
  UI->>Auth: POST /api/auth/login or /register
  Auth->>Identity: Validate or create user
  Identity-->>Auth: User identity
  Auth-->>UI: JWT token
  Customer->>UI: Add item and checkout
  UI->>API: Authenticated request with Bearer token
  API->>Handler: Create command using JWT user ID
  Handler->>DB: Load cart and products
  Handler->>DB: Decrease stock and create order
  Handler->>DB: Clear cart
  Handler-->>API: OrderDto
  API-->>UI: Order response
```

## Command class model

```mermaid
classDiagram
  class ProductCommands { +Handle(CreateProductCommand) ProductDto }
  class CartCommands { +Handle(AddToCartCommand) void }
  class OrderCommands { +Handle(CreateOrderCommand) OrderDto }
  class AuthController { +Register(RegisterRequest) JWT +Login(LoginRequest) JWT }
  class CustomerController { +Cart() +AddToCart(AddItemRequest) +Orders() }
  class IProductRepository
  class ICartRepository
  class IOrderRepository
  AuthController --> AppUser
  CustomerController --> CartCommands
  CustomerController --> ICartRepository
  CustomerController --> IProductRepository
  ProductCommands --> IProductRepository
  CartCommands --> ICartRepository
  CartCommands --> IProductRepository
  OrderCommands --> ICartRepository
  OrderCommands --> IProductRepository
  OrderCommands --> IOrderRepository
```

## Frontend route map

```mermaid
graph TD
  Home[/]
  Home --> Product[/products/:id]
  Home --> Cart[/cart]
  Cart --> Checkout[/checkout]
  Checkout --> Orders[/orders]
  Home --> Login[/login]
  Login --> Register[/register]
  Login --> Account[/account]
  Home --> Admin[/admin]
  Admin --> AdminProducts[/admin/products]
  Unknown --> NotFound[NotFound]
