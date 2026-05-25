using BankAccountManagement.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankAccountManagement.Tests.Persistence;

public class BankDbContextTests
{
  private static readonly object EnvLock = new();

  private sealed class TestableBankDbContext : BankDbContext
  {
    public void InvokeOnConfiguring(DbContextOptionsBuilder builder)
    {
      base.OnConfiguring(builder);
    }
  }

  private sealed class EnvAndDotEnvScope : IDisposable
  {
    private readonly string? _originalEnvValue;
    private readonly string _envFilePath;
    private readonly bool _envFileExisted;
    private readonly string? _envFileContent;

    public EnvAndDotEnvScope(string? envValue, string? dotEnvContent)
    {
      _originalEnvValue = Environment.GetEnvironmentVariable("BANK_DB_CONNECTION");
      Environment.SetEnvironmentVariable("BANK_DB_CONNECTION", envValue);

      _envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
      _envFileExisted = File.Exists(_envFilePath);
      _envFileContent = _envFileExisted ? File.ReadAllText(_envFilePath) : null;

      if (dotEnvContent == null)
      {
        if (_envFileExisted)
        {
          File.Delete(_envFilePath);
        }
      }
      else
      {
        File.WriteAllText(_envFilePath, dotEnvContent);
      }
    }

    public void Dispose()
    {
      Environment.SetEnvironmentVariable("BANK_DB_CONNECTION", _originalEnvValue);

      if (_envFileExisted)
      {
        File.WriteAllText(_envFilePath, _envFileContent ?? string.Empty);
      }
      else if (File.Exists(_envFilePath))
      {
        File.Delete(_envFilePath);
      }
    }
  }

  [Fact]
  public void Constructor_WithOptions_ShouldUseProvidedOptions()
  {
    DbContextOptions<BankDbContext> options = new DbContextOptionsBuilder<BankDbContext>()
      .UseInMemoryDatabase(Guid.NewGuid().ToString())
      .Options;

    using BankDbContext context = new(options);

    Assert.NotNull(context.Customers);
    Assert.NotNull(context.Accounts);
    Assert.NotNull(context.Persons);
    Assert.NotNull(context.Companies);
    Assert.NotNull(context.CheckingAccounts);
    Assert.NotNull(context.SavingsAccounts);
  }

  [Fact]
  public void OnConfiguring_WhenOptionsAlreadyConfigured_ShouldNotRequireConnectionString()
  {
    lock (EnvLock)
    {
      using EnvAndDotEnvScope _ = new(envValue: null, dotEnvContent: null);
      TestableBankDbContext context = new();
      DbContextOptionsBuilder<BankDbContext> builder = new DbContextOptionsBuilder<BankDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString());

      context.InvokeOnConfiguring(builder);

      Assert.True(builder.IsConfigured);
    }
  }

  [Fact(Skip = "This test requires temporarily hiding the local .env file.")]
  public void OnConfiguring_WhenEnvAndDotEnvMissing_ShouldThrow()
  {
    lock (EnvLock)
    {
      using EnvAndDotEnvScope _ = new(envValue: null, dotEnvContent: null);
      TestableBankDbContext context = new();
      DbContextOptionsBuilder<BankDbContext> builder = new();

      InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => context.InvokeOnConfiguring(builder));
      Assert.Contains("Missing BANK_DB_CONNECTION", ex.Message);
    }
  }

  [Fact]
  public void OnConfiguring_WhenEnvironmentVariableExists_ShouldConfigureProvider()
  {
    lock (EnvLock)
    {
      using EnvAndDotEnvScope _ = new(
        envValue: "server=localhost;uid=user;pwd=pwd;database=bank_account_management",
        dotEnvContent: null);
      TestableBankDbContext context = new();
      DbContextOptionsBuilder<BankDbContext> builder = new();

      context.InvokeOnConfiguring(builder);

      Assert.True(builder.IsConfigured);
    }
  }

  [Fact]
  public void OnConfiguring_WhenOnlyDotEnvExists_ShouldConfigureProvider()
  {
    lock (EnvLock)
    {
      using EnvAndDotEnvScope _ = new(
        envValue: null,
        dotEnvContent: "BANK_DB_CONNECTION=\"server=localhost;uid=user;pwd=pwd;database=bank_account_management\"");
      TestableBankDbContext context = new();
      DbContextOptionsBuilder<BankDbContext> builder = new();

      context.InvokeOnConfiguring(builder);

      Assert.True(builder.IsConfigured);
    }
  }
}
