using BankAccountManagement.Models;

namespace BankAccountManagement.Tests.Models;

public class CustomerModelTests
{
  [Fact]
  public void Person_AddSameAccountTwice_Throws()
  {
    Person person = new("Mary", "Melbourne");
    CheckingAccount account = new();
    person.AddAccount(account);

    Assert.Throws<ArgumentException>(() => person.AddAccount(account));
  }

  [Fact]
  public void Person_RemoveMissingAccount_Throws()
  {
    Person person = new("Mary", "Melbourne");
    CheckingAccount account = new();

    Assert.Throws<ArgumentException>(() => person.RemoveAccount(account));
  }

  [Fact]
  public void Person_ChargeAllAccounts_ChargesEachAccount()
  {
    Person person = new("Mary", "Melbourne");
    CheckingAccount checking = new();
    SavingsAccount savings = new(2.5);
    checking.Deposit(100);
    savings.Deposit(100);
    person.AddAccount(checking);
    person.AddAccount(savings);

    person.ChargeAllAccounts(20);

    Assert.Equal(80, checking.Balance);
    Assert.Equal(80, savings.Balance);
  }

  [Fact]
  public void Company_ChargeAllAccounts_ChargesSavingsAtDoubleRate()
  {
    Company company = new("Acme", "Sydney", "ABN-1", "ACN-1");
    CheckingAccount checking = new();
    SavingsAccount savings = new(1);
    checking.Deposit(100);
    savings.Deposit(100);
    company.AddAccount(checking);
    company.AddAccount(savings);

    company.ChargeAllAccounts(10);

    Assert.Equal(90, checking.Balance);
    Assert.Equal(80, savings.Balance);
  }
}
