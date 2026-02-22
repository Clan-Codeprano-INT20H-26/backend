# Backend

Modular monolith REST API built with ASP.NET Core 10.

## Tech Stack
- ASP.NET Core 10
- Entity Framework Core
- PostgreSQL
- Docker

## Project Structure
```
src/
├── Backend.Api                 # Entry point
├── Backend.Modules.Shared      # Shared models, interfaces, DTOs
│   ├── DTOs/
│   ├── Interfaces/
│   ├── Models/
│   └── Exceptions/
└── Backend.Modules.{Entity}    # Business modules
    ├── Domain/
    ├── Application/
    ├── Infrastructure/
    └── Presentation/
```

## Getting Started

### Prerequisites
- .NET 10 SDK
- Docker

### Run
```bash
docker-compose up
```

### Run locally
```bash
dotnet restore
dotnet run --project src/Backend.Api
```

## Adding a New Module
1. Create `Backend.Modules.{Name}` project
2. Add `Domain`, `Application`, `Infrastructure`, `Presentation` folders
3. Add interface to `Backend.Modules.Shared/Interfaces/{Name}/`
4. Add DTOs to `Backend.Modules.Shared/DTOs/{Name}/`
5. Register module in `Program.cs`

## Conventions
- DTOs are placed in `Backend.Modules.Shared/DTOs/{ModuleName}/`
- Interfaces are placed in `Backend.Modules.Shared/Interfaces/{ModuleName}/`
- Each module follows the same structure: `Domain`, `Application`, `Infrastructure`, `Presentation`
- Use classic controllers, not Minimal API

## Branching
- `main` — production
- `dev` — working branch
- `feature/name` — feature branches, merge into `dev`