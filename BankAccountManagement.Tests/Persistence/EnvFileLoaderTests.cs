using BankAccountManagement.Persistence;

namespace BankAccountManagement.Tests.Persistence;

public class EnvFileLoaderTests : IDisposable
{
  private readonly string _envFilePath;

  public EnvFileLoaderTests()
  {
    _envFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.env");
  }

  [Fact]
  public void GetValue_WhenFileDoesNotExist_ShouldReturnNull()
  {
    string? value = EnvFileLoader.GetValue("BANK_DB_CONNECTION", _envFilePath);

    Assert.Null(value);
  }

  [Fact]
  public void GetValue_WhenKeyExists_ShouldReturnValue()
  {
    File.WriteAllLines(_envFilePath, new[]
    {
      "# comment",
      "",
      "OTHER=value",
      "BANK_DB_CONNECTION=server=localhost;uid=test;pwd=test;database=test"
    });

    string? value = EnvFileLoader.GetValue("BANK_DB_CONNECTION", _envFilePath);

    Assert.Equal("server=localhost;uid=test;pwd=test;database=test", value);
  }

  [Fact]
  public void GetValue_WhenValueIsQuoted_ShouldRemoveQuotes()
  {
    File.WriteAllText(_envFilePath, "BANK_DB_CONNECTION=\"quoted-value\"");

    string? value = EnvFileLoader.GetValue("BANK_DB_CONNECTION", _envFilePath);

    Assert.Equal("quoted-value", value);
  }

  [Fact]
  public void GetValue_WhenKeyDoesNotExist_ShouldReturnNull()
  {
    File.WriteAllText(_envFilePath, "OTHER=value");

    string? value = EnvFileLoader.GetValue("BANK_DB_CONNECTION", _envFilePath);

    Assert.Null(value);
  }

  public void Dispose()
  {
    if (File.Exists(_envFilePath))
    {
      File.Delete(_envFilePath);
    }
  }
}
