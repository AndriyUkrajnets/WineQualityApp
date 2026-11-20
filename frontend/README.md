# WineQuality frontend

React, TypeScript, and Vite frontend for the wine-quality predictor. The browser
calls only the .NET API. In local development, Vite proxies `/api` and `/health`
to the .NET API so the browser stays on one origin.

## Run locally

Start the Python ML service and .NET API first. Then, from this directory:

```powershell
npm.cmd install
npm.cmd run dev
```

Vite serves the app at `http://localhost:5173`. By default, the proxy target is
`http://localhost:5294`; override it with `VITE_API_PROXY_TARGET` if the .NET
API uses another address.

To make direct browser requests to a separately hosted .NET API, set
`VITE_API_BASE_URL` and add the frontend origin to
`Cors:AllowedOrigins` in the API configuration.

## Production build

```powershell
npm.cmd run build
```

Prediction history is shown as a placeholder until the .NET history endpoint
and SQL Server persistence are implemented.
