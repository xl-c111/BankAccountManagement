using BankAccountManagement.Application;
using BankAccountManagement.Models;
using BankAccountManagement.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankAccountManagement.Tests.Application;

public class DemoRunnerTests : IDisposable
{
  private readonly BankDbContext _context;
  private readonly StringWriter _output;
  private readonly TextWriter _originalOutput;

  public DemoRunnerTests()
  {
    DbContextOptions<BankDbContext> options = new DbContextOptionsBuilder<BankDbContext>()
      .UseInMemoryDatabase(Guid.NewGuid().ToString())
      .Options;

    _context = new BankDbContext(options);
    _output = new StringWriter();
    _originalOutput = Console.Out;
    Console.SetOut(_output);
  }

  [Fact]
  public void Run_ShouldCreatePersonCompanyAndAccounts()
  {
    DemoRunner runner = new(_context);

    runner.Run();

    Assert.Equal(1, _context.Customers.Count());
    Assert.Equal(2, _context.Accounts.Count());
    Assert.Single(_context.Persons);
    Assert.Empty(_context.Companies);
    Assert.Equal(1, _context.CheckingAccounts.Count());
    Assert.Equal(1, _context.SavingsAccounts.Count());
  }

  [Fact]
  public void Run_ShouldPrintFormattedDemoOutput()
  {
    DemoRunner runner = new(_context);

    runner.Run();

    string output = _output.ToString();
    Assert.Contains("Demo data saved to MySQL successfully.", output);
    Assert.Contains("Customer Type: Person", output);
    Assert.Contains("Customer Type: Company", output);
    Assert.Contains("Date Of Birth: 1994-05-12", output);
    Assert.Contains("Next Check Number: 2", output);
    Assert.Contains("Update demo completed:", output);
    Assert.Contains("Remove demo completed:", output);
  }

  [Fact]
  public void Run_WhenDatabaseHasExistingRecords_ShouldContinueGeneratedIds()
  {
    Person existingCustomer = new("Existing Customer", "Melbourne");
    CheckingAccount existingAccount = new();
    existingCustomer.AddAccount(existingAccount);
    _context.Customers.Add(existingCustomer);
    _context.Accounts.Add(existingAccount);
    _context.SaveChanges();

    DemoRunner runner = new(_context);

    runner.Run();

    Assert.Equal(2, _context.Customers.Count());
    Assert.Equal(3, _context.Accounts.Count());
    Assert.Contains(_context.Customers, customer => customer.CustomerId > existingCustomer.CustomerId);
    Assert.Contains(_context.Accounts, account => account.AccountId > existingAccount.AccountId);
  }

  public void Dispose()
  {
    Console.SetOut(_originalOutput);
    _output.Dispose();
    _context.Dispose();
  }
}
