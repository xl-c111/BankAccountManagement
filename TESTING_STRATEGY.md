# BankAccountManagement Testing Strategy

## Goals
- Keep business-rule tests fast and deterministic.
- Verify EF Core + MySQL behavior with real relational constraints.
- Prevent migration/schema regressions in CI.

## Test Layers
1. Unit tests (`BankAccountManagement.Tests/Models`, `Controller`, `Application`, `Persistence`)
- Scope: domain logic, controller behavior, demo workflow, and local configuration helpers.
- Fast tests use EF Core InMemory where persistence behavior is needed without MySQL.
- Run on every commit/PR.

2. MySQL integration tests (`BankAccountManagement.Tests/Controller` + `BankAccountManagement.Tests/Persistence`)
- Scope: `AccountController + BankDbContext + MySQL`.
- Validate CRUD, TPH mapping, cascade delete, and relationship loading.
- Run on PR or nightly pipeline.

3. Migration smoke tests
- Apply migrations to a clean MySQL test schema.
- Fail fast if schema creation/migration breaks.

4. Coverage
- Coverage uses `coverage.runsettings`.
- The coverage report includes project code and excludes EF migration files only.

## Framework and Tooling
- Single framework: `xUnit` for all tests.
- Keep one test project: `BankAccountManagement.Tests`.
- Use traits for filtering integration tests:
  - `Category=MySqlIntegration`

## Running Tests
1. Unit tests + skipped integration templates:
```bash
dotnet test BankAccountManagement.Tests
```

2. Enable MySQL integration tests:
- Set test database connection:
```bash
export BANK_TEST_DB_CONNECTION="server=localhost;uid=<db_user>;pwd=<db_password>;database=bank_account_management_test"
```
- In `Controller/AccountControllerTests.cs`, remove `Skip = ...` from `[Fact]`.
- Run:
```bash
dotnet test BankAccountManagement.Tests --filter "Category=MySqlIntegration"
```

## Important Conventions
- Never run integration tests against production/dev schema.
- Always use isolated schema name ending with `_test`.
- Keep model tests independent from static ID exact values.
- Keep package versions aligned across EF Core and MySQL provider.
