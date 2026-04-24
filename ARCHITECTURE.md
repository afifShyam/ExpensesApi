# ExpenseApi Architecture & Data Flow

## Overview

This is a **Layered Architecture** ASP.NET Core Web API using **Entity Framework Core** with **PostgreSQL**. The application follows a clean separation of concerns with 4 main layers.

```
┌─────────────────────────────────────────────────────────────────┐
│                         CLIENT                                  │
│              (Browser, Postman, Mobile App)                     │
└─────────────────────┬───────────────────────────────────────────┘
                      │ HTTP Request (JSON)
                      ▼
┌─────────────────────────────────────────────────────────────────┐
│  ┌──────────────────────────────────────────────────────────┐   │
│  │              CONTROLLER LAYER                            │   │
│  │         (ExpensesController.cs)                          │   │
│  │  • Receives HTTP requests                                │   │
│  │  • Validates input (ModelState)                          │   │
│  │  • Returns HTTP responses (200, 404, 201, etc.)         │   │
│  │  • NO business logic here                                │   │
│  └────────────────────┬─────────────────────────────────────┘   │
│                       │ Calls Service Interface                  │
│                       ▼                                          │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │              SERVICE LAYER                               │   │
│  │         (ExpenseService.cs)                              │   │
│  │  • Contains business logic                               │   │
│  │  • Transforms DTOs to/from Models                        │   │
│  │  • Coordinates between Controller and Repository         │   │
│  │  • Data conversion & mapping                             │   │
│  └────────────────────┬─────────────────────────────────────┘   │
│                       │ Calls Repository Interface               │
│                       ▼                                          │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │              REPOSITORY LAYER                            │   │
│  │         (ExpenseRepository.cs)                           │   │
│  │  • Handles all database operations                       │   │
│  │  • Uses Entity Framework Core                            │   │
│  │  • Returns/saves Entity Models                             │   │
│  │  • NO business logic here                                │   │
│  └────────────────────┬─────────────────────────────────────┘   │
│                       │ Uses EF Core                            │
│                       ▼                                          │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │              DATA LAYER                                  │   │
│  │         (ExpenseDbContext + PostgreSQL)                  │   │
│  │  • DbContext manages database connections                │   │
│  │  • Tracks entity changes                                 │   │
│  │  • Generates SQL queries                                 │   │
│  └──────────────────────────────────────────────────────────┘   │
│                                                                  │
│                    Dependency Injection (DI)                      │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  • IExpenseService → ExpenseService (Scoped)             │   │
│  │  • IExpenseRepository → ExpenseRepository (Scoped)       │   │
│  │  • ExpenseDbContext → PostgreSQL (Scoped)                │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

---

## Data Flow: Create Expense

```
POST /api/expenses
     │
     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpensesController.Create(CreateExpenseDto dto)        │
│  • Validates DTO using Data Annotations                 │
│  • Calls _expenseService.CreateAsync(dto)             │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpenseService.CreateAsync(CreateExpenseDto dto)       │
│  • Maps DTO → Expense Model                             │
│    { Title=dto.Title, Amount=dto.Amount, ... }        │
│  • Calls _expenseRepository.CreateAsync(expense)        │
│  • Converts result back to ExpenseResponseDto         │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpenseRepository.CreateAsync(Expense expense)         │
│  • Sets CreatedAt = DateTime.UtcNow                     │
│  • _context.Expenses.Add(expense)                     │
│  • await _context.SaveChangesAsync()                    │
│  • Returns saved Expense with generated Id              │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpenseDbContext + PostgreSQL                          │
│  • INSERT INTO "Expenses" (...) VALUES (...)            │
│  • Auto-generates Id (auto-increment)                   │
│  • Commits transaction                                    │
└─────────────────────────────────────────────────────────┘
                     │
                     ▲
