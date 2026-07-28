# Iron & Breath — Home Workout Tracker

A full-stack tracker for a 4-day dumbbell strength split with a 3-month
progressive-overload program, a guided timed session player, a sun-salutation
warm-up, and an instructional video attached to every exercise.

**Multi-user** with isolated data: sign up with email/password or Google, and
each account gets its own editable program, settings, and logged sessions.
Every user's program is a clone of the seeded template that they can fully
customise (add/edit/remove/reorder days & exercises). Mobile-friendly UI, and
SQLite → PostgreSQL is a config flag, not a code change.

---

## Stack

| Layer      | Choice |
|------------|--------|
| Frontend   | React 19 + TypeScript, Vite, React Router, TanStack Query |
| Backend    | ASP.NET Core 8 Web API (controllers) |
| ORM / DB   | EF Core 8 + SQLite (dev) → Npgsql/PostgreSQL (later), EF Migrations |
| Video      | Curated YouTube id per exercise, provider-agnostic DTO, self-hosted seam ready |

> **Version notes:** the project targets **.NET 8** (`net8.0`) even though the
> installed SDK is 10 — the ASP.NET 8 runtime is present. The Vite scaffold
> pinned **React 19** (fully compatible with the chosen libraries). The warm-up
> is defined in **configuration** (`WarmUp` section) rather than a DB entity,
> since there is only ever one warm-up and §2.3 wants pacing params in config.

---

## Repo layout

```
/iron-and-breath
  /server
    /IronAndBreath.Api              ASP.NET Core Web API (controllers, DTOs, services, seed)
    /IronAndBreath.Domain           Entities, enums, pure calculators (progression, stats)
    /IronAndBreath.Infrastructure   DbContext, EF config, migrations, provider selection
    /IronAndBreath.Tests            xUnit tests (calculators, migration+seed, PG compatibility)
  /client                           React + Vite app (features/dashboard, session-player, history, shared)
  docker-compose.yml                Optional Postgres for the cutover (SQLite needs no container)
  README.md
```

---

## Prerequisites

- **.NET SDK 8+** (SDK 10 works; the app targets `net8.0` and the ASP.NET 8 runtime)
- **Node 18+** and npm
- EF Core tools: `dotnet tool install --global dotnet-ef` (any 8+ version)

---

## Getting started

Two terminals.

### 1) API

```bash
cd server/IronAndBreath.Api
dotnet run
```

- Runs on `http://localhost:5201` (and `https://localhost:7250`).
- In **Development** it automatically applies migrations, seeds the 4-day split
  **template**, and runs the video seeder. Per-user settings and each user's own
  editable copy of the program are created when they sign up.
- Swagger UI: `http://localhost:5201/swagger`.
- HTTPS redirection is **disabled in Development** so the Vite proxy can reach
  the API over plain HTTP.

### 2) Client

```bash
cd client
npm install
npm run dev
```

- Runs on `http://localhost:5173`.
- `/api/*` is proxied to the API (`vite.config.ts`), so there's no CORS in dev.
  Override the target with `VITE_API_TARGET`, or point a deployed build at an
  absolute API with `VITE_API_BASE`.

---

## Database & migrations

Migrations are used from day one (never `EnsureCreated`) so they run against
both SQLite and PostgreSQL.

- **Applied automatically** on startup in Development (`DbInitializer.MigrateAndSeedAsync`).
  Guarded to Development so production can migrate out-of-band.
- **Location:** `server/IronAndBreath.Infrastructure/Persistence/Migrations`.
- **Static seed** (4 days, 24 exercises, 3 phases) is baked into the migration
  via `HasData`. The settings row and videos are seeded at runtime.

### Add a migration

```bash
cd server
dotnet ef migrations add <Name> \
  --project IronAndBreath.Infrastructure \
  --startup-project IronAndBreath.Api \
  --output-dir Persistence/Migrations
```

### Reset the local database

The SQLite file lives in `server/IronAndBreath.Api/data/` (git-ignored).

```bash
# from server/IronAndBreath.Api
rm -rf data           # delete ironandbreath.db (+ -wal/-shm)
dotnet run            # recreates + re-seeds on next start
```

---

## Reseeding exercises & videos

- **Exercises / days / phases:** edit
  `server/IronAndBreath.Infrastructure/Persistence/Seed/SeedData.cs`, then add a
  migration (see above). This is the source of truth for the static program.
