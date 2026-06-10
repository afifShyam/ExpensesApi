# Setup Guide

This guide walks you through setting up the **ExpenseApi** project on a macOS development environment.

## Prerequisites
- **.NET SDK 8.0** (or latest LTS) – install via `brew install --cask dotnet-sdk`
- **Git** – for source control.
- **Docker Desktop** – optional, for containerised database and runtime.
- **PostgreSQL** – local instance (or use Docker) – required for production profile.
- **Node.js** (optional) – if you want to use the Swagger UI with custom front‑end tooling.

## Clone the Repository
```bash
git clone https://github.com/yourusername/ExpenseApi.git
cd ExpenseApi
```

## Solution Structure
```
ExpenseApi/
├─ src/
│   ├─ Core/
│   ├─ Application/
│   ├─ Infrastructure/
│   └─ Presentation/
├─ tests/
├─ doc/   ← documentation files (this folder)
└─ ExpenseApi.sln
```

## Local Development Database (SQLite)
The test configuration uses an in‑memory SQLite DB, so you can start the API without any external DB.

## Production Database (PostgreSQL)
You can spin up PostgreSQL with Docker:
```bash
docker run -d \
  --name expense-postgres \
  -e POSTGRES_USER=expense_user \
  -e POSTGRES_PASSWORD=secretpwd \
  -e POSTGRES_DB=expense_db \
  -p 5432:5432 \
  postgres:15-alpine
```
Update the connection string in `appsettings.Development.json` accordingly.

## Build the Solution
```bash
dotnet build
```

## Apply Database Migrations
Navigate to the **Infrastructure** project and run EF Core migrations:
```bash
cd src/Infrastructure
# Ensure the EF Core tools are installed
dotnet tool install --global dotnet-ef
# Add a migration (only the first time)
 dotnet ef migrations add InitialCreate
# Apply the migration to the DB (PostgreSQL)
 dotnet ef database update
```
For SQLite (tests) no action is needed – migrations will be applied automatically on first use.

## Run the API Locally
```bash
cd src/Presentation
dotnet run
```
The API will be available at `https://localhost:5001` (or `http://localhost:5000`). Swagger UI is reachable at `/swagger`.

## Run Unit / Integration Tests
```bash
cd ../../tests
dotnet test
```
All tests should pass.

## Dockerise the Application
A simplified `Dockerfile` is provided in the `Presentation` project.
```bash
cd src/Presentation
docker build -t expenseapi .
# Run container (ensure PostgreSQL is reachable)
 docker run -d -p 8080:80 \
   -e ConnectionStrings__Default=Host=host.docker.internal;Port=5432;Database=expense_db;Username=expense_user;Password=secretpwd \
   expenseapi
```
The API will be reachable at `http://localhost:8080`.

---
*Keep this file up‑to‑date as tooling or project structure evolves.*
