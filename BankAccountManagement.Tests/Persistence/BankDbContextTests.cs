using BankAccountManagement.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankAccountManagement.Tests.Persistence;

public class BankDbContextTests
{
  [Fact]
  public void Constructor_WithOptions_ShouldUseProvidedOptions()
  {
    DbContextOptions<BankDbContext> options = new DbContextOptionsBuilder<BankDbContext>()
      .UseInMemoryDatabase(Guid.NewGuid().ToString())
      .Options;

    using BankDbContext context = new(options);

    Assert.NotNull(context.Customers);
    Assert.NotNull(context.Accounts);
    Assert.NotNull(context.Persons);
    Assert.NotNull(context.Companies);
    Assert.NotNull(context.CheckingAccounts);
    Assert.NotNull(context.SavingsAccounts);
  }
}