- **Videos:** edit
  `server/IronAndBreath.Api/Seed/video-seed.json` — map each exercise to a
  YouTube id you've reviewed for correctness/licensing. Leave `youTubeId` empty
  to show the "no video yet" placeholder. The seeder is **idempotent**: fill
  ids, restart the API, done. Duplicate exercise names (e.g. Push-ups) share the
  clip by name. Thumbnails are auto-derived for YouTube when not provided.

---

## Configuration (`appsettings.json`)

```jsonc
{
  "Database": { "Provider": "Sqlite", "ConnectionString": "Data Source=data/ironandbreath.db" },
  "Cors":     { "AllowedOrigins": ["http://localhost:5173", "http://localhost:4173"] },
  "WarmUp":   { "Name": "Surya Namaskar", "Rounds": 4, "SecondsPerRound": 50, "TransitionSeconds": 15 },
  "VideoSeed":{ "Path": "Seed/video-seed.json" }
}
```

Everything is overridable via environment variables (e.g.
`Database__Provider`, `Database__ConnectionString`).

---

## SQLite → PostgreSQL migration (cutover)

The provider is selected in one place (`DependencyInjection.ConfigureProvider`)
from the `Database` config. Queries are LINQ-only (no provider-specific SQL) and
verified against Npgsql by an offline test
(`PostgresModelCompatibilityTests`).

This repo ships the **SQLite** migration set. At cutover, regenerate a fresh
**Postgres** set (the approach chosen from brief §6):

1. Start Postgres (see `docker-compose.yml`):
   ```bash
   docker compose up -d postgres
   ```
2. Point the app at Postgres (env or `appsettings`):
   ```bash
   # PowerShell
   $env:Database__Provider = "Postgres"
   $env:Database__ConnectionString = "Host=localhost;Database=ironandbreath;Username=postgres;Password=postgres"
   ```
3. Replace the migration set with a Postgres one. Remove the SQLite migrations
   folder (or keep it in source control on a branch), then:
   ```bash
   cd server
   $env:DATABASE_PROVIDER = "Postgres"
   $env:DATABASE_CONNECTION_STRING = "Host=localhost;Database=ironandbreath;Username=postgres;Password=postgres"
   dotnet ef migrations add InitialCreate \
     --project IronAndBreath.Infrastructure --startup-project IronAndBreath.Api \
     --output-dir Persistence/Migrations
   ```
   (`AppDbContextFactory` reads `DATABASE_PROVIDER` / `DATABASE_CONNECTION_STRING`
   so the EF tooling targets Postgres.)
4. Run the API — it applies the Postgres migrations and seeds on startup.

> The design-time factory and the app share the same provider-selection logic,
> so tooling and runtime never disagree.

---

## Testing

```bash
cd server
dotnet test
```

Covers the highest-risk logic: progression phase/week calculation, stats &
streak edge cases (grace period for the in-progress week, off-by-one week
boundaries), migration + seed integrity (SQLite in-memory), and Npgsql model
compatibility.

---

## Accounts & authentication

Auth is JWT bearer tokens. Two ways in:

- **Email + password** — passwords hashed with ASP.NET Core `PasswordHasher`
  (PBKDF2). `POST /api/auth/register` and `POST /api/auth/login`.
- **Google** — the SPA renders the Google Identity Services button; the backend
  verifies the returned ID token (`Google.Apis.Auth`). Config-driven: set the
  Google **Web Client ID** and it activates automatically (button appears,
  `/api/auth/google` accepts tokens). No client id ⇒ Google is simply hidden and
  email/password still works.

On first sign-in a user is **provisioned**: the seeded template program
(4 days / 24 exercises, `UserId == null`) is cloned into user-owned, editable
rows, and a settings row is created. All data endpoints are `[Authorize]`d and
scoped to the caller — users never see each other's data.

The token is stored in `localStorage` and attached as `Authorization: Bearer`.
A `401` on a protected endpoint clears it and drops back to the login screen.

### Configuring Google sign-in

1. In Google Cloud Console → *APIs & Services → Credentials*, create an
   **OAuth 2.0 Client ID** of type *Web application*.
2. Add your app origin to **Authorized JavaScript origins**
   (`http://localhost:5173` for Vite dev, `http://localhost:8080` for the Docker
   web container, plus your production origin).
