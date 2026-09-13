# API contract

All request bodies are JSON. Validation failures should be returned as 400 responses; missing products return 404; unavailable stock and empty carts return 500 in this MVP and should be mapped to ProblemDetails/409 in production.

| Method | Route | Purpose |
|---|---|---|
| GET | `/api/products` | List active products |
| POST | `/api/products` | Create product |
| POST | `/api/cart/items` | Add product quantity to a user cart |
| POST | `/api/orders` | Convert the user's cart into an order |

The current sample uses a `userId` field in command bodies. Replace it with an authenticated claims principal when Identity/JWT is enabled.
