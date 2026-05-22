namespace BankAccountManagement.Models;

public abstract class Account
{
  private static long nextAccountId = 1000;
  public long AccountId { get; private set; }
  public double Balance { get; protected set; }
  public DateTime CreatedAt { get; private set; }
  public bool IsActive { get; private set; }

  public long CustomerId { get; private set; }

  public Customer Customer { get; private set; } = null!;

  // protected means only this class and its child class can use it.
  protected Account()
  {
    AccountId = nextAccountId;
    nextAccountId += 5;

    Balance = 0;
    CreatedAt = DateTime.Now;
    IsActive = true;
  }

  public static void SetNextAccountId(long nextId)
  {
    if (nextId > nextAccountId)
    {
      nextAccountId = nextId;
    }
  }

  public virtual double Withdraw(double amount)
  {
    ValidateAmount(amount);

    Balance -= amount;

    return amount;
  }

  public void Deposit(double amount)
  {
    ValidateAmount(amount);

    Balance += amount;
  }

  public void CorrectBalance(double amount)
  {
    Balance = amount;
  }

  public void Deactivate()
  {
    IsActive = false;
  }

  protected void ValidateAmount(double amount)
  {
    if (amount <= 0)
    {
      throw new ArgumentException("Amount should be greater than 0.");
    }
  }
}
