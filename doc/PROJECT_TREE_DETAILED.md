# Detailed Project Tree

```
ExpenseApi/
├─ .git/                     # Git repository metadata
├─ .kilo/                    # KiLo tool configuration (if any)
├─ .vscode/                  # VS Code workspace settings
├─ Application/
│   ├─ DTOs/                 # Data Transfer Objects used by the API
│   ├─ Interfaces/           # Service contracts for the Application layer
│   ├─ Services/             # Business services / use‑case implementations
│   └─ DependencyInjection.cs # Registers Application services in DI container
├─ Common/
│   ├─ Mapping/              # Object‑mapper profiles (e.g., AutoMapper)
│   ├─ ApiErrorResponse.cs   # Standard error response model
│   ├─ ApiSuccessResponse.cs # Standard success response model
│   ├─ Error.cs              # Custom error handling utilities
│   ├─ Policies.cs           # Authorization / validation policies
│   ├─ Result.cs             # Result monad / operation result wrapper
│   └─ StringExtensions.cs   # Helper extensions for strings
├─ Configuration/
│   ├─ ControllerConfiguration.cs # MVC controller settings
│   ├─ CorsConfiguration.cs       # CORS policy configuration
│   └─ RateLimitingConfiguration.cs # Rate‑limit policy configuration
├─ Controllers/
│   ├─ ApiControllerBase.cs   # Base class for all API controllers
│   ├─ CommitmentController.cs # Sample controller (perhaps for commitments)
│   └─ ExpensesController.cs # Main controller handling expense CRUD
├─ Domain/
│   └─ Entities/            # Core domain entities (Expense, Category, User, …)
├─ Infrastructure/
│   ├─ Persistence/          # EF Core DbContext and repository implementations
│   │   ├─ ExpenseDbContext.cs
│   │   └─ Repositories/
│   │       ├─ ExpenseRepository.cs
│   │       └─ CategoryRepository.cs
│   ├─ Services/            # External services (e.g., email, logging adapters)
│   └─ Migrations/          # EF Core migration files
├─ Middleware/
│   ├─ ExceptionHandlingMiddleware.cs
│   └─ RequestLoggingMiddleware.cs
├─ Migrations/               # Database migration scripts (if not under Infrastructure)
│   ├─ 20230101010101_InitialCreate.cs
│   └─ ExpenseApiDbContextModelSnapshot.cs
├─ Program.cs                # Application entry point, host building
├─ Properties/
│   └─ launchSettings.json   # Debug launch profiles for VS
├─ README.md                 # Project README
├─ appsettings.json          # Default configuration
├─ appsettings.Development.json # Development overrides
├─ appsettings copy.json     # Possibly a backup copy of settings
├─ bin/                      # Build output (auto‑generated)
├─ obj/                      # Intermediate compilation files (auto‑generated)
└─ doc/                      # Documentation folder
    ├─ ARCHITECTURE.md
    ├─ API_ENDPOINTS.md
    ├─ CLEAN_ARCHITECTURE_NOTES.md
    ├─ DATABASE.md
    ├─ LEARNING_LOG.md
    ├─ SETUP.md
    └─ COMMANDS.md
```

*The tree reflects the current layout of the repository, showing the most important files and sub‑folders for each layer.*
