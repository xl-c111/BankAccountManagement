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

    Assert.Equal(2, _context.Customers.Count());
    Assert.Equal(4, _context.Accounts.Count());
    Assert.Single(_context.Persons);
    Assert.Single(_context.Companies);
    Assert.Equal(2, _context.CheckingAccounts.Count());
    Assert.Equal(2, _context.SavingsAccounts.Count());
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
  }

  public void Dispose()
  {
    Console.SetOut(_originalOutput);
    _output.Dispose();
    _context.Dispose();
  }
}
