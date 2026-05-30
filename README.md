# Admin Panel Backend [![Cloud CI (GitHub-hosted)](https://github.com/maksim-chmel/AdminPanelBack/actions/workflows/ci-cloud.yml/badge.svg)](https://github.com/maksim-chmel/AdminPanelBack/actions/workflows/ci-cloud.yml)

REST API backend for a support ticket management system. Part of a four-component platform — see [System Overview](#system-overview) below.

---

## System Overview

This project is one of four components that form a complete ticketing platform:

| Repository | Technology | Role |
|---|---|---|
| **[AdminPanelBack](https://github.com/maksim-chmel/AdminPanelBack)** ← you are here | ASP.NET Core 8 | REST API, business logic, database |
| [ticketing-system-ui](https://github.com/maksim-chmel/Ticketing-system-ui) | React 19 + TypeScript | Admin panel for coordinators |
| [feedback_bot](https://github.com/maksim-chmel/feedback_bot) | Node.js + TypeScript | Telegram bot for end users |
| [alarm_bot](https://github.com/maksim-chmel/alarm_bot) | Node.js + TypeScript | Telegram bot that notifies operators of new tickets |

```
User (Telegram)
     │ creates ticket via feedback_bot
     ▼
PostgreSQL ◄──────────────────────────────────────────────────
     │                                                        │
     ├── alarm_bot polls every 15s → notifies operator       │
     │                                                        │
     └── AdminPanelBack (this repo) ─────────────────────────┘
              │
              ▼
     ticketing-system-ui (admin panel)
     Coordinators manage tickets, view stats, send broadcasts
```

---

## Features

- **Authentication** — JWT access tokens + HttpOnly cookie refresh tokens with automatic rotation; rate limiting on login/refresh endpoints
- **Ticket management** — view all feedback tickets (paginated), update status through a configurable lifecycle (`Open → InProgress → Waiting → Done / Rejected`)
- **User management** — list users (paginated), add admin comments per user
- **Broadcast messages** — queue system-wide messages delivered to all users via Telegram bot
- **Statistics** — status distribution and requests-over-time aggregations for dashboard charts
- **Response caching** — Redis-backed output cache for read-heavy admin endpoints; tag-based invalidation on writes
- **Bot API security** — operator endpoints protected with `X-Api-Key` header
- **Observability** — structured logging with Serilog (console + Seq sink), Prometheus metrics endpoint, OpenTelemetry instrumentation
- **Auto-migration & seeding** — database migrates on startup when enabled; default admin account is seeded if absent
- **Request validation** — FluentValidation on DTOs

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 |
| ORM | Entity Framework Core 8 + Npgsql (PostgreSQL) |
| Cache | ASP.NET Core Output Caching + StackExchange.Redis |
| Auth | ASP.NET Core Identity + JWT Bearer + API key (bots) |
| Mapping | AutoMapper 12 |
| Validation | FluentValidation |
| Logging | Serilog (Console + Seq) |
| Metrics | prometheus-net + OpenTelemetry |
| Containerization | Docker / Docker Compose |
| Config | DotNetEnv (`.env` file) |

---

## Project Structure

```
AdminPanelBack/
├── Controllers/        # Auth, Feedback, User, Statistics, Broadcast, BotFeedback
├── Services/           # Business logic (Auth, Login, Token, Feedback, User, …)
├── Repository/         # Data access layer with interface/implementation pairs
├── Models/             # Domain models (Feedback, User, Admin, BroadcastMessage, …)
├── DTO/                # Request/response DTOs
├── Validators/         # FluentValidation rules
├── Profiles/           # AutoMapper profiles
├── Middleware/         # ErrorHandlingMiddleware, ApiKeyAuthFilter,
│                       # AuthorizedOutputCachePolicy
├── DB/                 # AppDbContext
└── Program.cs          # App bootstrap & DI composition
```

---

## API Endpoints

All routes are prefixed with `/api`.

### Auth
| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/auth/login` | Public (rate limited) | Login, returns access token; sets HttpOnly `refreshToken` cookie |
| POST | `/auth/refresh` | Cookie (rate limited) | Refresh access token using `refreshToken` cookie (rotates refresh token) |

### Users (admin panel)
| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/users?page=&pageSize=` | Bearer | List users (cached, paginated) |
| GET | `/users/{userId:long}` | Bearer | Get user by id (cached) |
| PATCH | `/users/{userId:long}/comment` | Bearer | Update admin comment (`UpdateUserCommentRequest`) |

### Feedbacks (tickets)
| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/feedbacks?page=&pageSize=` | Bearer | List all feedback tickets (cached, paginated) |
| PATCH | `/feedbacks/{id:int}` | Bearer | Update ticket status (`UpdateFeedbackStatusRequest`) |

### Statistics
| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/statistics/status-distribution` | Bearer | Ticket count by status (cached) |
| GET | `/statistics/requests-over-time` | Bearer | Ticket volume over time (cached) |

### Broadcast messages
| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/broadcast-messages` | Bearer | Queue a broadcast message (`CreateBroadcastMessageRequest`), returns `204` |

### Operator API (used by bots)
| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/operator/feedbacks` | `X-Api-Key` | Create feedback (`UsersMessageDto`), returns `204` |
| GET | `/operator/user-ids` | `X-Api-Key` | Get all user ids |
| GET | `/operator/users/{userId:long}` | `X-Api-Key` | Get user by id |
| PUT | `/operator/users/{userId:long}` | `X-Api-Key` | Upsert user (`UserDto`), returns `204` |
| GET | `/operator/users/{userId:long}/feedbacks` | `X-Api-Key` | List feedbacks for a user |
| POST | `/operator/broadcast-message-pulls` | `X-Api-Key` | Pull active broadcast messages and mark them inactive |
| POST | `/operator/unnotified-feedback-pulls` | `X-Api-Key` | Pull unnotified feedbacks and mark them as sent to operator |

### Metrics
| Route | Description |
|---|---|
| `/metrics` | Prometheus scraping endpoint |

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL
- Redis (required for output caching)
- Docker (recommended)

### Quick Start (Docker Compose)

One command — no `.env` required (sensible defaults for local demo):

```bash
docker compose up -d --build
```

| Service | URL |
|---|---|
| API | http://localhost:5101 |
| Swagger | http://localhost:5101/swagger |
| Seq | http://localhost:8081 |
| PostgreSQL (host) | `localhost:5433`, db `feedbackdb`, user `postgres`, password `postgres` |

**Default admin** (created on first startup if none exists):

| | |
|---|---|
| Email | `admin@example.com` |
| Password | `YourStrongAdminPassword123!` |

Login: `POST http://localhost:5101/api/auth/login` with `{"username":"admin@example.com","password":"YourStrongAdminPassword123!"}`.

Bot API key (header `X-Api-Key`): `dev-demo-api-key`.

Override any value via `.env` (see `.env.example`). For production, set strong secrets and do not rely on compose defaults.

### Environment Variables

Create a `.env` file in the project root (see `.env.example`):

```env
DefaultConnection=Host=localhost;Port=5432;Database=feedbackdb;Username=postgres;Password=yourpassword
JWT_SECRET_KEY=your-secret-key-minimum-32-chars
JWT_ISSUER=adminpanel
JWT_AUDIENCE=frontadminpanel
JWT_EXPIRES_IN_MINUTES=60
CORS_ORIGIN=http://localhost:3000
SEQ_URL=http://localhost:5341
REDIS_CONNECTION_STRING=localhost:6379
API_KEY=your-bot-api-key
ADMIN_EMAIL=admin@example.com
ADMIN_PASSWORD=YourStrongAdminPassword123!
MIGRATE_ON_STARTUP=true
```

| Variable | Required | Description |
|---|---|---|
| `DefaultConnection` | Yes | PostgreSQL connection string |
| `REDIS_CONNECTION_STRING` | Yes | Redis connection string for output cache |
| `JWT_SECRET_KEY` | Yes | Symmetric key for JWT signing |
| `JWT_ISSUER` / `JWT_AUDIENCE` | Yes | JWT issuer and audience |
| `API_KEY` | Yes (for bots) | Shared secret for `X-Api-Key` on `/api/operator/*` |
| `ADMIN_EMAIL` / `ADMIN_PASSWORD` | No | Seed default admin if none exists |
| `MIGRATE_ON_STARTUP` | No | Apply EF migrations on startup (`true` in compose by default) |
| `CORS_ORIGIN` | No | Allowed frontend origin (default `http://localhost:3000`) |
| `SEQ_URL` | No | Seq log server URL |

If you're using the included `compose.yaml`, PostgreSQL is exposed on host port `5433`:

```env
DefaultConnection=Host=localhost;Port=5433;Database=feedbackdb;Username=postgres;Password=yourpassword
```

### Run locally

```bash
cd AdminPanelBack
dotnet run
```

The app listens on port `8080` (see `Program.cs`). Swagger UI is available at `/swagger` in Development mode.

---

## Architecture Notes

- **Repository pattern** — all data access is abstracted behind interfaces
- **Service layer** — business logic separated from controllers; each domain area has its own service with a corresponding interface
- **Global error handling** — `ErrorHandlingMiddleware` catches unhandled exceptions centrally
- **Token rotation** — refresh tokens stored in the database and rotated on each use
- **Rate limiting** — fixed-window limiter on `/api/auth/*` (10 requests per 10 seconds, queue limit 2)

### Output caching

Read-heavy admin GET endpoints are cached in Redis with tag-based invalidation:

| Policy | TTL | Vary by | Tag | Endpoints |
|---|---|---|---|---|
| `AdminFeedbacksPolicy` | 60 s | `page`, `pageSize` | `feedbacks` | `GET /feedbacks` |
| `AdminUsersListPolicy` | 60 s | `page`, `pageSize` | `users` | `GET /users` |
| `AdminUserByIdPolicy` | 2 min | — | `users` | `GET /users/{id}` |
| `AdminStatisticsPolicy` | 3 min | — | `statistics` | `GET /statistics/*` |

`AuthorizedOutputCachePolicy` only stores successful `GET`/`HEAD` responses with status `200`. Mutations call `IOutputCacheStore.EvictByTagAsync` so related cache entries are cleared (e.g. updating a feedback evicts `feedbacks` and `statistics`).

---

## Frontend Repository

[Ticketing System UI](https://github.com/maksim-chmel/Ticketing-system-ui) — React 19 + TypeScript admin interface with Recharts dashboards, served via Nginx.
