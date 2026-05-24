# BankAccountManagement

A .NET 10 console project for bank account management using EF Core + MySQL, with xUnit tests.

## Features
- Create and remove customers.
- Support `Person` and `Company` customer types.
- Create and remove checking and savings accounts.
- Auto-generate customer IDs from `2_000_000`, incrementing by `7`.
- Auto-generate account IDs from `1_000`, incrementing by `5`.
- Deposit, withdraw, correct account balance, add savings interest, and issue check numbers.
- Persist customers and accounts to MySQL using EF Core migrations.

## Stack
- .NET 10
- C#
- EF Core 10 (`MySql.EntityFrameworkCore`)
- MySQL
- xUnit

## Project Layout
```text
BankAccountManagement/
├── Application/
├── Controller/
├── Models/
├── Persistence/
├── Migrations/
├── BankAccountManagement.Tests/
├── Program.cs
├── BankAccountManagement.csproj
└── BankAccountManagement.slnx
```

## Database
- Database provider: MySQL.
- EF Core mapping uses inheritance discriminators for customers and accounts.
- Migrations are stored in `Migrations/`.
- Connection strings are read from environment variables or local `.env`.
- Real secrets should stay in `.env` and must not be committed.

## Run
1. Clone and enter project:
```bash
git clone <https://github.com/xl-c111/BankAccountManagement.git>
cd BankAccountManagement
```

2. Configure DB connection (required):
```bash
export BANK_DB_CONNECTION="server=localhost;uid=<db_user>;pwd=<db_password>;database=bank_account_management"
```

You can also store this value in local `.env` (copied from `.env.example`). Do not commit real secrets.

3. Build and migrate:
```bash
dotnet restore BankAccountManagement.slnx
dotnet build BankAccountManagement.slnx
dotnet ef database update
```

4. Start app:
```bash
dotnet run
```

## Test
Run all tests:
```bash
dotnet test BankAccountManagement.Tests
```

MySQL integration tests are currently scaffolded with `Skip` by default.

To run MySQL database tests, set a separate test database connection:
```bash
export BANK_TEST_DB_CONNECTION="server=localhost;uid=<db_user>;pwd=<db_password>;database=bank_account_management_test"
```

This value can also be stored in local `.env`.

Then remove `Skip = ...` from the MySQL tests in `BankAccountManagement.Tests/Controller/AccountControllerTests.cs`.

## Coverage
Generate and open coverage report (clean run):
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
export PATH="$PATH:$HOME/.dotnet/tools"
rm -rf BankAccountManagement.Tests/TestResults coverage-report
dotnet test BankAccountManagement.Tests --collect:"XPlat Code Coverage" --settings coverage.runsettings
reportgenerator -reports:"BankAccountManagement.Tests/TestResults/*/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
open coverage-report/index.html
```

Coverage uses `coverage.runsettings`, which excludes EF migration files from the coverage calculation.
