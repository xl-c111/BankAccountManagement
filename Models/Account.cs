namespace BankAccountManagement.Models;

/// <summary>
/// Base class for checking and savings accounts.
/// </summary>
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

  /// <summary>
  /// Sets the next generated account id when existing records are already stored.
  /// </summary>
  /// <param name="nextId">The next account id to use if it is greater than the current value.</param>
  public static void SetNextAccountId(long nextId)
  {
    if (nextId > nextAccountId)
    {
      nextAccountId = nextId;
    }
  }

  /// <summary>
  /// Withdraws money from the account. Base account withdrawals can overdraw the balance.
  /// </summary>
  /// <param name="amount">The amount to withdraw.</param>
  /// <returns>The amount withdrawn.</returns>
  /// <exception cref="ArgumentException">Thrown when the amount is less than or equal to zero.</exception>
  public virtual double Withdraw(double amount)
  {
    ValidateAmount(amount);

    Balance -= amount;

    return amount;
  }

  /// <summary>
  /// Deposits money into the account.
  /// </summary>
  /// <param name="amount">The amount to deposit.</param>
  /// <exception cref="ArgumentException">Thrown when the amount is less than or equal to zero.</exception>
  public void Deposit(double amount)
  {
    ValidateAmount(amount);

    Balance += amount;
  }

  /// <summary>
  /// Replaces the account balance with a specific value.
  /// </summary>
  /// <param name="amount">The new balance value.</param>
  public void CorrectBalance(double amount)
  {
    Balance = amount;
  }

  /// <summary>
  /// Marks the account as inactive.
  /// </summary>
  public void Deactivate()
  {
    IsActive = false;
  }

  /// <summary>
  /// Validates that an amount is greater than zero.
  /// </summary>
  /// <param name="amount">The amount to validate.</param>
  /// <exception cref="ArgumentException">Thrown when the amount is less than or equal to zero.</exception>
  protected void ValidateAmount(double amount)
  {
    if (amount <= 0)
    {
      throw new ArgumentException("Amount should be greater than 0.");
    }
  }
}
