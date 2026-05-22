# BankAccountManagement

A .NET 10 console project for bank account management using EF Core + MySQL, with xUnit tests.

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

## Run
1. Clone and enter project:
```bash
git clone <your-repository-url>
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
