using BankAccountManagement.Controller;
using BankAccountManagement.Models;
using BankAccountManagement.Tests.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankAccountManagement.Tests.Controller.Persistence;

[Collection("MySqlIntegration")]
public class AccountControllerDatabaseConstraintTests : MySqlTestBase
{
  private readonly AccountController _controller;

  public AccountControllerDatabaseConstraintTests()
  {
    _controller = new AccountController(Context);
  }

  [Fact]
  [Trait("Category", "MySqlIntegration")]
  public void CreateCustomer_NameTooLong_ShouldThrowDbUpdateException()
  {
    string tooLongName = new string('N', 101);

    Assert.Throws<DbUpdateException>(() => _controller.CreateCustomer(tooLongName, "Melbourne", "person"));
  }

  [Fact]
  [Trait("Category", "MySqlIntegration")]
  public void CreateCustomer_AddressTooLong_ShouldThrowDbUpdateException()
  {
    string tooLongAddress = new string('A', 201);

    Assert.Throws<DbUpdateException>(() => _controller.CreateCustomer("Jane", tooLongAddress, "person"));
  }

  [Fact]
  [Trait("Category", "MySqlIntegration")]
  public void UpdateCustomer_EmailTooLong_ShouldThrowDbUpdateException()
  {
    Customer customer = _controller.CreateCustomer("Jane", "Melbourne", "person");
    string tooLongEmail = $"{new string('e', 95)}@mail.com";

    Assert.Throws<DbUpdateException>(() => _controller.UpdateCustomer(customer, email: tooLongEmail));
  }

  [Fact]
  [Trait("Category", "MySqlIntegration")]
  public void CreateCompany_AbnTooLong_ShouldThrowDbUpdateException()
  {
    string tooLongAbn = new string('1', 21);

    Assert.Throws<DbUpdateException>(() =>
      _controller.CreateCustomer("ACME", "Sydney", "company", tooLongAbn, "ACN-123"));
  }

  [Fact]
  [Trait("Category", "MySqlIntegration")]
  public void UpdateCompany_IndustryTooLong_ShouldThrowDbUpdateException()
  {
    Customer company = _controller.CreateCustomer("ACME", "Brisbane", "company", "ABN-123", "ACN-123");
    string tooLongIndustry = new string('I', 101);

    Assert.Throws<DbUpdateException>(() => _controller.UpdateCustomer(company, industry: tooLongIndustry));
  }
}
