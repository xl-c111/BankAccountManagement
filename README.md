# BankAccountManagement

A .NET 10 console-based bank account management project using EF Core and MySQL.  
The project models customers and accounts with inheritance mapping, persists data with code-first migrations, and includes unit/integration testing scaffolding.

## Tech Stack
- .NET 10
- C#
- Entity Framework Core 10
- MySql.EntityFrameworkCore 10
- MySQL
- xUnit (testing)

## Project Structure
```text
BankAccountManagement/
├── Controller/
│   └── AccountController.cs
├── Application/
│   └── DemoRunner.cs
├── Models/
│   ├── Customer.cs
│   ├── Person.cs
│   ├── Company.cs
│   ├── Account.cs
│   ├── CheckingAccount.cs
│   └── SavingsAccount.cs
├── Persistence/
│   └── BankDbContext.cs
├── Migrations/
├── Program.cs
├── BankAccountManagement.Tests/
│   ├── Models/
│   └── Integration/
└── TESTING_STRATEGY.md
```

## Domain Overview

### Customer hierarchy
- `Customer` (abstract base)
- `Person`
- `Company`

### Account hierarchy
- `Account` (abstract base)
- `CheckingAccount`
- `SavingsAccount`

### Business behavior highlights
- `Person.ChargeAllAccounts(amount)`: charges the same amount for each account.
- `Company.ChargeAllAccounts(amount)`: charges double for `SavingsAccount`, normal for others.
- `SavingsAccount.Withdraw(amount)`: returns `0` when amount is greater than balance.
- Static ID generators are currently used for `CustomerId` and `AccountId`.

## Database Design
- Provider: MySQL via EF Core.
- Mapping strategy:
  - TPH (Table-per-Hierarchy) for `Customer` using discriminator `customer_type`
  - TPH for `Account` using discriminator `account_type`
- Main tables:
  - `customers`
  - `accounts`
- Relationship:
  - `Customer (1) -> (many) Account` with cascade delete.

Connection is configured in `BankDbContext`:
- Uses `BANK_DB_CONNECTION` environment variable when set.
- Falls back to default local connection string when not set.

## Getting Started

### 1. Prerequisites
- .NET SDK 10+
- MySQL server running locally or remotely

### 2. Configure connection string (recommended)
```bash
export BANK_DB_CONNECTION="server=localhost;uid=root;pwd=MyPass123;database=bank_account_management"
```

You can copy `.env.example` to `.env` for local reference, but do not commit real secrets.

### 3. Restore and build
```bash
dotnet restore BankAccountManagement.slnx
dotnet build BankAccountManagement.slnx
```

### 4. Apply migration (if needed)
```bash
dotnet ef database update
```

### 5. Run
```bash
dotnet run
```

## Testing
Testing strategy is documented in:
- `TESTING_STRATEGY.md`

### Run all tests
```bash
dotnet test BankAccountManagement.Tests
```

### MySQL integration tests
Integration tests are scaffolded and currently marked with `Skip` by default to avoid accidental DB writes.

To enable:
1. Set a dedicated test DB:
```bash
export BANK_TEST_DB_CONNECTION="server=localhost;uid=root;pwd=MyPass123;database=bank_account_management_test"
```
2. Remove `Skip = ...` from facts in:
   - `BankAccountManagement.Tests/Integration/AccountControllerMySqlTests.cs`
3. Run only integration tests:
```bash
dotnet test BankAccountManagement.Tests --filter "Category=MySqlIntegration"
```

## Test Coverage
Generate coverage data:
```bash
dotnet test BankAccountManagement.Tests --collect:"XPlat Code Coverage"
```

Generate HTML report (requires ReportGenerator):
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
export PATH="$PATH:$HOME/.dotnet/tools"
reportgenerator -reports:"BankAccountManagement.Tests/TestResults/**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```

Open report:
```bash
open coverage-report/index.html
```

## Developer Workflow
Run local quality checks:
```bash
./scripts/check.sh
```

Clean generated artifacts (`bin/`, `obj/`, `TestResults/`, `coverage-report/`):
```bash
./scripts/clean.sh
```

## Current Implementation Notes
- `Program.cs` currently runs a demo workflow and writes test data into DB.
- `AccountController` persists immediately (`SaveChanges()` inside methods).
- Static ID generation is application-managed, not DB auto-increment.

## Recommended Next Improvements
1. Move connection string fully to configuration files and environment-based profiles.
2. Refactor demo logic from `Program.cs` into application services.
3. Add CI pipeline with:
   - PR: unit tests + lint/build
   - Nightly/main: MySQL integration tests + coverage artifact
4. Replace static ID strategy with DB-generated keys or dedicated ID service.
