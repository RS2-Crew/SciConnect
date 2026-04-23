# SciConnect - Project Guide

## Overview
SciConnect is a microservices-based scientific research collaboration platform built with .NET 8 and Angular 17. It manages institutions, employees, instruments, analyses, keywords, and microorganisms for scientific research networks. The target users are doctors and researchers.

## Architecture

### Microservices (all .NET 8 Web API)
| Service | Port | Database | Purpose |
|---------|------|----------|---------|
| **IdentityService** | 4000 | IdentityDb | Auth, JWT, user management, email notifications |
| **DB.API** | 4001 | EntityDb | Main CRUD for all domain entities (CQRS + MediatR) |
| **AnalyticsService** | 4002 | EntityDb (shared, read-only) | Summary stats, institution breakdowns |
| **SciConnectSPA** | 4200 | - | Angular 17 frontend (SSR + Nginx) |

### Infrastructure (Docker)
| Component | Port | Purpose |
|-----------|------|---------|
| **MS SQL Server 2017** | 1433 | Primary database (IdentityDb + EntityDb) |
| **RabbitMQ 4** | 5672 / 15672 | Event bus (MassTransit) |
| **Redis** | 6379 | Distributed cache (verification codes) |

### Inter-service Communication
- **RabbitMQ/MassTransit**: DB.API publishes entity creation events -> IdentityService consumes (notify_broadcast_queue) + AnalyticsService consumes (analytics_entity_queue)
- **Shared DB**: AnalyticsService reads directly from EntityDb — real-time data, no caching
- **No API Gateway**: Frontend calls each service directly on separate ports

## Project Structure
```
SciConnect/
  SciConnect/
    IdentityServer/IdentityService/    # Auth microservice
    Services/DB/
      DB.Domain/                       # Domain entities (EntityBase with audit fields)
      DB.Application/                  # CQRS handlers, validators (MediatR + FluentValidation)
      DB.API/DB.API/                   # REST controller, DI setup
    DB.Infrastructure/                 # EF Core repos, factories, migrations, seed data
    AnalyticsService/                  # Read-only analytics + MassTransit consumers
      Consumers/                       # InstitutionCreated, EmployeeCreated, SimpleEntityCreated
    Common/EventBus.Messages/          # Shared event contracts
    WebApps/SciConnectSPA/             # Angular 17 SPA
    docker-compose.yml                 # Base services
    docker-compose.override.yml        # Dev environment config
    SciConnect.sln                     # Solution (8 projects)
```

## Domain Model (EntityDb)
6 aggregate roots, all inherit EntityBase (CreatedBy, CreatedDate, LastModifiedBy, LastModifiedDate):
- **Institution** (Name, Street, StreetNumber, City, Country, Phone, Email, Website) - HiLo key generation
- **Employee** (FirstName, LastName) - belongs to Institution (N:1, RESTRICT delete)
- **Instrument** (Name)
- **Analysis** (Name)
- **Keyword** (Name)
- **Microorganism** (Name)

### Many-to-Many Relationships (junction tables)
- Institution <-> Instrument, Microorganism, Keyword, Analysis
- Analysis <-> Microorganism
- Employee <-> Keyword

## Pagination
All GetAll endpoints support pagination via query parameters:
- `?pageNumber=1&pageSize=10` (defaults)
- Response: `{ items: [...], pageNumber, pageSize, totalCount, totalPages, hasPreviousPage, hasNextPage }`
- Implementation: `PagedResult<T>` in DB.Application, `GetPagedAsync` in RepositoryBase
- Frontend sends `pageSize=1000` to get all data (pagination ready for future UI integration)

## Authentication & Authorization
- **JWT Bearer** with 15-min access tokens, 30-day refresh tokens
- **3 roles**: Guest (read-only), Administrator (read+write), PM (read+write+manage users)
- **Policies**: ReadAccess (Guest/Admin/PM), WriteAccess (Admin/PM)
- **Verification codes**: Redis-cached, 60-min TTL, for admin registration flow
- **PM registration**: Requires special hashed password from config

## Key Patterns
- **Clean Architecture**: Domain -> Application -> Infrastructure -> API
- **CQRS**: Commands (create/delete/add-relationship) and Queries (get/get-with-relations) via MediatR
- **Repository Pattern**: RepositoryBase<T> + entity-specific repos
- **Factory Pattern**: EntityFactories + ViewModelFactories
- **Validation Pipeline**: FluentValidation via MediatR ValidationBehavior
- **Event-Driven**: MassTransit publishes InstitutionCreatedEvent, EmployeeCreatedEvent, SimpleEntityCreatedEvent

## Frontend (Angular 17)
- **Theme**: Medical/healthcare teal palette (#0d9488 primary, #0f766e dark)
- **Dark mode**: Toggle via ThemeService, stored in component state
- **Components**: Login, Register, Dashboard (tabs: Analyses, Researchers, DB Management), Admin Management
- **Services**: DataService (CRUD), AnalyticsService, ThemeService, AppStateService (auth state)
- **Icons**: Font Awesome 6.4.0
- **CSS**: Bootstrap 5.3.3 base + custom component CSS

## Build & Run
```bash
# Full stack with Docker
cd SciConnect
docker-compose up --build

# Individual service (local dev)
dotnet run --project IdentityServer/IdentityService
dotnet run --project Services/DB/DB.API/DB.API
dotnet run --project AnalyticsService

# Frontend
cd WebApps/SciConnectSPA
npm install && npm start
```

## Database
- Auto-migrates on startup with retry (10 attempts, 3s delay)
- Seed data: 3 institutions, 3 employees, 3 instruments, 3 analyses, 3 keywords, 3 microorganisms with relationships
- Connection: `sa` / `MATF12345678rs2` (dev only)

## Remaining Technical Debt
1. **No UPDATE endpoints** - entities can only be created/deleted, not modified
2. **No API Gateway** - frontend calls services directly
3. **Hardcoded secrets** in appsettings (JWT key, DB password, email password) - dev environment only
4. **N+1 queries** in IdentityService consumers (loops GetRolesAsync per user)
5. **No refresh token rotation** - same token reused until expiry
6. **No rate limiting** on any service
7. **Case-sensitivity inconsistency** in name lookups across repositories
8. **Console.WriteLine** in GetInstrumentsByInstitutionQueryHandler (should use ILogger)
