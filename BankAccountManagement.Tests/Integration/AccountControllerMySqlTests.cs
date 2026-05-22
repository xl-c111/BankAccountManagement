using BankAccountManagement.Controller;
using BankAccountManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAccountManagement.Tests.Integration;

public class AccountControllerMySqlTests : MySqlIntegrationTestBase
{
  [Fact(Skip = "Set BANK_TEST_DB_CONNECTION and remove Skip to run against MySQL.")]
  [Trait("Category", "MySqlIntegration")]
  public void Database_Is_Empty_AtStart()
  {
    AccountController controller = new(Context);

    Assert.Empty(controller.GetCustomers());
    Assert.Empty(controller.GetAccounts());
  }

  [Fact(Skip = "Set BANK_TEST_DB_CONNECTION and remove Skip to run against MySQL.")]
  [Trait("Category", "MySqlIntegration")]
  public void Add_And_Remove_Customer_PersistsInMySql()
  {
    AccountController controller = new(Context);

    Customer customer = controller.CreateCustomer("Jane D", "Melbourne", "person");
    controller.CreateAccount(customer, "checking");

    int customerCountAfterCreate = Context.Customers.Count();
    int accountCountAfterCreate = Context.Accounts.Count();

    controller.RemoveCustomer(customer);

    int customerCountAfterDelete = Context.Customers.Count();
    int accountCountAfterDelete = Context.Accounts.Count();

    Assert.Equal(1, customerCountAfterCreate);
    Assert.Equal(1, accountCountAfterCreate);
    Assert.Equal(0, customerCountAfterDelete);
    Assert.Equal(0, accountCountAfterDelete);
  }

  [Fact(Skip = "Set BANK_TEST_DB_CONNECTION and remove Skip to run against MySQL.")]
  [Trait("Category", "MySqlIntegration")]
  public void Tph_Discriminator_Works_ForPersonAndCompany()
  {
    AccountController controller = new(Context);

    controller.CreateCustomer("Mary", "Sydney", "person");
    controller.CreateCustomer("ACME", "Brisbane", "company", "ABN-123", "ACN-123");

    int personCount = Context.Persons.Count();
    int companyCount = Context.Companies.Count();
    int totalCount = Context.Customers.Count();

    Assert.Equal(1, personCount);
    Assert.Equal(1, companyCount);
    Assert.Equal(2, totalCount);
  }
}
