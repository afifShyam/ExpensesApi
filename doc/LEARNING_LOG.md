# Learning Log

This file is a running journal for anyone studying the **ExpenseApi** codebase and Clean Architecture concepts. Keep it updated as you explore the project.

## Log Format
| Date | Topic | What I Learned | Questions / Next Steps |
|------|-------|----------------|------------------------|
| 2026‑05‑29 | Project Overview | Understanding the layer separation and folder layout. | How to add a new use‑case?
|  |  |  |  |

## Sample Entries
### 2026‑05‑29 – Clean Architecture Basics
- **Key Takeaway:** Inner layers (`Domain`, `Application`) contain no dependencies on outer layers.
- **Action:** Created an interface `IExpenseRepository` in `Application` and implemented it in `Infrastructure`.
- **Open Question:** When should I place validation – in DTOs, commands, or domain entities?

### 2026‑05‑30 – EF Core Migrations
- **Key Takeaway:** Migrations are generated from the `Infrastructure` DbContext.
- **Command Used:** `dotnet ef migrations add AddCategoryTable`.
- **Open Question:** How to handle database versioning across environments?

*Add new rows as you progress. This log is version‑controlled with the repo.*
