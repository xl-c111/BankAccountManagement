using BankAccountManagement.Models;
using BankAccountManagement.Persistence;

namespace BankAccountManagement.Application;

public class DemoRunner
{
  private readonly BankDbContext? _context;

  public DemoRunner()
  {
  }

  public DemoRunner(BankDbContext context)
  {
    _context = context;
  }

  public void Run()
  {
    using BankDbContext? ownedContext = _context == null ? new BankDbContext() : null;
    BankDbContext context = _context ?? ownedContext!;

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

    // Step 2: Create a person customer and set optional profile fields.
    Person personCustomer = new Person("Mary Smith", "Melbourne")
    {
      PhoneNumber = "0400 111 222",
      Email = "mary.smith@example.com",
      DateOfBirth = new DateTime(1994, 5, 12).Date,
      Occupation = "Analyst"
    };

    // Step 3: Create a person checking account.
    CheckingAccount personCheckingAccount = new CheckingAccount();
    personCheckingAccount.Deposit(500);

    // Step 3.5: Simulate using one check number.
    // getNextCheckNumber() returns the current number and then increments by 1.
    int personIssuedCheckNumber = personCheckingAccount.GetNextCheckNumber();

    // Step 4: Create a person savings account.
    SavingsAccount personSavingsAccount = new SavingsAccount(2.5);
    personSavingsAccount.Deposit(1000);
    personSavingsAccount.AddInterest();

    // Step 5: Add both accounts to the person customer.
    personCustomer.AddAccount(personCheckingAccount);
    personCustomer.AddAccount(personSavingsAccount);

    // Step 6: Test charge all accounts for person.
    // For Person, this should subtract the same amount from every account.
    personCustomer.ChargeAllAccounts(50);

    // Step 7: Create a company customer and set optional profile fields.
    Company companyCustomer = new Company("Acme Pty Ltd", "Sydney", "ABN-123456789", "ACN-987654321")
    {
      PhoneNumber = "02 9000 1234",
      Email = "accounts@acme.example.com",
      Industry = "Technology"
    };

    // Step 8: Create company accounts.
    CheckingAccount companyCheckingAccount = new CheckingAccount();
    companyCheckingAccount.Deposit(2000);
    int companyIssuedCheckNumber = companyCheckingAccount.GetNextCheckNumber();

    SavingsAccount companySavingsAccount = new SavingsAccount(3.2);
    companySavingsAccount.Deposit(5000);
    companySavingsAccount.AddInterest();

    // Step 9: Add both accounts to the company customer.
    companyCustomer.AddAccount(companyCheckingAccount);
    companyCustomer.AddAccount(companySavingsAccount);

    // Step 10: Test charge all accounts for company.
    // For Company, checking is charged normal amount, savings is charged double amount.
    companyCustomer.ChargeAllAccounts(60);

    // Step 11: Add data to EF Core context.
    context.Customers.Add(personCustomer);
    context.Customers.Add(companyCustomer);
    context.Accounts.Add(personCheckingAccount);
    context.Accounts.Add(personSavingsAccount);
    context.Accounts.Add(companyCheckingAccount);
    context.Accounts.Add(companySavingsAccount);

    // Step 12: Save changes to MySQL.
    context.SaveChanges();

    // Step 13: Print results in a standardized format.
    Console.WriteLine("Demo data saved to MySQL successfully.");
    Console.WriteLine();
    PrintCustomerSummary(personCustomer, personCheckingAccount, personSavingsAccount, personIssuedCheckNumber);
    Console.WriteLine();
    PrintCustomerSummary(companyCustomer, companyCheckingAccount, companySavingsAccount, companyIssuedCheckNumber);
  }

  private static void PrintCustomerSummary(
    Customer customer,
    CheckingAccount checkingAccount,
    SavingsAccount savingsAccount,
    int issuedCheckNumber)
  {
    Console.WriteLine($"Customer ID: {customer.CustomerId}");
    Console.WriteLine($"Customer Type: {customer.GetType().Name}");
    Console.WriteLine($"Name: {customer.Name}");
    Console.WriteLine($"Address: {customer.Address}");
    Console.WriteLine($"Phone: {customer.PhoneNumber}");
    Console.WriteLine($"Email: {customer.Email}");
    Console.WriteLine($"Created At: {customer.CreatedAt:yyyy-MM-dd HH:mm:ss}");
    if (customer is Person person)
    {
      Console.WriteLine($"Date Of Birth: {person.DateOfBirth:yyyy-MM-dd}");
      Console.WriteLine($"Occupation: {person.Occupation}");
    }
    if (customer is Company company)
    {
      Console.WriteLine($"ABN: {company.ABN}");
      Console.WriteLine($"ACN: {company.ACN}");
      Console.WriteLine($"Industry: {company.Industry}");
    }
    Console.WriteLine();
    Console.WriteLine($"Checking Account ID: {checkingAccount.AccountId}");
    Console.WriteLine($"Checking Created At: {checkingAccount.CreatedAt:yyyy-MM-dd HH:mm:ss}");
    Console.WriteLine($"Checking Balance: {checkingAccount.Balance:F2}");
    Console.WriteLine($"Issued Check Number: {issuedCheckNumber}");
    Console.WriteLine($"Next Check Number: {checkingAccount.NextCheckNumber}");
    Console.WriteLine();
    Console.WriteLine($"Savings Account ID: {savingsAccount.AccountId}");
    Console.WriteLine($"Savings Created At: {savingsAccount.CreatedAt:yyyy-MM-dd HH:mm:ss}");
    Console.WriteLine($"Savings Balance: {savingsAccount.Balance:F2}");
    Console.WriteLine($"Interest Rate: {savingsAccount.InterestRate:F2}%");
  }
}
