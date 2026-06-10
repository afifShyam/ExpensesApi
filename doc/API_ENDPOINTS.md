# API Endpoints

The Expense API follows REST principles and uses JSON payloads. All endpoints are secured with JWT Bearer authentication.

| HTTP Method | Route | Use‑Case | Request DTO | Response |
|-------------|-------|----------|-------------|----------|
| `GET` | `/api/expenses` | Get list of expenses for the current user | `GetExpensesQuery` (optional pagination, filtering) | `ExpenseDto[]` |
| `GET` | `/api/expenses/{id}` | Retrieve a single expense | `id` (GUID path parameter) | `ExpenseDto` |
| `POST` | `/api/expenses` | Create a new expense | `CreateExpenseCommand` | `201 Created` (Location header pointing to `/api/expenses/{newId}`) |
| `PUT` | `/api/expenses/{id}` | Update an existing expense | `UpdateExpenseCommand` | `204 No Content` |
| `DELETE` | `/api/expenses/{id}` | Delete an expense | `id` (GUID) | `204 No Content` |
| `GET` | `/api/categories` | List expense categories | – | `CategoryDto[]` |
| `POST` | `/api/categories` | Create a new category | `CreateCategoryCommand` | `201 Created` |
| `PUT` | `/api/categories/{id}` | Update a category | `UpdateCategoryCommand` | `204 No Content` |
| `DELETE` | `/api/categories/{id}` | Delete a category | `id` (GUID) | `204 No Content` |

## Common Request / Response Shapes

```csharp
// Example DTOs (placed in Application layer)
public record ExpenseDto(Guid Id, decimal Amount, string Currency, DateTime Date, string Description, Guid CategoryId);
public record CreateExpenseCommand(decimal Amount, string Currency, DateTime Date, string Description, Guid CategoryId);
public record UpdateExpenseCommand(decimal Amount, string Currency, DateTime Date, string Description, Guid CategoryId);
```

All error responses follow the RFC 7807 "Problem Details" format.

---
*Refer to `CLEAN_ARCHITECTURE_NOTES.md` for how these endpoints map to use‑cases and inner layers.*
