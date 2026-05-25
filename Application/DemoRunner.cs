using BankAccountManagement.Controller;
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

    AccountController controller = new(context);

    // Step 2: Create a person customer and related accounts.
    Person personCustomer = (Person)controller.CreateCustomer("Mary Smith", "Melbourne", "person");
    CheckingAccount personCheckingAccount = (CheckingAccount)controller.CreateAccount(personCustomer, "checking");
    personCheckingAccount.Deposit(500);

    // Step 3.5: Simulate using one check number.
    // getNextCheckNumber() returns the current number and then increments by 1.
    int personIssuedCheckNumber = personCheckingAccount.GetNextCheckNumber();

    // Step 3: Create a person savings account.
    SavingsAccount personSavingsAccount = (SavingsAccount)controller.CreateAccount(personCustomer, "savings");
    personSavingsAccount.Deposit(1000);

    // Step 4: Test charge all accounts for person.
    // For Person, this should subtract the same amount from every account.
    personCustomer.ChargeAllAccounts(50);

    controller.UpdateCustomer(
      personCustomer,
      phoneNumber: "0400 111 222",
      email: "mary.smith@example.com",
      dateOfBirth: new DateTime(1994, 5, 12),
      occupation: "Analyst");

    // Step 5: Create a company customer and related accounts.
    Company companyCustomer = (Company)controller.CreateCustomer("Acme Pty Ltd", "Sydney", "company", "ABN-123456789", "ACN-987654321");
    CheckingAccount companyCheckingAccount = (CheckingAccount)controller.CreateAccount(companyCustomer, "checking");
    companyCheckingAccount.Deposit(2000);
    int companyIssuedCheckNumber = companyCheckingAccount.GetNextCheckNumber();

    SavingsAccount companySavingsAccount = (SavingsAccount)controller.CreateAccount(companyCustomer, "savings");
    companySavingsAccount.Deposit(5000);

    // Step 6: Test charge all accounts for company.
    // For Company, checking is charged normal amount, savings is charged double amount.
    companyCustomer.ChargeAllAccounts(60);

    controller.UpdateCustomer(
      companyCustomer,
      phoneNumber: "02 9000 1234",
      email: "accounts@acme.example.com",
      industry: "Technology");

    // Step 7: Persist balance changes from operations above.
    context.SaveChanges();

    // Step 8: Print created results.
    Console.WriteLine("Demo data saved to MySQL successfully.");
    Console.WriteLine();
    PrintCustomerSummary(personCustomer, personCheckingAccount, personSavingsAccount, personIssuedCheckNumber);
    Console.WriteLine();
    PrintCustomerSummary(companyCustomer, companyCheckingAccount, companySavingsAccount, companyIssuedCheckNumber);

    // Step 9: Update existing records through controller methods.
    controller.UpdateCustomer(
      personCustomer,
      address: "Brisbane",
      occupation: "Senior Analyst");
    controller.UpdateAccount(personCheckingAccount, newBalance: 777.77);

    Console.WriteLine();
    Console.WriteLine("Update demo completed:");
    Console.WriteLine($"Updated Person Address: {personCustomer.Address}");
    Console.WriteLine($"Updated Person Occupation: {personCustomer.Occupation}");
    Console.WriteLine($"Updated Checking Balance: {personCheckingAccount.Balance:F2}");

    // Step 10: Remove one account and one customer through controller methods.
    controller.RemoveAccount(companySavingsAccount);
    controller.RemoveCustomer(companyCustomer);

    Console.WriteLine();
    Console.WriteLine("Remove demo completed:");
    Console.WriteLine($"Remaining customers: {context.Customers.Count()}");
    Console.WriteLine($"Remaining accounts: {context.Accounts.Count()}");
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
