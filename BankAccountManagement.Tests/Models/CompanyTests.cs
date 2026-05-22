using BankAccountManagement.Models;

namespace BankAccountManagement.Tests.Models;

public class CompanyTests
{
  [Fact]
  public void ChargeAllAccounts_ShouldChargeSavingsAtDoubleRate()
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

  [Fact]
  public void ChargeAllAccounts_WithInvalidAmount_ShouldThrow()
  {
    Company company = new("Acme", "Sydney", "ABN", "ACN");

    Assert.Throws<ArgumentException>(() => company.ChargeAllAccounts(-1));
  }

  [Fact]
  public void Constructor_WithInvalidAbnOrAcn_ShouldThrow()
  {
    Assert.Throws<ArgumentException>(() => new Company("Acme", "Sydney", "", "ACN"));
    Assert.Throws<ArgumentException>(() => new Company("Acme", "Sydney", "ABN", ""));
  }

  [Fact]
  public void ParameterlessConstructor_ShouldCreateObject()
  {
    Company company = new();

    Assert.NotNull(company);
  }
}
