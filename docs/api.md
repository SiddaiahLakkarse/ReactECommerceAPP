# API contract

All request and response bodies are JSON. JWT-protected routes require:

```http
Authorization: Bearer <jwt-token>
```

## Authentication

| Method | Route | Purpose |
|---|---|---|
| POST | `/api/auth/register` | Create a user and return a JWT |
| POST | `/api/auth/login` | Validate credentials and return a JWT |

Registration body:

```json
{ "email": "customer@example.com", "password": "Password123!", "displayName": "Customer" }
```

Login body:

```json
{ "email": "customer@example.com", "password": "Password123!" }
```

## Catalog

| Method | Route | Auth | Purpose |
|---|---|---|---|
| GET | `/api/products` | No | List active products |
| POST | `/api/products` | Current MVP: No | Create a product |

Product creation body:

```json
{ "name": "Notebook", "description": "Recycled paper", "price": 14.00, "stockQuantity": 30 }
```

## Customer and orders

| Method | Route | Auth | Purpose |
|---|---|---|---|
| GET | `/api/customer/cart` | Yes | Get the authenticated user's cart |
| POST | `/api/customer/cart/items` | Yes | Add `{ productId, quantity }` to the cart |
| GET | `/api/customer/orders` | Yes | List the authenticated user's orders |
| POST | `/api/orders` | Yes | Create an order from the authenticated user's cart |

The order handler validates the cart, snapshots product names/prices, decreases stock, creates the order, clears the cart, and returns an `OrderDto`.

## Error behavior

- `400 Bad Request` - invalid request or password policy failure
- `401 Unauthorized` - missing/invalid JWT or invalid credentials
- `404 Not Found` - missing product
- `409 Conflict` - recommended production mapping for insufficient stock or cart conflicts
- `500 Internal Server Error` - unexpected failures; production should return RFC 7807 ProblemDetails

Swagger/OpenAPI is enabled in Development.
