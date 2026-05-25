using BankAccountManagement.Controller;
using BankAccountManagement.Models;
using BankAccountManagement.Tests.Persistence;

namespace BankAccountManagement.Tests.Controller.Persistence;

public class AccountControllerTests : MySqlTestBase
{
  private readonly AccountController _controller;

  public AccountControllerTests()
  {
    _controller = new AccountController(Context);
  }

  [Fact]
  [Trait("Category", "MySqlIntegration")]
  public void DatabaseShouldStartEmpty()
  {
    Assert.Empty(_controller.GetCustomers());
    Assert.Empty(_controller.GetAccounts());
  }

  [Fact]
  [Trait("Category", "MySqlIntegration")]
  public void AddAndRemoveCustomerShouldPersistInMySql()
  {
    Customer customer = _controller.CreateCustomer("Jane D", "Melbourne", "person");
    _controller.CreateAccount(customer, "checking");

    int customerCountAfterCreate = Context.Customers.Count();
    int accountCountAfterCreate = Context.Accounts.Count();

    _controller.RemoveCustomer(customer);

    int customerCountAfterDelete = Context.Customers.Count();
    int accountCountAfterDelete = Context.Accounts.Count();

    Assert.Equal(1, customerCountAfterCreate);
    Assert.Equal(1, accountCountAfterCreate);
    Assert.Equal(0, customerCountAfterDelete);
    Assert.Equal(0, accountCountAfterDelete);
  }

  [Fact]
  [Trait("Category", "MySqlIntegration")]
  public void TphDiscriminatorShouldWorkForPersonAndCompany()
  {
    _controller.CreateCustomer("Mary", "Sydney", "person");
    _controller.CreateCustomer("ACME", "Brisbane", "company", "ABN-123", "ACN-123");

    int personCount = Context.Persons.Count();
    int companyCount = Context.Companies.Count();
    int totalCount = Context.Customers.Count();

    Assert.Equal(1, personCount);
    Assert.Equal(1, companyCount);
    Assert.Equal(2, totalCount);
  }
}