3. Provide the client id to the API:
   ```bash
   # env (works for dotnet run and docker)
   Google__ClientId=xxxxxxxx.apps.googleusercontent.com
   ```
   or `appsettings`/`.env`. Nothing is hardcoded and no secret is committed.

## Configure the workout

The **Configure** page (`/settings`) is a full editor:

- Program settings: start date, weekly target.
- Per day: rename, edit focus, delete, reorder.
- Per exercise: name, target text, rep range, base sets, cue, attached video
  (from the shared library), delete, reorder.

Every change is user-scoped and immediately reflected on the dashboard/session
player.

## API surface

```
POST   /api/auth/register                create an account (email + password)  -> token
POST   /api/auth/login                   sign in                               -> token
POST   /api/auth/google                  verify a Google ID token / link/create-> token
GET    /api/auth/me                      current user (authorized)
GET    /api/auth/config                  which providers are enabled (public)

GET    /api/workout-days                 caller's days with nested exercises + video refs
GET    /api/workout-days/{id}            single day
POST   /api/workout-days                 create a day
PUT    /api/workout-days/{id}            rename / edit focus
DELETE /api/workout-days/{id}            delete a day (blocked if it has logged sessions)
POST   /api/workout-days/reorder         reorder days by id
POST   /api/workout-days/{id}/exercises              add an exercise
PUT    /api/workout-days/{id}/exercises/{exId}       edit an exercise
DELETE /api/workout-days/{id}/exercises/{exId}       remove an exercise
POST   /api/workout-days/{id}/exercises/reorder      reorder a day's exercises
GET    /api/videos                       shared instructional-video library (for the picker)

GET    /api/program/phase-today          current phase number + parameters
GET    /api/program/warmup               warm-up parameters
GET    /api/sessions?from=&to=           session history (date range optional)
POST   /api/sessions                     log a completed/abandoned session (phase computed server-side)
PATCH  /api/sessions/{id}                edit a logged session
DELETE /api/sessions/{id}                delete a logged session
GET    /api/stats/summary                totals, this week/month, streak, avg/week, per-day counts
GET    /api/settings  ·  PUT /api/settings   program start date + weekly target
```

_All endpoints except `/api/auth/*` (register/login/google/config) require a bearer token._

## Docker (full stack)

```bash
docker compose up --build
#   web -> http://localhost:8080   SPA + /api reverse proxy (nginx)
#   api -> http://localhost:5201   Swagger at /swagger
```

Or use a launcher (creates `.env` from `.env.example` on first run, then brings
the stack up). **Docker** and **Podman** variants are provided — pick your engine:

```
# Docker
docker-up.cmd            # Windows (double-click or run in a terminal)
./docker-up.ps1          # PowerShell (add -d to detach)
./docker-up.sh           # macOS / Linux / Git Bash

# Podman (uses "podman compose", falls back to podman-compose)
podman-up.cmd  ·  ./podman-up.ps1  ·  ./podman-up.sh
```

Stop with `docker-down.cmd` / `podman-down.cmd` (or `… compose down`; add `-v`
to drop the data volume). Images are fully-qualified (`docker.io/library/…`,
`mcr.microsoft.com/…`) so Podman's short-name resolution doesn't prompt.

## Debugging in VS Code

`.vscode/launch.json` + `tasks.json` are included:

- **API (.NET)** — builds and launches the API, opens Swagger when it's ready
  (needs the C# / C# Dev Kit extension).
- **Client (Vite + Chrome / Edge)** — starts the dev server and opens a debug
  browser.
- **Full stack (API + Client)** — a compound that runs both.
- Tasks: `build-api`, `client-dev`, `test`, and `podman-up`.

Press **F5** and choose a configuration.

- `web` builds the Vite SPA and serves it with nginx, which proxies `/api` to
  the `api` container (same browser origin, so no CORS).
- `api` runs the ASP.NET Core app (SQLite persisted in the `ironandbreath-data`
  volume; migrates + seeds on start).
- Set a real `JWT_KEY` (≥ 32 bytes) and optional `GOOGLE_CLIENT_ID` via env or a
  `.env` file next to `docker-compose.yml`:
  ```env
  JWT_KEY=please-change-this-to-a-long-random-string-min-32-bytes
  GOOGLE_CLIENT_ID=xxxxxxxx.apps.googleusercontent.com
  ```
- Postgres is available under a profile: `docker compose --profile postgres up -d postgres`.
