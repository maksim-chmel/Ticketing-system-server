# Ticketing System — Admin Panel Backend

A RESTful backend for a support ticket management admin panel. Built with ASP.NET Core 8, it provides JWT-based authentication, ticket lifecycle management, user administration, and operational observability via Prometheus and OpenTelemetry.

> **Frontend repository:** [Ticketing System UI](https://github.com/maksim-chmel/Ticketing-system-ui-main)

---

## Features

- **Authentication** — JWT access tokens + HttpOnly cookie refresh tokens with automatic rotation
- **Ticket management** — view all feedback tickets, update status through a configurable lifecycle (`Open → InProgress → Waiting → Done / Rejected`)
- **User management** — list users, add admin comments per user
- **Broadcast messages** — create system-wide broadcast messages via a dedicated endpoint
- **Statistics** — status distribution and requests-over-time aggregations for dashboard charts
- **Observability** — structured logging with Serilog (console + Seq sink), Prometheus metrics endpoint, OpenTelemetry instrumentation for ASP.NET Core, HTTP client, and runtime
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
├── DTO/                # Response DTOs (UserDto, FeedbackDto, StatisticsDto)
├── Profiles/           # AutoMapper profiles
├── Middleware/         # Global error handling middleware
├── DB/                 # AppDbContext
└── Program.cs          # App bootstrap & DI composition
```

---

## API Endpoints

### Auth
| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/login` | Public | Login, returns access token; sets refresh token cookie |
| POST | `/api/auth/refresh` | Cookie | Rotates token pair |

### Feedback (tickets)
| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/feedback` | Bearer | Get all tickets |
| POST | `/api/feedback/update-status/{id}?status={value}` | Bearer | Update ticket status |

### Users
| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/user/users-to-list` | Bearer | Get user list |
| POST | `/api/user/update-comment` | Bearer | Add/update admin comment on user |

### Statistics
| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/statistics/status-distribution` | Bearer | Ticket count by status |
| GET | `/api/statistics/requests-over-time` | Bearer | Ticket volume over time |

### Broadcast
| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/broadcast/add-broadcastMessage` | Public | Queue a broadcast message |

### Metrics
| Route | Description |
|---|---|
| `/metrics` | Prometheus scraping endpoint |

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL (or Docker)

### Environment Variables

Create a `.env` file in the project root:

```env
DefaultConnection=Host=localhost;Port=5432;Database=ticketing;Username=postgres;Password=yourpassword
JWT_SECRET_KEY=your-secret-key-min-32-chars
JWT_ISSUER=your-issuer
JWT_AUDIENCE=your-audience
JWT_EXPIRES_IN_MINUTES=60
```

### Run locally

```bash
cd AdminPanelBack
dotnet run
```

The API will be available at `http://localhost:5101`. Swagger UI is available at `/swagger` in Development mode.

### Run with Docker Compose

```bash
docker compose up --build
```

---

## Architecture Notes

- **Repository pattern** — all data access is abstracted behind interfaces, keeping services testable and independent of EF Core
- **Service layer** — business logic is separated from controllers; each domain area (auth, feedback, users, stats, broadcast) has its own service with a corresponding interface
- **Global error handling** — `ErrorHandlingMiddleware` catches unhandled exceptions centrally, preventing implementation details from leaking into API responses
- **Token rotation** — refresh tokens are stored in the database and rotated on each use; expired or reused tokens are rejected

---

## Frontend Repository

[Ticketing System UI](https://github.com/maksim-chmel/Ticketing-system-ui-main) — React 19 + TypeScript admin interface with Recharts dashboards, served via Nginx.
