using BankAccountManagement.Models;
using BankAccountManagement.Persistence;

namespace BankAccountManagement.Application;

public class DemoRunner
{
  public void Run()
  {
    using var context = new BankDbContext();

    // Step 1: Update static ID generators based on existing database records.
    // This prevents duplicate primary key errors when running the program multiple times.
    if (context.Customers.Any())
    {
      long nextCustomerId = context.Customers.Max(c => c.CustomerId) + 7;
      Customer.SetNextCustomerId(nextCustomerId);
    }

    if (context.Accounts.Any())
    {
      long nextAccountId = context.Accounts.Max(a => a.AccountId) + 5;
      Account.SetNextAccountId(nextAccountId);
    }

    // Step 2: Create a customer.
    Person customer = new Person("Mary Smith", "Melbourne");

    // Step 3: Create a checking account.
    CheckingAccount checkingAccount = new CheckingAccount();
    checkingAccount.Deposit(500);

    // Step 4: Create a savings account.
    SavingsAccount savingsAccount = new SavingsAccount(2.5);
    savingsAccount.Deposit(1000);
    savingsAccount.AddInterest();

    // Step 5: Add both accounts to the customer.
    customer.AddAccount(checkingAccount);
    customer.AddAccount(savingsAccount);

    // Step 6: Test charge all accounts.
    // For Person, this should subtract the same amount from every account.
    customer.ChargeAllAccounts(50);

    // Step 7: Add data to EF Core context.
    context.Customers.Add(customer);
    context.Accounts.Add(checkingAccount);
    context.Accounts.Add(savingsAccount);

    // Step 8: Save changes to MySQL.
    context.SaveChanges();

    // Step 9: Print results.
    Console.WriteLine("Test data saved to MySQL successfully.");
    Console.WriteLine();
    Console.WriteLine($"Customer ID: {customer.CustomerId}");
    Console.WriteLine($"Customer Name: {customer.Name}");
    Console.WriteLine();
    Console.WriteLine($"Checking Account ID: {checkingAccount.AccountId}");
    Console.WriteLine($"Checking Balance: {checkingAccount.Balance}");
    Console.WriteLine($"Next Check Number: {checkingAccount.NextCheckNumber}");
    Console.WriteLine();
    Console.WriteLine($"Savings Account ID: {savingsAccount.AccountId}");
    Console.WriteLine($"Savings Balance: {savingsAccount.Balance}");
    Console.WriteLine($"Interest Rate: {savingsAccount.InterestRate}");
  }
}
