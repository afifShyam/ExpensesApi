# Database Design

The project uses **PostgreSQL** for production and **SQLite** (in‑memory) for unit tests.

## Schema Overview
```mermaid
classDiagram
    class Expense {
        +Guid Id
        +decimal Amount
        +string Currency
        +DateTime Date
        +string Description
        +Guid CategoryId
        +Guid UserId
    }
    class Category {
        +Guid Id
        +string Name
        +Guid UserId
    }
    class "User" {
        +Guid Id
        +string Email
        +string PasswordHash
    }
    Expense "*" --> "1" Category : belongs to
    Expense "*" --> "1" User : owned by
    Category "*" --> "1" User : owned by
```

## Entity‑Framework Core Mapping (excerpt)
```csharp
public class ExpenseEntity {
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = default!;
    public DateTime Date { get; set; }
    public string Description { get; set; } = default!;
    public Guid CategoryId { get; set; }
    public Guid UserId { get; set; }
    public CategoryEntity Category { get; set; } = default!;
    public UserEntity User { get; set; } = default!;
}
```

## Migrations
- Run `dotnet ef migrations add InitialCreate` in the **Infrastructure** project.
- Apply with `dotnet ef database update`.

---
*See `CLEAN_ARCHITECTURE_NOTES.md` for where the DAL lives.*
