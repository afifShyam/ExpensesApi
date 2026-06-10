# Clean Architecture Notes

This document expands on how the **ExpenseApi** project maps Clean Architecture concepts to concrete C# code.

## 1. Dependency Rule in Practice
- **Domain (Core)** projects reference **only .NET BCL** and **no third‑party libraries** except for pure validation helpers that do not pull infrastructure concerns.
- **Application** references the **Domain** project for entity types and defines **abstractions** (e.g., `IExpenseRepository`, `ITimeProvider`).
- **Infrastructure** references **Application** (to implement the abstractions) and **Domain** (for entity models). It also references EF Core, Dapper, or any persistence library.
- **Presentation** (Web API) references **Application** to call use‑cases and **Domain** for DTOs, but never directly references repositories.

## 2. Project Structure
```
ExpenseApi.sln
├─ src/
│   ├─ Core/                # Domain entities, value objects, domain services
│   │   ├─ Entities/
│   │   ├─ ValueObjects/
│   │   └─ Interfaces/       # e.g., IExpenseRepository (port)
│   ├─ Application/         # Use‑cases (interactors), DTOs, validation
│   │   ├─ UseCases/
│   │   ├─ DTOs/
│   │   └─ Ports/            # Interfaces needed by use‑cases (e.g., ITimeProvider)
│   ├─ Infrastructure/      # EF Core DbContext, Repository implementations
│   │   ├─ Persistence/
│   │   └─ Services/
│   └─ Presentation/        # ASP.NET Core Web API
│       ├─ Controllers/
│       └─ Startup/          # DI registration, middleware
└─ tests/                    # Parallel test projects per layer
```

## 3. Key Patterns
| Pattern | Where Used | Reason |
|---------|------------|--------|
| Repository | Infrastructure → implements `IExpenseRepository` defined in Application | Decouples data access from business logic |
| CQRS (optional) | Application layer – separate `Command` and `Query` use‑cases | Improves readability and scalability |
| Mediator (MediatR) | Presentation → forwards requests to Application handlers | Reduces controller boilerplate |
| Specification | Application / Domain – filter expenses by date, category, amount | Encapsulates query logic |
| Domain Events | Domain → raised by entities, handled in Infrastructure | Enables side‑effects without coupling |

## 4. Example Flow (Create Expense)
1. **Controller** receives HTTP POST `/api/expenses` with `CreateExpenseCommand` DTO.
2. Controller injects `IMediator` (or directly the use‑case) and sends the command.
3. **Use‑Case** (`CreateExpenseUseCase`) validates input, constructs a domain `Expense` entity, and calls `IExpenseRepository.AddAsync`.
4. **Repository Implementation** (EF Core) persists the entity.
5. Upon success, the use‑case may raise a **Domain Event** (`ExpenseCreated`).
6. An **Event Handler** in Infrastructure sends a notification email (or writes a log).
7. Controller returns `201 Created`.

## 5. Testing Guidelines
- **Unit Tests** – target `Core` and `Application` only, mocking repository interfaces.
- **Integration Tests** – spin up an in‑memory SQLite DB using the `Infrastructure` project.
- **API Tests** – use `WebApplicationFactory<T>` to test the whole request pipeline.

## 6. Frequently Asked Questions
- **Where should validation live?**
  - Basic DTO validation (required fields, ranges) via FluentValidation in the Application layer.
  - Business invariants (e.g., expense amount > 0) belong in the Domain entity constructors or methods.
- **Can I reference EF Core in the Domain?**
  - No. Keep Domain free of EF Core attributes. Use separate mapping configurations in Infrastructure.
- **How to add a new feature?**
  1. Define a new use‑case in `Application/UseCases`.
  2. Add any required repository contracts to `Application/Ports`.
  3. Implement the contracts in `Infrastructure`.
  4. Expose an API endpoint in `Presentation/Controllers` that delegates to the use‑case.

---
*Keep this note updated as the architecture evolves.*
