using BankAccountManagement.Models;
using BankAccountManagement.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankAccountManagement.Controller;

/// <summary>
/// Coordinates customer and account operations with the database.
/// </summary>
public class AccountController
{
  public List<Customer> Customers = new List<Customer>();
  public List<Account> Accounts = new List<Account>();

  private readonly BankDbContext _context;

  public AccountController(BankDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Creates a person or company customer and saves it.
  /// </summary>
  /// <param name="name">The customer name.</param>
  /// <param name="address">The customer address.</param>
  /// <param name="customerType">The customer type: person or company.</param>
  /// <param name="abn">The company ABN. Required when customer type is company.</param>
  /// <param name="acn">The company ACN. Required when customer type is company.</param>
  /// <returns>The created customer.</returns>
  /// <exception cref="ArgumentException">Thrown when input values are invalid.</exception>
  public Customer CreateCustomer(string name, string address, string customerType, string? abn = null, string? acn = null)
  {
    if (string.IsNullOrWhiteSpace(customerType))
    {
      throw new ArgumentException("Customer type cannot be empty.");
    }

    string normalizedCustomerType = customerType.Trim().ToLower();

    if (normalizedCustomerType == "person")
    {
      Person newPerson = new Person(name, address);

      // add the customer to system customer list
      Customers.Add(newPerson);

      // add the customer to the database
      _context.Customers.Add(newPerson);
      _context.SaveChanges();

      return newPerson;
    }

    if (normalizedCustomerType == "company")
    {
      if (string.IsNullOrWhiteSpace(abn) || string.IsNullOrWhiteSpace(acn))
      {
        throw new ArgumentException("ABN and ACN are required when customer type is company.");
      }

      Company newCompany = new Company(name, address, abn, acn);

      // add the customer to system customer list
      Customers.Add(newCompany);

      // add the customer to the database
      _context.Customers.Add(newCompany);
      _context.SaveChanges();

      return newCompany;
    }

    throw new ArgumentException("Customer type must be either 'person' or 'company'.");
  }

  /// <summary>
  /// Creates a checking or savings account for a customer and saves it.
  /// </summary>
  /// <param name="customer">The customer who owns the account.</param>
  /// <param name="account">The account type: checking or savings.</param>
  /// <returns>The created account.</returns>
  /// <exception cref="ArgumentException">Thrown when the customer or account type is invalid.</exception>
  public Account CreateAccount(Customer customer, string account)
  {
    if (customer == null)
    {
      throw new ArgumentException("Customer cannot be null.");
    }

    if (string.IsNullOrWhiteSpace(account))
    {
      throw new ArgumentException("Account type cannot be empty.");
    }

    string normalizedAccountType = account.Trim().ToLower();

    if (normalizedAccountType == "checking")
    {
      CheckingAccount checkingAccount = new CheckingAccount();

      // add the account to system account list
      Accounts.Add(checkingAccount);

      // add the account to the customer's account list
      customer.AddAccount(checkingAccount);

      // add the account to the database
      _context.Accounts.Add(checkingAccount);
      _context.SaveChanges();

      return checkingAccount;
    }

    if (normalizedAccountType == "savings")
    {
      SavingsAccount savingsAccount = new SavingsAccount(0);

      // add the account to the account list
      Accounts.Add(savingsAccount);

      // add the account to the customer's account list
      customer.AddAccount(savingsAccount);

      // add the account to the database
      _context.Accounts.Add(savingsAccount);
      _context.SaveChanges();

      return savingsAccount;
    }

    throw new ArgumentException("Account type must be either 'checking' or 'savings'.");
  }

  /// <summary>
  /// Removes a customer and all accounts owned by that customer.
  /// </summary>
  /// <param name="customer">The customer to remove.</param>
  /// <exception cref="ArgumentException">Thrown when the customer is null.</exception>
  public void RemoveCustomer(Customer customer)
  {
    if (customer == null)
    {
      throw new ArgumentException("Customer cannot be null.");
    }

    var customerAccounts = customer.Accounts.ToList();

    foreach (var account in customerAccounts)
    {
      // delete accounts related to the customer
      RemoveAccount(account);
    }

    // delete customer
    Customers.Remove(customer);

    // delete customer from the database
    _context.Customers.Remove(customer);
    _context.SaveChanges();
  }

  /// <summary>
  /// Removes an account from the controller, customer lists, and database.
  /// </summary>
  /// <param name="account">The account to remove.</param>
  /// <exception cref="ArgumentException">Thrown when the account is null.</exception>
  public void RemoveAccount(Account account)
  {
    if (account == null)
    {
      throw new ArgumentException("Account cannot be null.");
    }

    // remove the account from system account list
    Accounts.Remove(account);

    foreach (Customer customer in Customers)
    {
      // remove the account from the customer's account list
      if (customer.Accounts.Contains(account))
      {
        customer.RemoveAccount(account);
      }
    }

    // remove the account from the database
    _context.Accounts.Remove(account);
    _context.SaveChanges();
  }

  /// <summary>
  /// Returns all customers with their accounts.
  /// </summary>
  /// <returns>A list of customers including their accounts.</returns>
  public List<Customer> GetCustomers()
  {
    return _context.Customers
      .Include(customer => customer.Accounts)
      .ToList();
  }

  /// <summary>
  /// Returns all accounts.
  /// </summary>
  /// <returns>A list of all accounts.</returns>
  public List<Account> GetAccounts()
  {
    return _context.Accounts.ToList();
  }
}
