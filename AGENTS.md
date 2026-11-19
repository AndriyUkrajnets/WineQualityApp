# WineQualityApp — Codex Project Instructions

## 1. Project Overview

WineQualityApp is a learning project designed to demonstrate the integration of a machine learning model into a full-stack web application.

The application predicts wine quality based on physicochemical characteristics using an existing trained Scikit-learn regression model serialized as a Joblib file.

The project is intended to demonstrate:
- ASP.NET Core backend development.
- Integration between .NET and Python through REST APIs.
- Practical machine learning model serving.
- React frontend development.
- SQL Server database integration.
- Docker-based application deployment.
- Clean, maintainable and extensible software architecture.

Keep the initial implementation simple. Avoid unnecessary abstractions and technologies.

## 2. Technology Stack

### Backend
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- HttpClient / IHttpClientFactory
- Swagger / OpenAPI
- xUnit

### Frontend
- React
- TypeScript
- Vite
- React hooks
- Vitest

### Machine Learning Service
- Python
- FastAPI
- Pydantic
- Scikit-learn
- Joblib
- pytest

### Infrastructure
- Docker
- Docker Compose
- Git monorepo

## 3. System Architecture

The application consists of three independently running applications and a database.

### React Frontend
Responsibilities:
- Display the wine input form.
- Collect 11 numerical wine characteristics.
- Allow selection of wine color (red or white).
- Send prediction requests to the .NET API.
- Display prediction results.
- Display prediction history.

React must communicate only with the .NET backend, not directly with Python.

### .NET Backend
Responsibilities:
- Expose REST API endpoints.
- Validate incoming requests.
- Coordinate the prediction workflow.
- Communicate with the Python ML service.
- Save prediction requests and results to SQL Server.
- Provide prediction history and details.

### Python ML Service
Responsibilities:
- Load the existing Joblib model.
- Validate prediction requests.
- Apply the exact preprocessing required by the trained model.
- Execute model inference.
- Return predicted wine quality.

The model must be loaded once during application startup, not for every prediction.

Python must not access the application database.

### SQL Server
Responsibilities:
- Store prediction input features.
- Store predicted quality.
- Store prediction creation timestamp.
- Provide persistent prediction history through the .NET backend.

## 4. Machine Learning Model Requirements

An existing trained regression model is available as a Joblib file.

The current artifact export script in `C:\projects\Data Science\Regression\wine_quality` saves a fitted Scikit-learn `Pipeline`, not a bare estimator. It contains a `ColumnTransformer` and a `RandomForestRegressor`. The preprocessing passes the 11 numeric features through unchanged and one-hot encodes `color` (`red` or `white`). The final Random Forest does not use numerical scaling. Confirm the artifact's actual type and feature metadata when it is added to this repository; if it differs from the export script, inspect the training code and reproduce that artifact's preprocessing exactly.

Before implementing inference:
1. Inspect the model and its expected input.
2. Inspect the original training notebook or script when available.
3. Identify the exact feature order: the 11 numeric features listed below, followed by `color`.
4. Confirm numerical scaling or transformations from the artifact and training code. The current export uses numeric passthrough for the Random Forest.
5. Confirm wine color encoding from the artifact and training code. The current export uses `OneHotEncoder(handle_unknown="ignore")` inside the pipeline.
6. Reuse preprocessing contained in the fitted pipeline; do not duplicate it in the service. If the artifact is a bare estimator instead, reproduce the original preprocessing exactly.
7. Verify predictions against known examples from the original training environment before relying on production predictions.

Never assume that raw input values can be passed directly to the model.

Do not retrain or replace the model unless explicitly requested.

Never silently change model preprocessing.

Only load trusted Joblib files.

## 5. Wine Input Features

The prediction request contains the following 12 features:

1. fixed_acidity
2. volatile_acidity
3. citric_acid
4. residual_sugar
5. chlorides
6. free_sulfur_dioxide
7. total_sulfur_dioxide
8. density
9. pH
10. sulphates
11. alcohol
12. color (red or white)

The model predicts a continuous numerical quality score.

Do not convert the regression result into a classification unless explicitly requested.

## 6. API Design

### .NET API

POST /api/predictions
- Accept wine characteristics.
- Validate input.
- Request prediction from Python.
- Save input and prediction to SQL Server.
- Return the saved prediction.

GET /api/predictions
- Return prediction history.

