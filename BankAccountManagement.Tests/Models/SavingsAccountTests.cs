using BankAccountManagement.Models;

namespace BankAccountManagement.Tests.Models;

public class SavingsAccountTests
{
  [Fact]
  public void AddInterest_ShouldIncreaseBalance()
  {
    SavingsAccount account = new(10);
    account.Deposit(200);

    account.AddInterest();

    Assert.Equal(220, account.Balance);
  }

  [Fact]
  public void Withdraw_WhenAmountGreaterThanBalance_ShouldReturnZero()
  {
    SavingsAccount account = new(2.5);
    account.Deposit(50);

    double withdrawn = account.Withdraw(80);

    Assert.Equal(0, withdrawn);
    Assert.Equal(50, account.Balance);
  }

  [Fact]
  public void Constructor_WithNegativeInterestRate_ShouldThrow()
  {
    Assert.Throws<ArgumentException>(() => new SavingsAccount(-0.1));
  }
}