┌────────────────────┴────────────────────────────────────┐
│  Returns ExpenseResponseDto with Id, CreatedAt, etc.      │
│  HTTP 201 Created with Location header                  │
└─────────────────────────────────────────────────────────┘
```

---

## Data Flow: Get All Expenses

```
GET /api/expenses
     │
     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpensesController.GetAll()                            │
│  • Calls _expenseService.GetAllAsync()                  │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpenseService.GetAllAsync()                           │
│  • Calls _expenseRepository.GetAllAsync()               │
│  • Maps List<Expense> → IEnumerable<ExpenseResponseDto> │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpenseRepository.GetAllAsync()                        │
│  • await _context.Expenses                              │
│      .AsNoTracking()          ← Read-only optimization  │
│      .OrderByDescending(e => e.Date)                    │
│      .ToListAsync()                                       │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpenseDbContext + PostgreSQL                          │
│  • SELECT ... FROM "Expenses" ORDER BY "Date" DESC      │
│  • EF Core maps rows to Expense objects                 │
└─────────────────────────────────────────────────────────┘
                     │
                     ▲
┌────────────────────┴────────────────────────────────────┐
│  Returns IEnumerable<ExpenseResponseDto>                  │
│  HTTP 200 OK with JSON array                            │
└─────────────────────────────────────────────────────────┘
```

---

## Data Flow: Get Single Expense

```
GET /api/expenses/{id}
     │
     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpensesController.GetById(int id)                     │
│  • Calls _expenseService.GetByIdAsync(id)               │
│  • Returns 404 if null, 200 if found                    │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpenseService.GetByIdAsync(int id)                    │
│  • Calls _expenseRepository.GetByIdAsync(id)            │
│  • Maps to ExpenseResponseDto if found                  │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpenseRepository.GetByIdAsync(int id)                 │
│  • await _context.Expenses                              │
│      .AsNoTracking()                                      │
│      .FirstOrDefaultAsync(e => e.Id == id)              │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpenseDbContext + PostgreSQL                          │
│  • SELECT ... FROM "Expenses" WHERE "Id" = @id          │
│  • Returns single row or null                           │
└─────────────────────────────────────────────────────────┘
```

---

## Data Flow: Update Expense

```
PUT /api/expenses/{id}
     │
     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpensesController.Update(int id, UpdateExpenseDto dto)  │
│  • Validates DTO                                          │
│  • Calls _expenseService.UpdateAsync(id, dto)           │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpenseService.UpdateAsync(int id, UpdateExpenseDto) │
│  • Maps DTO → Expense Model (without Id)                │
│  • Calls _expenseRepository.UpdateAsync(id, expense)    │
│  • Converts result to DTO if successful                 │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpenseRepository.UpdateAsync(int id, Expense expense) │
│  • FindAsync(id) to get existing entity                   │
│  • Returns null if not found                            │
│  • Updates properties:                                    │
│    existing.Title = expense.Title                       │
│    existing.Amount = expense.Amount                     │
│    ... + UpdatedAt = DateTime.UtcNow                  │
│  • await _context.SaveChangesAsync()                    │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpenseDbContext + PostgreSQL                          │
│  • SELECT ... WHERE "Id" = @id                          │
│  • UPDATE "Expenses" SET ... WHERE "Id" = @id             │
│  • Commits transaction                                  │
└─────────────────────────────────────────────────────────┘
                     │
                     ▲
┌────────────────────┴────────────────────────────────────┐
│  Returns ExpenseResponseDto                             │
│  HTTP 200 OK or 404 Not Found                           │
└─────────────────────────────────────────────────────────┘
```

---

## Data Flow: Delete Expense

```
DELETE /api/expenses/{id}
     │
     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpensesController.Delete(int id)                      │
