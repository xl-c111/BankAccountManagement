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

2. Create two local MySQL databases (one for app data, one for tests):
```sql
CREATE DATABASE IF NOT EXISTS bank_account_management;
CREATE DATABASE IF NOT EXISTS bank_account_management_test;
```

3. Configure local `.env` in the project root (required):
```env
BANK_DB_CONNECTION="server=localhost;uid=<db_user>;pwd=<db_password>;database=bank_account_management"
BANK_TEST_DB_CONNECTION="server=localhost;uid=<db_user>;pwd=<db_password>;database=bank_account_management_test"
```

You can also use shell environment variables instead of `.env`. Do not commit real secrets.

4. Build and migrate:
```bash
dotnet restore BankAccountManagement.slnx
dotnet build BankAccountManagement.slnx
dotnet ef database update
```

5. Start demo run:
```bash
dotnet run
```

## Test
Run all tests:
```bash
dotnet test BankAccountManagement.Tests
```

Run MySQL integration tests only:
```bash
dotnet test BankAccountManagement.Tests --filter "Category=MySqlIntegration"
```

Current MySQL integration coverage includes:
- CRUD and update persistence through `AccountController`.
- TPH discriminator behavior for `Person` and `Company`.
- Relationship loading (`GetCustomers` with accounts).
- Deletion behaviors (remove account vs remove customer with related accounts).
- Database constraint exception paths (`DbUpdateException`) for oversized fields.

MySQL integration tests are grouped into the `MySqlIntegration` collection and run serially to avoid concurrent schema create/drop conflicts on the shared test database.
The test database may be dropped/cleaned by the integration test lifecycle, so it can appear empty after test runs.

## Coverage
Generate and open coverage report (clean run):
Requires `BANK_TEST_DB_CONNECTION` to be set (via environment variable or local `.env`).
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
export PATH="$PATH:$HOME/.dotnet/tools"
rm -rf BankAccountManagement.Tests/TestResults coverage-report
dotnet test BankAccountManagement.Tests --collect:"XPlat Code Coverage" --settings coverage.runsettings
reportgenerator -reports:"BankAccountManagement.Tests/TestResults/*/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
open coverage-report/index.html
```

Coverage uses `coverage.runsettings`, which excludes EF migration files from the coverage calculation.

## Verify Data In MySQL
To inspect data created by the app in the main database:
```sql
USE bank_account_management;
SELECT * FROM customers;
SELECT * FROM accounts;
```
