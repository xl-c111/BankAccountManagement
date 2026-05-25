# BankAccountManagement Testing Strategy

## Goals
- Keep business-rule tests fast and deterministic.
- Verify EF Core + MySQL behavior with real relational constraints.
- Prevent migration/schema regressions in CI.

## Test Layers
1. Unit tests (`BankAccountManagement.Tests/Models`, `Controller`, `Application`, `Persistence`)
- Scope: domain logic, controller behavior, demo workflow, and local configuration helpers.
- Fast tests use EF Core InMemory where persistence behavior is needed without MySQL.
- Recommended to run before each commit/PR.

2. MySQL integration tests (`BankAccountManagement.Tests/Controller` + `BankAccountManagement.Tests/Persistence`)
- Scope: `AccountController + BankDbContext + MySQL`.
- Validate CRUD, update persistence, TPH mapping, cascade delete, relationship loading, and database constraint exceptions (`DbUpdateException` paths).
- Recommended to run before PR submission (or any release cut).

3. Migration smoke tests
- Apply migrations to a clean MySQL test schema.
- Fail fast if schema creation/migration breaks.

4. Coverage
- Coverage uses `coverage.runsettings`.
- The coverage report includes project code and excludes EF migration files only.
- Use Risk Hotspots as a prioritization signal: methods with high cyclomatic complexity should be covered with branch-focused tests first, then simplified through small refactors.

## Framework and Tooling
- Single framework: `xUnit` for all tests.
- Keep one test project: `BankAccountManagement.Tests`.
- Use traits for filtering integration tests:
  - `Category=MySqlIntegration`
- MySQL integration tests use `[Collection("MySqlIntegration")]` with `DisableParallelization = true` to avoid schema create/drop races on the shared test database.

## Running Tests
1. Full test suite:
```bash
dotnet test BankAccountManagement.Tests
```

2. Run only MySQL integration tests:
- Set test database connection:
```bash
export BANK_TEST_DB_CONNECTION="server=localhost;uid=<db_user>;pwd=<db_password>;database=bank_account_management_test"
```
- Run:
```bash
dotnet test BankAccountManagement.Tests --filter "Category=MySqlIntegration"
```

## Important Conventions
- Never run integration tests against production/dev schema.
- Always use isolated schema name ending with `_test`.
- Keep model tests independent from static ID exact values.
- Keep package versions aligned across EF Core and MySQL provider.
