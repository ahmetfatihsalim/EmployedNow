# EmployedNow Full-Stack MVP

EmployedNow is a Job Postboard MVP with a .NET 8 Web API backend and a React + TypeScript frontend designed for clear non-technical demos.

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

How to Run EmployedNow MVP

  Ensure Docker Desktop is running.
  Open a terminal in the project root folder.
  Run the following command in terminal 

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

## Strategic Product Roadmap
This roadmap converts the current MVP into a market-ready hiring and professional network platform.

### Planning Assumptions
1. The current MVP remains the base product and production-hardening starts immediately.
2. A realistic outcome is a strong 3-month foundation plus a 3-month scaling extension.
3. Product priorities are candidate experience, recruiter efficiency, trust/safety, and platform reliability.

### Target Product Pillars
1. Career Marketplace: job discovery, application lifecycle, recruiter pipeline management.
2. Professional Network: profiles, connections, messaging-ready graph, activity signals.
3. Platform Integrations: payment, CRM/ERP, notification channels, and enterprise workflows.
4. Operational Excellence: security, observability, CI/CD, and scalable architecture.

### Core Technology Direction
1. Backend: .NET  as the primary stack, modular service architecture, REST-first APIs.
2. Data: PostgreSQL as production database, Redis for cache/session/rate limiting.
3. Async Workloads: RabbitMQ or Kafka for event-driven workflows and background processing.
4. Search: Elasticsearch or OpenSearch for advanced job/profile search.
5. Infrastructure: Docker baseline, Kubernetes for scale environments.
6. Cloud: AWS, Azure, or GCP with environment isolation for dev/staging/production.
7. Delivery and Quality: Git-based branching, code review gates, CI/CD pipelines, automated tests.
8. Operations: centralized logging, metrics, tracing, alerting, and incident runbooks.

### 3-Month Execution Plan (Foundation to Product-Market Readiness)

#### Month 1: Production Foundation and Core Domain Expansion
Goals
1. Stabilize architecture for scale and prepare a reliable production baseline.
2. Upgrade identity, authorization, and data model depth for real-world use.
3. Build recruiter and candidate profile essentials.

Business Deliverables
1. Company profile pages and candidate profile CV-style structure.
2. Job lifecycle model improvement (draft, published, archived, closed).
3. Recruiter workflow baseline (job creation, candidate list, status updates).

Technical Deliverables
1. Database migration path from SQLite to PostgreSQL.
2. Role and permission model hardening for candidate, recruiter, company admin.
3. Redis integration for caching hot reads and controlling repeat requests.
4. Baseline CI pipeline for build, test, and deployment checks.
5. Structured logging and error classification standards.

Exit Criteria
1. Platform runs consistently in staged environments with PostgreSQL.
2. Recruiters and candidates can manage richer profiles and job states.
3. Critical flows have measurable performance and error baselines.

#### Month 2: Marketplace Intelligence and Workflow Automation
Goals
1. Improve job-candidate matching and discovery relevance.
2. Introduce automation for high-volume operational actions.
3. Prepare enterprise-friendly integration surfaces.

Business Deliverables
1. Smart filtering and relevance-based job discovery.
2. Application tracking stages for recruiters (new, review, interview, rejected, hired).
3. Candidate notifications for application progress and recruiter activity.

Technical Deliverables
1. Search service rollout with Elasticsearch/OpenSearch.
2. Background jobs and event processing with RabbitMQ or Kafka.
3. Notification adapters for email and push-ready architecture.
4. Integration gateway contracts for ERP, CRM, e-invoice, and shipping-like external systems.
5. API versioning policy and partner-facing documentation standards.

Exit Criteria
1. Search quality and response speed are stable under realistic load.
2. Recruiter pipeline flow is fully usable end-to-end.
3. Event-driven architecture handles asynchronous workloads reliably.

#### Month 3: Monetization, Trust, and Go-to-Market Readiness
Goals
1. Launch premium and commercial features.
2. Increase trust with security, moderation, and operational governance.
3. Prepare for controlled public rollout.

Business Deliverables
1. Premium recruiter and premium candidate feature packages.
2. Sponsored jobs and visibility boost capabilities.
3. Admin console for abuse handling, fraud signals, and account governance.

Technical Deliverables
1. Payment and subscription lifecycle orchestration (webhooks, retries, reconciliation).
2. Security hardening: audit logs, secrets management, access policy reviews.
3. Full observability stack with SLA/SLO dashboards and alerting.
4. Blue/green or canary deployment strategy in CI/CD.
5. Load testing and capacity planning for launch traffic.

Exit Criteria
1. Monetization flows are auditable and operationally safe.
2. Platform can sustain target launch traffic with defined response objectives.
3. Product is ready for a managed beta and early revenue generation.

### Month 4-6 Extension (Recommended for Competitive Parity)
The first 3 months are enough for a strong launch candidate. Full competitive parity with established players is more realistic with a 6-month horizon.

#### Month 4: Engagement and Network Effects
1. Introduce feed-ready activity events and profile interactions.
2. Add recommendation engines for jobs, companies, and connections.
3. Start messaging architecture (secure direct messaging foundation).

#### Month 5: Enterprise and Ecosystem Expansion
1. Multi-tenant company workspaces and team permissions.
2. Advanced analytics for recruiters (funnel, source quality, conversion).
3. Expanded integration marketplace (ATS, HRIS, CRM, billing systems).

#### Month 6: Scale, Compliance, and Internationalization
1. Regional scaling strategy and multi-language readiness.
2. Compliance controls (privacy policy tooling, retention rules, consent tracking).
3. Cost optimization and platform reliability improvements at scale.

### KPI Framework for the Roadmap
1. Candidate Activation: profile completion, first application rate, weekly active users.
2. Recruiter Productivity: time-to-first-qualified-candidate, pipeline conversion rates.
3. Marketplace Liquidity: jobs posted, applications per job, interview rate.
4. Revenue Health: premium conversion, sponsor campaign ROI, churn.
5. Platform Reliability: API latency, error rate, deployment success rate, incident recovery time.

### Leadership Model (Tech Lead + Entrepreneur Lens)
1. Build for speed in Month 1, for leverage in Month 2, and for defensibility in Month 3.
2. Keep architecture modular so revenue experiments do not destabilize the core platform.
3. Make every major feature measurable with KPIs before further scaling investment.
4. Prioritize integration-readiness early to unlock B2B expansion and long-term enterprise value.
