using BankAccountManagement.Models;

namespace BankAccountManagement.Tests.Models;

public class AccountTests
{
  [Fact]
  public void Deposit_ShouldIncreaseBalance()
  {
    CheckingAccount account = new();

    account.Deposit(100);

    Assert.Equal(100, account.Balance);
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-10)]
  public void Deposit_WithInvalidAmount_ShouldThrow(double amount)
  {
    CheckingAccount account = new();

    Assert.Throws<ArgumentException>(() => account.Deposit(amount));
  }

  [Fact]
  public void Withdraw_ShouldDecreaseBalance()
  {
    CheckingAccount account = new();
    account.Deposit(200);

    double withdrawn = account.Withdraw(50);

    Assert.Equal(50, withdrawn);
    Assert.Equal(150, account.Balance);
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public void Withdraw_WithInvalidAmount_ShouldThrow(double amount)
  {
    CheckingAccount account = new();

    Assert.Throws<ArgumentException>(() => account.Withdraw(amount));
  }

  [Fact]
  public void CorrectBalance_ShouldSetBalance()
  {
    CheckingAccount account = new();

    account.CorrectBalance(123.45);

    Assert.Equal(123.45, account.Balance);
  }

  [Fact]
  public void Deactivate_ShouldSetInactive()
  {
    CheckingAccount account = new();

    account.Deactivate();

    Assert.False(account.IsActive);
  }

  [Fact]
  public void SetNextAccountId_ShouldAffectNextCreatedAccount()
  {
    CheckingAccount first = new();
    long next = first.AccountId + 500;

    Account.SetNextAccountId(next);
    CheckingAccount second = new();

    Assert.True(second.AccountId >= next);
  }
}
