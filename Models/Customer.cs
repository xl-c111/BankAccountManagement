namespace BankAccountManagement.Models;

/// <summary>
/// Base class for person and company customers.
/// </summary>
public abstract class Customer
{
  private static long nextCustomerId = 2_000_000;

  public long CustomerId { get; private set; }

  public string Name { get; set; } = string.Empty;

  public string Address { get; set; } = string.Empty;

  public string? PhoneNumber { get; set; }

  public string? Email { get; set; }

  public DateTime CreatedAt { get; private set; }

  public bool IsActive { get; private set; }

  public List<Account> Accounts { get; private set; } = new List<Account>();

  protected Customer()
  {
  }

  protected Customer(string name, string address)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentException("Customer name cannot be empty.");
    }

    if (string.IsNullOrWhiteSpace(address))
    {
      throw new ArgumentException("Customer address cannot be empty.");
    }

    CustomerId = nextCustomerId;
    nextCustomerId += 7;

    Name = name;
    Address = address;
    PhoneNumber = "";
    Email = "";
    CreatedAt = DateTime.Now;
    IsActive = true;
  }

  /// <summary>
  /// Sets the next generated customer id when existing records are already stored.
  /// </summary>
  /// <param name="nextId">The next customer id to use if it is greater than the current value.</param>
  public static void SetNextCustomerId(long nextId)
  {
    if (nextId > nextCustomerId)
    {
      nextCustomerId = nextId;
    }
  }

  /// <summary>
  /// Adds an account to this customer.
  /// </summary>
  /// <param name="account">The account to add.</param>
  /// <exception cref="ArgumentException">Thrown when the account is null or already exists.</exception>
  public void AddAccount(Account account)
  {
    ValidateAccountNotNull(account);

    if (Accounts.Contains(account))
    {
      throw new ArgumentException("This account already exists for this customer.");
    }

    Accounts.Add(account);
  }

  /// <summary>
  /// Removes an account from this customer.
  /// </summary>
  /// <param name="account">The account to remove.</param>
  /// <exception cref="ArgumentException">Thrown when the account is null or does not exist.</exception>
  public void RemoveAccount(Account account)
  {
    ValidateAccountExists(account);

    Accounts.Remove(account);
  }

  /// <summary>
  /// Marks the customer as inactive.
  /// </summary>
  public void Deactivate()
  {
    IsActive = false;
  }

  /// <summary>
  /// Charges all accounts owned by this customer.
  /// </summary>
  /// <param name="amount">The amount to charge.</param>
  /// <exception cref="ArgumentException">Thrown when the amount is less than or equal to zero.</exception>
  public abstract void ChargeAllAccounts(double amount);

  private void ValidateAccountNotNull(Account account)
  {
    if (account == null)
    {
      throw new ArgumentException("Account cannot be null.");
    }
  }

  private void ValidateAccountExists(Account account)
  {
    ValidateAccountNotNull(account);

    if (!Accounts.Contains(account))
    {
      throw new ArgumentException("This account does not exist for this customer.");
    }
  }
}
