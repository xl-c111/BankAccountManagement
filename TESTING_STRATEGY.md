# BankAccountManagement Testing Strategy

## Goals
- Keep business-rule tests fast and deterministic.
- Verify EF Core + MySQL behavior with real relational constraints.
- Prevent migration/schema regressions in CI.

## Test Layers
1. Unit tests (`BankAccountManagement.Tests/Models`)
- Scope: Domain logic in `Models` only.
- No database, no EF runtime dependency in assertions.
- Run on every commit/PR.

2. MySQL integration tests (`BankAccountManagement.Tests/Integration`)
- Scope: `AccountController + BankDbContext + MySQL`.
- Validate CRUD, TPH mapping, cascade delete, and relationship loading.
- Run on PR or nightly pipeline.

3. Migration smoke tests
- Apply migrations to a clean MySQL test schema.
- Fail fast if schema creation/migration breaks.

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
export BANK_TEST_DB_CONNECTION="server=localhost;uid=root;pwd=MyPass123;database=bank_account_management_test"
```
- In `Integration/AccountControllerMySqlTests.cs`, remove `Skip = ...` from `[Fact]`.
- Run:
```bash
dotnet test BankAccountManagement.Tests --filter "Category=MySqlIntegration"
```

## Important Conventions
- Never run integration tests against production/dev schema.
- Always use isolated schema name ending with `_test`.
- Keep model tests independent from static ID exact values.
- Keep package versions aligned across EF Core and MySQL provider.
