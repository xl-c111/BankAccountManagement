using BankAccountManagement.Models;

namespace BankAccountManagement.Tests.Models;

public class CustomerTests
{
  [Fact]
  public void AddAccount_WithSameAccountTwice_ShouldThrow()
  {
    Person person = new("Mary", "Melbourne");
    CheckingAccount account = new();
    person.AddAccount(account);

    Assert.Throws<ArgumentException>(() => person.AddAccount(account));
  }

  [Fact]
  public void RemoveAccount_WhenAccountDoesNotExist_ShouldThrow()
  {
    Person person = new("Mary", "Melbourne");
    CheckingAccount account = new();

    Assert.Throws<ArgumentException>(() => person.RemoveAccount(account));
  }

  [Fact]
  public void RemoveAccount_WithNull_ShouldThrow()
  {
    Person person = new("Mary", "Melbourne");

    Assert.Throws<ArgumentException>(() => person.RemoveAccount(null!));
  }

  [Fact]
  public void Deactivate_ShouldSetInactive()
  {
    Person person = new("Mary", "Melbourne");

    person.Deactivate();

    Assert.False(person.IsActive);
  }

  [Fact]
  public void SetNextCustomerId_ShouldAffectNextCreatedCustomer()
  {
    Person first = new("A", "B");
    long next = first.CustomerId + 700;

    Customer.SetNextCustomerId(next);
    Person second = new("C", "D");

    Assert.True(second.CustomerId >= next);
  }
}
