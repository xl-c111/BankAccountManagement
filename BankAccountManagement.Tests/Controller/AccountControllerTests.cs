using BankAccountManagement.Controller;
using BankAccountManagement.Models;
using BankAccountManagement.Tests.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankAccountManagement.Tests.Controller.Persistence;

[Collection("MySqlIntegration")]
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

  [Fact]
  [Trait("Category", "MySqlIntegration")]
  public void UpdateCustomerShouldPersistFieldsInMySql()
  {
    Customer customer = _controller.CreateCustomer("Jane", "Melbourne", "person");

    _controller.UpdateCustomer(
      customer,
      name: "Jane Smith",
      address: "Sydney",
      phoneNumber: "0400000000",
      email: "jane@example.com",
      dateOfBirth: new DateTime(1992, 3, 4),
      occupation: "Engineer");

    Person saved = Assert.IsType<Person>(Context.Customers.Single());
    Assert.Equal("Jane Smith", saved.Name);
    Assert.Equal("Sydney", saved.Address);
    Assert.Equal("0400000000", saved.PhoneNumber);
    Assert.Equal("jane@example.com", saved.Email);
    Assert.Equal(new DateTime(1992, 3, 4), saved.DateOfBirth);
    Assert.Equal("Engineer", saved.Occupation);
  }

  [Fact]
  [Trait("Category", "MySqlIntegration")]
  public void GetCustomersShouldLoadAccountsFromMySql()
  {
    Customer customer = _controller.CreateCustomer("Mary", "Perth", "person");
    _controller.CreateAccount(customer, "checking");
    _controller.CreateAccount(customer, "savings");

    List<Customer> customersFromDb = _controller.GetCustomers();

    Assert.Single(customersFromDb);
    Assert.Equal(2, customersFromDb[0].Accounts.Count);
  }

  [Fact]
  [Trait("Category", "MySqlIntegration")]
  public void RemoveAccountShouldNotDeleteCustomer()
  {
    Customer customer = _controller.CreateCustomer("Tom", "Adelaide", "person");
    Account account = _controller.CreateAccount(customer, "checking");

    _controller.RemoveAccount(account);

    Assert.Equal(1, Context.Customers.Count());
    Assert.Equal(0, Context.Accounts.Count());
  }

  [Fact]
  [Trait("Category", "MySqlIntegration")]
  public void RemoveCustomerWithMultipleAccountsShouldDeleteRelatedAccounts()
  {
    Customer customer = _controller.CreateCustomer("Lily", "Hobart", "person");
    _controller.CreateAccount(customer, "checking");
    _controller.CreateAccount(customer, "savings");

    _controller.RemoveCustomer(customer);

    Assert.Equal(0, Context.Customers.Count());
    Assert.Equal(0, Context.Accounts.Count());
  }

  [Fact]
  [Trait("Category", "MySqlIntegration")]
  public void UpdateCompanyIndustryShouldPersistInMySql()
  {
    Customer company = _controller.CreateCustomer("ACME", "Brisbane", "company", "ABN-123", "ACN-123");

    _controller.UpdateCustomer(company, industry: "Manufacturing");

    Company saved = Assert.IsType<Company>(Context.Customers.Single());
    Assert.Equal("Manufacturing", saved.Industry);
  }
}
