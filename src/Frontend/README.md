# eCommerce React frontend

This directory contains the React 19 + TypeScript + Vite storefront for the eCommerce application.

## Run

```powershell
npm install
npm run dev
```

The Vite development server runs at `http://localhost:5173`. The app expects the API at `http://localhost:5016/api` by default. Override it with:

```powershell
$env:VITE_API_URL = "http://localhost:5016/api"
npm run dev
```

## Available routes

- `/` - catalog
- `/products/:id` - product details
- `/cart` - cart
- `/checkout` - checkout
- `/orders` - order history
- `/login` - JWT login
- `/register` - account registration
- `/account` - authenticated account state
- `/admin` - admin dashboard UI
- `/admin/products` - product management UI

Navigation is implemented with browser history in `src/App.tsx` to keep the MVP dependency-light. Configure server-side fallback to `index.html` when deploying deep links outside Vite development.

## API integration

The catalog uses `GET /api/products`. Login and registration call `/api/auth/login` and `/api/auth/register`; the returned JWT is stored in `localStorage` under `ecommerce_token`. Authenticated customer endpoints are documented in the repository-level `docs/api.md`.

## Scripts

- `npm run dev` - start Vite with HMR
- `npm run build` - type-check and create a production bundle
- `npm run preview` - serve the production bundle locally
- `npm run lint` - run Oxlint

## Production notes

The checkout payment is currently a mock/local flow. Add a real payment provider, refresh-token handling, secure token storage, route guards, API error notifications, and server-backed cart/order synchronization before production deployment.
