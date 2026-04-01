# EmployedNow API MVP

EmployedNow is a LinkedIn-like MVP backend built with .NET 8 Web API.

## Tech Stack
- .NET 8 Web API
- Entity Framework Core + SQLite
- JWT Authentication
- BCrypt Password Hashing
- Swagger / OpenAPI
- Docker + docker-compose

## Project Structure
- src/EmployedNow.Domain
- src/EmployedNow.Application
- src/EmployedNow.Infrastructure
- src/EmployedNow.Api
- tests/EmployedNow.Api.IntegrationTests

## Prerequisites
- .NET SDK 8.x
- Docker Desktop (optional, for container run)

## Quick Start
```bash
docker-compose up --build
```

API base URL: `http://localhost:5000`

Swagger UI: `http://localhost:5000/swagger`

## Run Locally (without Docker)
```bash
dotnet restore EmployedNow.sln
dotnet build EmployedNow.sln
dotnet run --project src/EmployedNow.Api/EmployedNow.Api.csproj
```

Local API base URL: `http://localhost:5000`

## Database Migrations
This repository uses EF Core migrations and includes an initial migration at:
- `src/EmployedNow.Infrastructure/Data/Migrations/20260401153058_InitialCreate.cs`

If you change the model, create a new migration with:
```bash
dotnet tool restore
dotnet tool run dotnet-ef migrations add <MigrationName> \
  --project src/EmployedNow.Infrastructure/EmployedNow.Infrastructure.csproj \
  --startup-project src/EmployedNow.Api/EmployedNow.Api.csproj \
  --output-dir Data/Migrations
```

## Seeded Accounts
- Company: `company@employednow.local` / `Company123!`
- User 1: `seeker1@employednow.local` / `Seeker123!`
- User 2: `seeker2@employednow.local` / `Seeker123!`

## Implemented MVP Features
- Authentication
  - POST /api/auth/register
  - POST /api/auth/login
- Job Board
  - POST /api/jobs (Company only)
  - GET /api/jobs (Public + Pagination)
  - GET /api/jobs/{jobId} (Public)
- Applications
  - POST /api/applications (User only)
  - GET /api/applications/job/{jobId} (Company owner only)
- Networking
  - POST /api/connections (User only)
  - PUT /api/connections/{requestId} (Target user only)
  - GET /api/connections (User only)
- Premium Integration
  - POST /api/webhooks/premium (Shared secret in `X-Webhook-Secret`)

## Default Webhook Secret
`employednow-premium-secret`

## Integration Tests
Integration tests live under `tests/EmployedNow.Api.IntegrationTests`.

Run tests:
```bash
dotnet test EmployedNow.sln
```

Current test coverage includes:
- Health endpoint availability
- Public jobs endpoint accessibility without JWT
- Protected endpoint rejection for anonymous requests
- Login with seeded company returns JWT

## Smoke Check (Verified)
The API was smoke-tested by launching the app and calling `/health`:
- Request: `GET http://localhost:5000/health`
- Result: `200 OK`

## Notes
- Only registration, login, and job listing/detail are public.
- Duplicate job applications are blocked.
- Duplicate pending connection requests are blocked.
- Premium status is persisted but not used for feature gating in this MVP.
- SQLite path is normalized to an absolute location at startup to avoid local path issues.
- Functions and non-obvious logic blocks are documented with inline comments in the implementation.