GET /api/predictions/{id}
- Return an individual prediction.

GET /health
- Return backend health information.

### Python API

POST /predict
- Accept wine characteristics.
- Perform preprocessing and inference.
- Return predicted quality.

GET /health
- Return ML service health information, including model readiness.

### Communication

- Use JSON for all API communication.
- React communicates only with .NET.
- .NET communicates with Python using a typed HttpClient.
- Python communicates with the model locally.
- Only .NET accesses SQL Server.

## 7. Backend Architecture

Use a lightweight layered architecture.

The .NET solution should contain:

- WineQualityApp.Api
- WineQualityApp.Application
- WineQualityApp.Domain
- WineQualityApp.Infrastructure

Responsibilities:

Api:
- Controllers
- HTTP configuration
- Dependency injection setup

Application:
- DTOs
- Interfaces
- Application services
- Prediction workflow

Domain:
- Entities
- Core domain types

Infrastructure:
- EF Core
- DbContext
- Database configurations
- Python API HTTP client

Keep dependency direction consistent with this architecture.

Avoid introducing MediatR, CQRS, repositories for every entity, or other unnecessary abstractions.

## 8. Database

Use SQL Server and Entity Framework Core.

Initially, use one Predictions table containing:
- Id
- All 11 numerical wine features
- Color
- PredictedQuality
- CreatedAt

Use EF Core Code First migrations.

The database connection must be configurable through application configuration and environment variables.

During initial development, SQL Server runs locally on the developer's machine.

Do not assume SQL Server is already running in Docker.

## 9. Frontend Requirements

Keep the frontend simple.

Initial pages:
- PredictorPage
- HistoryPage

Components:
- WineForm
- PredictionResult
- PredictionTable

Use:
- React functional components.
- TypeScript interfaces.
- React hooks for local state.
- A separate API service layer.

Do not introduce Redux or complex state management unless future requirements justify it.

## 10. Docker and Deployment

Docker Compose will eventually orchestrate:
- React frontend
- .NET backend
- Python ML service
- SQL Server container

However, initial development should run the applications separately.

Dockerization is a later development milestone.

Use environment-based configuration so that local development and containerized deployment can use different service URLs and connection strings.

Use persistent storage for SQL Server in Docker.

Never commit secrets, passwords, connection credentials or private environment files.

## 11. Testing Requirements

### .NET
- xUnit unit tests.
- Test prediction service.
- Test request validation.
- Test Python client failure scenarios.
- Test API endpoints.

### Python
- pytest.
- Test model loading.
- Test preprocessing.
- Test valid predictions.
- Test invalid input.

### React
- Vitest.
- Test form validation.
- Test API integration behavior.
- Test prediction result rendering.

Add tests as functionality is implemented.

## 12. Development Principles

Follow these rules throughout the project:

1. Keep implementations simple and readable.
2. Prefer explicit, understandable code over unnecessary abstractions.
3. Follow SOLID principles where they provide practical value.
4. Use dependency injection.
5. Use asynchronous operations for I/O.
6. Use structured logging in .NET.
7. Validate data at application boundaries.
8. Handle errors explicitly.
9. Keep configuration outside business logic.
10. Avoid duplicating business logic between services.
11. Do not introduce new frameworks or dependencies without a clear reason.
12. Do not implement functionality outside the current task unless explicitly requested.

## 13. Development Roadmap

Implement the application incrementally in this order:

1. Inspect and understand the existing ML model and preprocessing.
2. Build the Python FastAPI service.
3. Build the .NET 10 Web API.
4. Integrate .NET with Python.
5. Integrate SQL Server using EF Core.
6. Build the React frontend.
7. Add automated tests.
8. Containerize the complete application.
9. Document setup and architecture.

Each stage should be tested before proceeding to the next.

## 14. How Codex Should Work

When starting a new task:

1. Read and follow these instructions.
2. Inspect the existing repository structure and code.
3. Understand the current implementation before modifying it.
4. Identify the smallest reasonable implementation.
5. Explain the proposed approach briefly.
6. Implement only the requested task.
7. Run relevant tests and builds.
8. Report changed files, implementation decisions and test results.

Do not recreate existing functionality unnecessarily.

Do not assume that previous tasks have been completed unless verified in the repository.

If a requirement is ambiguous or could affect the architecture, ask for clarification before making significant changes.

Keep the README and project documentation updated when architectural decisions or setup procedures change.
