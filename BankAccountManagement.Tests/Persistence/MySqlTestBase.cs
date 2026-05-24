using BankAccountManagement.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankAccountManagement.Tests.Persistence;

public abstract class MySqlTestBase : IDisposable
{
  protected readonly BankDbContext Context;

  protected MySqlTestBase()
  {
    string? connectionString =
      Environment.GetEnvironmentVariable("BANK_TEST_DB_CONNECTION")
      ?? EnvFileLoader.GetValue("BANK_TEST_DB_CONNECTION");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
      throw new InvalidOperationException(
        "Missing BANK_TEST_DB_CONNECTION. Set an environment variable or add it to .env.");
    }

    DbContextOptions<BankDbContext> options = new DbContextOptionsBuilder<BankDbContext>()
      .UseMySQL(connectionString)
      .Options;

    Context = new BankDbContext(options);
    Context.Database.EnsureDeleted();
    Context.Database.Migrate();
  }

  public void Dispose()
  {
    Context.Database.EnsureDeleted();
    Context.Dispose();
  }
}
