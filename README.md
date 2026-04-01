# EmployedNow Full-Stack MVP

EmployedNow is a LinkedIn-like MVP with a .NET 8 Web API backend and a React + TypeScript frontend designed for clear non-technical demos.

## Tech Stack
- Backend
  - .NET 8 Web API
  - Entity Framework Core + SQLite
  - JWT Authentication + BCrypt Password Hashing
  - Swagger / OpenAPI
- Frontend
  - Vite + React + TypeScript
  - Tailwind CSS
  - React Router DOM
  - React Context API (auth state)
  - Axios (JWT interceptor)
- Platform
  - Docker + docker-compose

## Project Structure
- src/EmployedNow.Domain
- src/EmployedNow.Application
- src/EmployedNow.Infrastructure
- src/EmployedNow.Api
- frontend
- tests/EmployedNow.Api.IntegrationTests

## Prerequisites
- .NET SDK 8.x
- Node.js 20+ (local frontend dev)
- Docker Desktop (optional, for container run)

## Quick Start
```bash
docker-compose up --build
```

URLs after startup:
- Frontend: `http://localhost:5173`
- API base URL: `http://localhost:5000`
- Swagger UI: `http://localhost:5000/swagger`

## Run Locally (without Docker)
```bash
dotnet restore EmployedNow.sln
dotnet build EmployedNow.sln
dotnet run --project src/EmployedNow.Api/EmployedNow.Api.csproj

cd frontend
npm install
npm run dev
```

Local URLs:
- Frontend: `http://localhost:5173`
- API: `http://localhost:5000`

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
- User Discovery
  - GET /api/users (Authenticated, optional role filter)
  - GET /api/users/me (Authenticated)
  - PUT /api/users/me/premium (Authenticated)
- Job Board
  - POST /api/jobs (Company only)
  - PUT /api/jobs/{jobId} (Company owner only)
  - DELETE /api/jobs/{jobId} (Company owner only)
  - GET /api/jobs (Public + Pagination)
  - GET /api/jobs/{jobId} (Public)
- Applications
  - POST /api/applications (User only)
  - GET /api/applications/job/{jobId} (Company owner only)
- Networking
  - POST /api/connections (Authenticated)
  - PUT /api/connections/{requestId} (Target user only)
  - GET /api/connections (Authenticated)
- Premium Integration
  - POST /api/webhooks/premium (Shared secret in `X-Webhook-Secret`)

## Default Webhook Secret
`employednow-premium-secret`

## Frontend Pages
- `/` public jobs board
- `/jobs/:id` public job details
- `/login` login form
- `/register` registration form (role dropdown)
- `/dashboard` role-based dashboard
  - Company: create, update, delete own jobs and view applicants
  - User: see open positions and apply directly
- `/network` authenticated networking (discover users, send/accept requests)
- `/me` user profile and premium upgrade action

## Use Case Guide
Use this section as a practical playbook for demos and daily flows.

### 1. New candidate joins and applies
1. Open the register page and create a User account.
2. Go to Dashboard and review open positions.
3. Apply from Dashboard or open a job details page and apply there.
4. Expected outcome: the application is submitted and shown as successful.

### 2. Company posts a new position
1. Register or log in as a Company account.
2. Open Dashboard and fill in title and description in Create Job.
3. Submit the post.
4. Expected outcome: the new job appears in the company job list and in the public jobs board.

### 3. Company updates or removes a posting
1. In Company Dashboard, go to Your Job Posts.
2. Select Edit to update title/description, or Delete to remove the posting.
3. Expected outcome: changes are reflected immediately in the dashboard and public listing.

### 4. Company reviews applicants
1. In Company Dashboard, open View Applicants.
2. Select one of your job posts.
3. Load applicants.
4. Expected outcome: applicant email list is displayed for that job.

### 5. Build network connections
1. Log in and open Network.
2. In Discover Users, send a connection request.
3. As the target account, log in and open Incoming Requests.
4. Accept the request.
5. Expected outcome: the request moves from pending to accepted state.

### 6. Upgrade account to premium
1. Log in and open User Info.
2. Select Upgrade to Premium.
3. Expected outcome: profile shows Premium as Yes and session reflects the new status.

### 7. Public visitor browsing flow
1. Open the home page without logging in.
2. Browse jobs and open details pages.
3. Expected outcome: job browsing works, while account-only actions require login.

### 8. Handling repeated actions
1. Try applying to the same job twice from the same account.
2. Or send an already-pending connection request again.
3. Expected outcome: the system blocks duplicates and shows a clear message.

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

## Frontend Build Test
```bash
cd frontend
npm run build
```

## Smoke Check (Verified)
The API was smoke-tested by launching the app and calling `/health` with `200 OK`.

## Notes
- Only registration, login, and job listing/detail are public.
- Duplicate job applications are blocked.
- Duplicate pending connection requests are blocked.
- Premium status is persisted but not used for feature gating in this MVP.
- SQLite path is normalized to an absolute location at startup to avoid local path issues.
- CORS is configured to allow frontend origins through `Cors:AllowedOrigins`.
- Frontend trusts backend validation and surfaces standardized API error messages.
