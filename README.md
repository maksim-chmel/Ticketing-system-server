# Ticketing System — Admin Panel Backend [![Cloud CI (GitHub-hosted)](https://github.com/maksim-chmel/Ticketing-system-server/actions/workflows/ci-cloud.yml/badge.svg)](https://github.com/maksim-chmel/Ticketing-system-server/actions/workflows/ci-cloud.yml)

REST API backend for a support ticket management system. Part of a four-component platform — see [System Overview](#system-overview) below.

---

## System Overview

This project is one of four components that form a complete ticketing platform:

| Repository | Technology | Role |
|---|---|---|
| **ticketing-system-server** ← you are here | ASP.NET Core 8 | REST API, business logic, database |
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
     └── ticketing-system-server (this repo) ────────────────┘
              │
              ▼
     ticketing-system-ui (admin panel)
     Coordinators manage tickets, view stats, send broadcasts
```

---

## Features

- **Authentication** — JWT access tokens + HttpOnly cookie refresh tokens with automatic rotation
- **Ticket management** — view all feedback tickets, update status through a configurable lifecycle (`Open → InProgress → Waiting → Done / Rejected`)
- **User management** — list users, add admin comments per user
- **Broadcast messages** — queue system-wide messages delivered to all users via Telegram bot
- **Statistics** — status distribution and requests-over-time aggregations for dashboard charts
- **Observability** — structured logging with Serilog (console + Seq sink), Prometheus metrics endpoint, OpenTelemetry instrumentation
- **Auto-migration & seeding** — database migrates automatically on startup; default admin account is seeded if absent

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 |
| ORM | Entity Framework Core 8 + Npgsql (PostgreSQL) |
| Auth | ASP.NET Core Identity + JWT Bearer |
| Mapping | AutoMapper 8 |
| Logging | Serilog (Console + Seq) |
| Metrics | prometheus-net + OpenTelemetry |
| Containerization | Docker / Docker Compose |
| Config | DotNetEnv (`.env` file) |

---

## Project Structure

```
AdminPanelBack/
├── Controllers/        # AuthController, FeedbackController, UserController,
│                       # StatisticsController, BroadcastController
├── Services/           # Business logic (Auth, Login, Token, Feedback,
│                       # User, Statistics, Broadcast)
├── Repository/         # Data access layer with interface/implementation pairs
├── Models/             # Domain models (Feedback, User, Admin, BroadcastMessage, …)
├── DTO/                # Response DTOs
├── Profiles/           # AutoMapper profiles
├── Middleware/         # Global error handling middleware
├── DB/                 # AppDbContext
└── Program.cs          # App bootstrap & DI composition
```

---

## API Endpoints

All routes are prefixed with `/api`.

### Auth
| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/auth/login` | Public | Login, returns access token; sets HttpOnly `refreshToken` cookie |
| POST | `/auth/refresh` | Cookie | Refresh access token using `refreshToken` cookie (rotates refresh token) |

### Users (admin panel)
| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/users` | Bearer | List users |
| GET | `/users/{userId:long}` | Bearer | Get user by id |
| PATCH | `/users/{userId:long}/comment` | Bearer | Update admin comment (`UpdateUserCommentRequest`) |

### Feedbacks (tickets)
| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/feedbacks` | Bearer | List all feedback tickets |
| PATCH | `/feedbacks/{id:int}` | Bearer | Update ticket status (`UpdateFeedbackStatusRequest`) |

### Statistics
| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/statistics/status-distribution` | Bearer | Ticket count by status |
| GET | `/statistics/requests-over-time` | Bearer | Ticket volume over time |

### Broadcast messages
| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/broadcast-messages` | Bearer | Queue a broadcast message (`CreateBroadcastMessageRequest`), returns `204` |

### Operator API (used by bots)
| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/operator/feedbacks` | Public | Create feedback (`UsersMessageDto`), returns `204` |
| GET | `/operator/user-ids` | Public | Get all user ids |
| GET | `/operator/users/{userId:long}` | Public | Get user by id |
| PUT | `/operator/users/{userId:long}` | Public | Upsert user (`UserDto`), returns `204` |
| GET | `/operator/users/{userId:long}/feedbacks` | Public | List feedbacks for a user |
| POST | `/operator/broadcast-message-pulls` | Public | Pull active broadcast messages and mark them inactive |
| POST | `/operator/unnotified-feedback-pulls` | Public | Pull unnotified feedbacks and mark them as sent to operator |

### Metrics
| Route | Description |
|---|---|
| `/metrics` | Prometheus scraping endpoint |

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Docker (recommended)

### Quick Start (Recruiter Friendly)

Run with Docker Compose (app + db + seq):

```bash
docker compose up -d --build
```

The backend listens on `http://localhost:5101` and Swagger UI is available at `/swagger` in Development mode.

### Environment Variables

Create a `.env` file in the project root:

```env
DefaultConnection=Host=localhost;Port=5432;Database=feedbackdb;Username=postgres;Password=yourpassword
JWT_SECRET_KEY=your-secret-key-minimum-32-chars
JWT_ISSUER=adminpanel
JWT_AUDIENCE=frontadminpanel
JWT_EXPIRES_IN_MINUTES=60
CORS_ORIGIN=http://localhost:3000
SEQ_URL=http://localhost:5341
```

If you're using the included `compose.yaml`, PostgreSQL is exposed on host port `5433`:

```env
DefaultConnection=Host=localhost;Port=5433;Database=feedbackdb;Username=postgres;Password=yourpassword
```

### Run locally

```bash
cd AdminPanelBack
dotnet run
```

Swagger UI is available at `/swagger` in Development mode.

---

## Architecture Notes

- **Repository pattern** — all data access is abstracted behind interfaces
- **Service layer** — business logic separated from controllers; each domain area has its own service with a corresponding interface
- **Global error handling** — `ErrorHandlingMiddleware` catches unhandled exceptions centrally
- **Token rotation** — refresh tokens stored in the database and rotated on each use
## Frontend Repository

[Ticketing System UI](https://github.com/maksim-chmel/Ticketing-system-ui-main) — React 19 + TypeScript admin interface with Recharts dashboards, served via Nginx.
