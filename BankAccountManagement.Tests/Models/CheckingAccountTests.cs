using BankAccountManagement.Models;

namespace BankAccountManagement.Tests.Models;

public class CheckingAccountTests
{
  [Fact]
  public void GetNextCheckNumber_ShouldIncrementByOne()
  {
    CheckingAccount account = new();

    int first = account.GetNextCheckNumber();
    int second = account.GetNextCheckNumber();

    Assert.Equal(1, first);
    Assert.Equal(2, second);
    Assert.Equal(3, account.NextCheckNumber);
  }
}
