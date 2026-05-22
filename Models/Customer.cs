namespace BankAccountManagement.Models;

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

  public static void SetNextCustomerId(long nextId)
  {
    if (nextId > nextCustomerId)
    {
      nextCustomerId = nextId;
    }
  }

  public void AddAccount(Account account)
  {
    ValidateAccountNotNull(account);

    if (Accounts.Contains(account))
    {
      throw new ArgumentException("This account already exists for this customer.");
    }

    Accounts.Add(account);
  }

  public void RemoveAccount(Account account)
  {
    ValidateAccountExists(account);

    Accounts.Remove(account);
  }

  public void Deactivate()
  {
    IsActive = false;
  }

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
