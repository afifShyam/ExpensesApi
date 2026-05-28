# ExpenseApi Architecture & Data Flow

## Overview

This is a **Layered Architecture** ASP.NET Core Web API using **Entity Framework Core** with **PostgreSQL**. The application follows a clean separation of concerns with 4 main layers.

### Language Version Update
The project now targets **C# 14** (previously C# 12). The `<LangVersion>` property in `ExpenseApi.csproj` has been set to `14`, enabling new language features such as **list patterns**, **raw string literals**, **record structs**, and **improved pattern matching**. This upgrade allows the codebase to leverage more expressive syntax while remaining fully compatible with .NET 9 runtime.

The change is isolated to the build configuration and does not affect runtime behavior, but developers should ensure any external libraries are compatible with the newer compiler version.

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
│  │  (ExpensesController.cs, CommitmentController.cs)        │   │
│  │  • Inherits from ApiControllerBase                      │   │
│  │  • Receives HTTP requests & validates input             │   │
│  │  • Uses Result Pattern to handle Service outcomes        │   │
│  │  • Maps Result<T> to IActionResult using .When()         │   │
│  └────────────────────┬─────────────────────────────────────┘   │
│                       │
│                       │ Calls Service Interface
│                       │ (Returns Result<T>)
│                       ▼
│
│  ┌──────────────────────────────────────────────────────────┐   │
│  │              SERVICE LAYER                               │   │
│  │         (ExpenseService.cs)                              │   │
│  │  • Contains business logic                               │   │
│  │  • Transforms DTOs to/from Models                        │   │
│  │  • Coordinates between Controller and Repository         │   │
│  │  • Data conversion & mapping                             │   │
│  │  • Returns Result<T> for functional error handling       │   │
│  └────────────────────┬─────────────────────────────────────┘   │
│                       │ Calls Repository Interface               │
│                       │ (IExpenseRepo, ICommitmentRepo)          │
│                       ▼                                          │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │              REPOSITORY LAYER                            │   │
│  │  (ExpenseRepository.cs, CommitmentRepository.cs)         │   │
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
│  │  • ICommitmentService → CommitmentService (Scoped)       │   │
│  │  • ICommitmentRepository → CommitmentRepository (Scoped) │   │
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
│  • Calls _expenseService.CreateAsync(dto)               │
│  • Maps Result outcome via .When() to CreatedAtAction   │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│  ExpenseService.CreateAsync(CreateExpenseDto dto)       │
│  • Business logic check (Amount > 0, Category exists)   │
│  • Returns Result.Failure if validation fails           │
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

See `/Users/shyam/Development/ExpenseApi/Program.cs`

```csharp
// Scoped = New instance per HTTP request
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();

builder.Services.AddScoped<ICommitmentRepository, CommitmentRepository>();
builder.Services.AddScoped<ICommitmentService, CommitmentService>();

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

### Expenses
| Method | Endpoint | Action | Description |
|--------|----------|--------|-------------|
| GET | `/api/expenses` | GetAll() | List all expenses, newest first |
| GET | `/api/expenses/{id}` | GetById(id) | Get single expense by ID |
| POST | `/api/expenses` | Create(dto) | Create new expense |
| PUT | `/api/expenses/{id}` | Update(id, dto) | Update existing expense |
| DELETE | `/api/expenses/{id}` | Delete(id) | Delete expense |

### Commitments
| Method | Endpoint | Action | Description |
|--------|----------|--------|-------------|
| GET | `/api/Commitment` | GetAll() | List all recurring commitments |
| GET | `/api/Commitment/{id}` | GetById(id) | Get single commitment by ID |
| POST | `/api/Commitment` | Create(dto) | Create new commitment |
| PUT | `/api/Commitment/{id}` | Update(id, dto) | Update existing commitment |
| DELETE | `/api/Commitment/{id}` | Delete(id) | Delete commitment |
| DELETE | `/api/Commitment` | DeleteAll() | Bulk delete all commitments |

---

## File Structure

```
/Users/shyam/Development/ExpenseApi/
├── ARCHITECTURE.md
├── README.md
├── ExpenseApi.slnx
├── ExpenseApi.csproj
├── Program.cs                    # App entry point, DI setup
├── appsettings.json              # Connection strings, config
├── appsettings.Development.json
├── Controllers/
│   ├── ApiControllerBase.cs      # Base controller with Result logic
│   ├── CommitmentController.cs   # Commitment API endpoints
│   └── ExpensesController.cs     # Expense API endpoints
├── Common/
│   ├── ApiErrorResponse.cs
│   ├── ApiSuccessResponse.cs
│   ├── Error.cs
│   ├── Policies.cs
│   ├── Result.cs
│   └── StringExtensions.cs
├── Configuration/
│   ├── ControllerConfiguration.cs
│   ├── CorsConfiguration.cs
│   └── RateLimitingConfiguration.cs
├── Domain/
│   ├── Common/
│   │   ├── Error.cs
│   │   └── Result.cs
│   └── Entities/
│       ├── Category.cs
│       ├── Commitment.cs
│       ├── CommitmentPayment.cs
│       ├── Expense.cs
│       └── MonthlyIncome.cs
├── Application/
│   ├── DTOs/
│   │   ├── Commitment/
│   │   │   ├── CommitmentResponseDto.cs
│   │   │   ├── CreateCommitmentDto.cs
│   │   │   └── UpdateCommitmentDto.cs
│   │   └── Expenses/
│   │       ├── CreateExpenseDto.cs
│   │       ├── ExpenseResponseDto.cs
│   │       └── UpdateExpenseDto.cs
│   ├── Interfaces/
│   │   ├── ICommitmentRepository.cs
│   │   └── IExpenseRepository.cs
│   ├── Services/
│   │   ├── Commitments/
│   │   │   ├── CommitmentService.cs
│   │   │   └── ICommitmentService.cs
│   │   └── Expenses/
│   │       ├── ExpenseService.cs
│   │       └── IExpenseService.cs
│   └── DependencyInjection.cs
├── Infrastructure/
│   ├── Data/
│   │   └── ExpenseDbContext.cs
│   ├── RateLimiting/
│   │   └── RateLimitingExtension.cs
│   ├── Repositories/
│   │   ├── Commitments/
│   │   │   └── CommitmentRepository.cs
│   │   └── Expenses/
│   │       └── ExpenseRepository.cs
│   └── DependencyInjection.cs
├── Middleware/
│   └── GlobalExceptionMiddleware.cs
├── Migrations/
│   ├── 20260424015554_InitialCreate.cs
│   ├── 20260424015554_InitialCreate.Designer.cs
│   ├── 20260518123232_AddBudgetTables.cs
│   ├── 20260518123232_AddBudgetTables.Designer.cs
│   └── ExpenseDbContextModelSnapshot.cs
├── Properties/
│   └── launchSettings.json
├── bin/
│   └── Debug/
│       └── net10.0/
├── obj/
│   └── Debug/
│       └── net10.0/
```


---

## Key Design Patterns Used

1. **Layered Architecture**: Separation between Controller → Service → Repository → Data
2. **Repository Pattern**: Abstracts data access, makes testing easier
3. **Service Pattern**: Contains business logic, keeps controllers thin
4. **DTO Pattern**: Separate objects for API contracts vs database entities
5. **Dependency Injection**: Loose coupling, testable code
6. **Interface Segregation**: Small, focused interfaces (IExpenseService, IExpenseRepository)
