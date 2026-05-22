using BankAccountManagement.Models;

namespace BankAccountManagement.Tests.Models;

public class AccountModelTests
{
  [Fact]
  public void Deposit_WithPositiveAmount_IncreasesBalance()
  {
    CheckingAccount account = new();

    account.Deposit(100);

    Assert.Equal(100, account.Balance);
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-10)]
  public void Deposit_WithInvalidAmount_Throws(double amount)
  {
    CheckingAccount account = new();

    Assert.Throws<ArgumentException>(() => account.Deposit(amount));
  }

  [Fact]
  public void CheckingAccount_GetNextCheckNumber_IncrementsSequence()
  {
    CheckingAccount account = new();

    int first = account.GetNextCheckNumber();
    int second = account.GetNextCheckNumber();

    Assert.Equal(1, first);
    Assert.Equal(2, second);
  }

  [Fact]
  public void SavingsAccount_WithdrawBeyondBalance_ReturnsZeroAndKeepsBalance()
  {
    SavingsAccount account = new(2.5);
    account.Deposit(50);

    double withdrawn = account.Withdraw(80);

    Assert.Equal(0, withdrawn);
    Assert.Equal(50, account.Balance);
  }

  [Fact]
  public void SavingsAccount_AddInterest_UpdatesBalance()
  {
    SavingsAccount account = new(10);
    account.Deposit(200);

    account.AddInterest();

    Assert.Equal(220, account.Balance);
  }
}
