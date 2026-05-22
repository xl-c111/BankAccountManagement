using BankAccountManagement.Controller;
using BankAccountManagement.Models;
using BankAccountManagement.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankAccountManagement.Tests.Controller;

public class AccountControllerUnitTests
  : IDisposable
{
  private readonly BankDbContext _context;
  private readonly AccountController _controller;

  public AccountControllerUnitTests()
  {
    DbContextOptions<BankDbContext> options = new DbContextOptionsBuilder<BankDbContext>()
      .UseInMemoryDatabase(Guid.NewGuid().ToString())
      .Options;

    _context = new BankDbContext(options);
    _controller = new AccountController(_context);
  }

  public void Dispose()
  {
    _context.Dispose();
  }

  [Fact]
  public void CreateCustomer_Person_SavesToListsAndDb()
  {
    Customer customer = _controller.CreateCustomer("Mary", "Melbourne", "person");

    Assert.IsType<Person>(customer);
    Assert.Single(_controller.Customers);
    Assert.Single(_context.Customers);
  }

  [Fact]
  public void CreateCustomer_Company_SavesToListsAndDb()
  {
    Customer customer = _controller.CreateCustomer("ACME", "Sydney", "company", "ABN-1", "ACN-1");

    Assert.IsType<Company>(customer);
    Assert.Single(_controller.Customers);
    Assert.Single(_context.Customers);
  }

  [Fact]
  public void CreateCustomer_CompanyWithoutAbnOrAcn_Throws()
  {
    Assert.Throws<ArgumentException>(() => _controller.CreateCustomer("ACME", "Sydney", "company"));
  }

  [Fact]
  public void CreateCustomer_InvalidType_Throws()
  {
    Assert.Throws<ArgumentException>(() => _controller.CreateCustomer("Mary", "Melbourne", "vip"));
  }

  [Fact]
  public void CreateCustomer_EmptyType_Throws()
  {
    Assert.Throws<ArgumentException>(() => _controller.CreateCustomer("Mary", "Melbourne", ""));
  }

  [Fact]
  public void CreateAccount_Checking_SavesAndLinksToCustomer()
  {
    Customer customer = _controller.CreateCustomer("Mary", "Melbourne", "person");

    Account account = _controller.CreateAccount(customer, "checking");

    Assert.IsType<CheckingAccount>(account);
    Assert.Single(_controller.Accounts);
    Assert.Single(customer.Accounts);
    Assert.Single(_context.Accounts);
  }

  [Fact]
  public void CreateAccount_Savings_SavesAndLinksToCustomer()
  {
    Customer customer = _controller.CreateCustomer("Mary", "Melbourne", "person");

    Account account = _controller.CreateAccount(customer, "savings");

    Assert.IsType<SavingsAccount>(account);
    Assert.Single(_controller.Accounts);
    Assert.Single(customer.Accounts);
    Assert.Single(_context.Accounts);
  }

  [Fact]
  public void CreateAccount_InvalidType_Throws()
  {
    Customer customer = _controller.CreateCustomer("Mary", "Melbourne", "person");

    Assert.Throws<ArgumentException>(() => _controller.CreateAccount(customer, "premium"));
  }

  [Fact]
  public void CreateAccount_NullCustomer_Throws()
  {
    Assert.Throws<ArgumentException>(() => _controller.CreateAccount(null!, "checking"));
  }

  [Fact]
  public void CreateAccount_EmptyType_Throws()
  {
    Customer customer = _controller.CreateCustomer("Mary", "Melbourne", "person");

    Assert.Throws<ArgumentException>(() => _controller.CreateAccount(customer, ""));
  }

  [Fact]
  public void RemoveAccount_RemovesFromListsAndDb()
  {
    Customer customer = _controller.CreateCustomer("Mary", "Melbourne", "person");
    Account account = _controller.CreateAccount(customer, "checking");

    _controller.RemoveAccount(account);

    Assert.Empty(_controller.Accounts);
    Assert.Empty(customer.Accounts);
    Assert.Empty(_context.Accounts);
  }

  [Fact]
  public void RemoveAccount_Null_Throws()
  {
    Assert.Throws<ArgumentException>(() => _controller.RemoveAccount(null!));
  }

  [Fact]
  public void RemoveCustomer_RemovesCustomerAndAccountsFromDb()
  {
    Customer customer = _controller.CreateCustomer("Mary", "Melbourne", "person");
    _controller.CreateAccount(customer, "checking");
    _controller.CreateAccount(customer, "savings");

    _controller.RemoveCustomer(customer);

    Assert.Empty(_controller.Customers);
    Assert.Empty(_context.Customers);
    Assert.Empty(_context.Accounts);
  }

  [Fact]
  public void RemoveCustomer_Null_Throws()
  {
    Assert.Throws<ArgumentException>(() => _controller.RemoveCustomer(null!));
  }

  [Fact]
  public void GetCustomers_ReturnsCustomersWithAccounts()
  {
    Customer customer = _controller.CreateCustomer("Mary", "Melbourne", "person");
    _controller.CreateAccount(customer, "checking");

    List<Customer> customers = _controller.GetCustomers();

    Assert.Single(customers);
    Assert.Single(customers[0].Accounts);
  }

  [Fact]
  public void GetAccounts_ReturnsAccountsFromDb()
  {
    Customer customer = _controller.CreateCustomer("Mary", "Melbourne", "person");
    _controller.CreateAccount(customer, "checking");
    _controller.CreateAccount(customer, "savings");

    List<Account> accounts = _controller.GetAccounts();

    Assert.Equal(2, accounts.Count);
    _ = accounts[0].CustomerId;
  }
}
