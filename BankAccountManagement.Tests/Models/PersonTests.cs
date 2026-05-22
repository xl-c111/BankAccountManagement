using BankAccountManagement.Models;
using System.Reflection;

namespace BankAccountManagement.Tests.Models;

public class PersonTests
{
  [Fact]
  public void ChargeAllAccounts_ShouldChargeEachAccountBySameAmount()
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
  public void ChargeAllAccounts_WithInvalidAmount_ShouldThrow()
  {
    Person person = new("Mary", "Melbourne");

    Assert.Throws<ArgumentException>(() => person.ChargeAllAccounts(0));
  }

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void Constructor_WithInvalidName_ShouldThrow(string name)
  {
    Assert.Throws<ArgumentException>(() => new Person(name, "Melbourne"));
  }

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void Constructor_WithInvalidAddress_ShouldThrow(string address)
  {
    Assert.Throws<ArgumentException>(() => new Person("Mary", address));
  }

  [Fact]
  public void ParameterlessConstructor_ShouldBeProtected()
  {
    ConstructorInfo? publicConstructor = typeof(Person).GetConstructor(Type.EmptyTypes);
    ConstructorInfo? protectedConstructor = typeof(Person).GetConstructor(
      BindingFlags.Instance | BindingFlags.NonPublic,
      binder: null,
      types: Type.EmptyTypes,
      modifiers: null);

    Assert.Null(publicConstructor);
    Assert.NotNull(protectedConstructor);
    Assert.True(protectedConstructor.IsFamily);
  }
}