│  • Calls _expenseService.DeleteAsync(id)                │
│  • Returns 404 if not found, 204 if deleted               │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpenseService.DeleteAsync(int id)                     │
│  • Calls _expenseRepository.DeleteAsync(id)             │
│  • Returns boolean result                                 │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpenseRepository.DeleteAsync(int id)                  │
│  • FindAsync(id) to get entity                            │
│  • Returns false if not found                             │
│  • _context.Expenses.Remove(expense)                    │
│  • await _context.SaveChangesAsync()                    │
│  • Returns true on success                                │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpenseDbContext + PostgreSQL                          │
│  • DELETE FROM "Expenses" WHERE "Id" = @id                │
│  • Commits transaction                                  │
└─────────────────────────────────────────────────────────┘
```

---

## Object Transformation Flow

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│   Incoming DTO  │────▶│  Service Layer  │────▶│  Entity Model   │
│  (JSON → C#)    │     │  (Maps data)    │     │  (Database)     │
│                 │     │                 │     │                 │
│ CreateExpenseDto│     │  new Expense()  │     │  Expense        │
│ UpdateExpenseDto│     │  { ... }        │     │  { Id, ... }    │
└─────────────────┘     └─────────────────┘     └─────────────────┘
                                                        │
                                                        ▼
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│  Outgoing DTO   │◀────│  Service Layer  │◀────│   Repository    │
│  (C# → JSON)    │     │  (Maps to DTO)  │     │  (Returns data) │
│                 │     │                 │     │                 │
│ ExpenseResponse │     │  ToResponseDto()│     │  Expense        │
│    { Id, ... }  │     │  { Id, ... }    │     │  { Id, ... }    │
└─────────────────┘     └─────────────────┘     └─────────────────┘
```

---

## Dependency Injection Setup

See `@/Users/shyam/Development/ExpenseApi/Program.cs:6-16`

```csharp
// Scoped = New instance per HTTP request
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();

// Database context - also scoped per request
builder.Services.AddDbContext<ExpenseDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

**Why Scoped?**
- Each HTTP request gets its own instances
- DbContext tracks entities for that request only
- Repositories and Services share the same DbContext instance within a request
- Prevents data mixing between concurrent requests

---

## API Endpoints Summary

| Method | Endpoint | Controller Action | Description |
|--------|----------|-------------------|-------------|
| GET | `/api/expenses` | GetAll() | List all expenses, newest first |
| GET | `/api/expenses/{id}` | GetById(id) | Get single expense by ID |
| POST | `/api/expenses` | Create(dto) | Create new expense |
| PUT | `/api/expenses/{id}` | Update(id, dto) | Update existing expense |
| DELETE | `/api/expenses/{id}` | Delete(id) | Delete expense |

---

## File Structure

```
/Users/shyam/Development/ExpenseApi/
├── Program.cs                    # App entry point, DI setup
├── appsettings.json              # Connection strings, config
├── ExpenseApi.csproj             # Project file, NuGet packages
├── Controllers/
│   └── ExpensesController.cs     # API endpoints
├── Services/
│   ├── IExpenseService.cs        # Service interface
│   └── ExpenseService.cs         # Business logic, mapping
├── Repositories/
│   ├── IExpenseRepository.cs     # Repository interface
│   └── ExpenseRepository.cs      # Data access with EF Core
├── Data/
│   └── ExpenseDbContext.cs       # EF Core DbContext
├── Models/
│   └── Expense.cs                # Database entity
├── DTOs/
│   ├── CreateExpenseDto.cs       # POST request data
│   ├── UpdateExpenseDto.cs       # PUT request data
│   └── ExpenseResponseDto.cs     # API response data
└── Migrations/                   # Database migrations
```

---

## Key Design Patterns Used

1. **Layered Architecture**: Separation between Controller → Service → Repository → Data
2. **Repository Pattern**: Abstracts data access, makes testing easier
3. **Service Pattern**: Contains business logic, keeps controllers thin
4. **DTO Pattern**: Separate objects for API contracts vs database entities
5. **Dependency Injection**: Loose coupling, testable code
6. **Interface Segregation**: Small, focused interfaces (IExpenseService, IExpenseRepository)
