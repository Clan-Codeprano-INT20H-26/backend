# Backend
Also read frontend README

Frontent Link: https://github.com/Clan-Codeprano-INT20H-26/iwk-frontend

Prod link: https://backend-5zqo.onrender.com/swagger

Modular monolith REST API built with ASP.NET Core 10.

## Dataset
The standard dataset provided in the assignment does not fit our application's requirements. The custom dataset used for this project is located in the `/TestData` directory.

## Admin Creds
Email: admin@gmail.com

Password: admin111

## Tech Stack
- ASP.NET Core 10
- Entity Framework Core
- PostgreSQL
- Docker

## Project Structure
```
src/
├── Backend.Api
│   ├── Backend.Api.csproj
│   ├── Backend.Api.http
│   ├── Dockerfile
│   ├── Program.cs
│   ├── Properties
│   ├── appsettings.Development.json
│   ├── appsettings.json
├── Backend.Module.Kit
│   ├── Application
│   ├── Backend.Module.Kit.csproj
│   ├── Domain
│   ├── Infrastructure
│   ├── KitMigrationExtensions.cs
│   ├── KitModuleRegistration.cs
│   ├── Migrations
│   ├── Presentation
├── Backend.Module.Tax
│   ├── Application
│   ├── Backend.Module.Tax.csproj
│   ├── Data
│   ├── Domain
│   ├── Infrastructure
│   ├── Migrations
│   ├── Presentation
│   ├── TaxMigrationExtensions.cs
│   ├── TaxModulesRegistration.cs
├── Backend.Modules.Auth
│   ├── Application
│   ├── AuthMigrationExtensions.cs
│   ├── AuthModuleRegistration.cs
│   ├── Backend.Modules.Auth.csproj
│   ├── Domain
│   ├── Infrastructure
│   ├── Interfaces
│   ├── Migrations
│   ├── Presentation
├── Backend.Modules.Order
│   ├── Application
│   ├── Backend.Modules.Order.csproj
│   ├── Domain
│   ├── Infrastructure
│   ├── Migrations
│   ├── OrderMigrationExtensions.cs
│   ├── OrdersModulesRegistration.cs
│   ├── Presentation
├── Backend.Modules.Payment
│   ├── Application
│   ├── Backend.Modules.Payment.csproj
│   ├── PaymentModuleRegistration.cs
│   ├── Presentation
├── Backend.Modules.Shared
│   ├── Backend.Modules.Shared.csproj
│   ├── DTOs
│   ├── Exceptions
│   ├── Infrastructure
│   ├── Interfaces
│   ├── Models
│   ├── SharedModuleRegistration.cs

```

## Geodata and taxes
Placed at src/Backend.Module.Tax/Data

## Assumptions & Constraints
The current implementation of the service contains a number of business assumptions and simplifications optimized for New York State

1. State Limitation: The service only supports locations within NY state. If the provided coordinates do not fall into any of the known counties in the database, the service returns an error.

2. Fixed State Rate: The State Rate is hardcoded at 4%
3. Tax Overlap (City vs County): If a city tax is found for the location (CityRate > 0), the effective county rate is forcibly set to 0. It is assumed that the city tax absorbs or replaces the county tax.
4. New York City Specifics: For the five boroughs of NYC (Bronx, Kings, New York, Queens, Richmond), the county tax is always forcibly set to 0.
5. Transportation Surcharge (MCTD): There is a list of 12 counties that belong to the Metropolitan Commuter Transportation District. If a location falls within one of them, a fixed Special Rate of 0.375% is automatically applied.
6. Name Matching: Linking geometric polygons to the rates table (TaxRates) is done via loose string comparison (.Contains()). This requires the jurisdiction names in the TaxRates table to be substrings of the names in the spatial tables (Counties/Cities).
## Getting Started

### Prerequisites
- .NET 10 SDK
- Docker


### Run
Rename .env.example to .env

```bash
docker-compose up --build
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