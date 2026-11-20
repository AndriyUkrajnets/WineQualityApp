# WineQualityApp

WineQualityApp is being built incrementally as three applications: a React
frontend, an ASP.NET Core API, and a Python ML service. The .NET API calls the
Python service; the frontend will call only the .NET API.

## Run the Python service

Follow [python-service/README.md](python-service/README.md) to install its
dependencies, add the trusted model artifact, and run FastAPI on port 8000.

## Run the .NET API

From this directory:

```powershell
dotnet run --project src/WineQualityApp.Api
```

The Python service URL defaults to `http://127.0.0.1:8000/`. Override it with
the `PythonService__BaseUrl` environment variable when needed.

In Development, the OpenAPI document is available at `/openapi/v1.json` on the
address printed by `dotnet run`.

## Current API scope

- `POST /api/predictions` validates the wine input, calls Python `/predict`,
  and returns its continuous `predicted_quality` result.
- `GET /health` reports whether the .NET API is running.

The API does not yet persist predictions or provide history. SQL Server and
history endpoints are a later milestone.

## Run the React frontend

Start the Python ML service and .NET API, then from this directory:

```powershell
cd frontend
npm.cmd install
npm.cmd run dev
```

The frontend is served at `http://localhost:5173`. Vite proxies `/api` and
`/health` to `http://localhost:5294`. Override the target with
`VITE_API_PROXY_TARGET` when using a different .NET API address.

## Run .NET unit tests

Stop a running .NET API instance, then run from this directory:

```powershell
dotnet test WineQualityApp.sln
```
