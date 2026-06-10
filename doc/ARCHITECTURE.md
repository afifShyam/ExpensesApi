# Architecture Overview

This repository demonstrates **Clean Architecture** principles using **C#**. The goal is to provide a clear, maintainable, and testable codebase for an expense‑tracking API.

## Layers
- **Domain**: Core business entities (`Expense`, `User`) and interfaces (`IExpenseRepository`).
- **Application**: Use‑cases/commands (`CreateExpense`, `GetExpensesByUser`).
- **Infrastructure**: Implementations of repositories (EF Core, in‑memory) and external services.
- **Presentation**: ASP.NET Core Web API exposing HTTP endpoints.

## Dependency Rules
- Outer layers depend on inner layers via abstractions.
- No inner layer references outer implementation details.
- Interfaces live in the inner layer; implementations live in the outer layer.

## Benefits
- Testability: unit‑test use‑cases without DB.
- Flexibility: swap EF Core for any other storage.
- Clear separation of concerns.

---
*Generated on 2026‑05‑29.*
