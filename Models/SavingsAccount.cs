namespace BankAccountManagement.Models;

/// <summary>
/// Represents a savings account with an interest rate.
/// </summary>
public class SavingsAccount : Account
{
  public double InterestRate { get; private set; }

  protected SavingsAccount()
  {
  }
  /// <summary>
  /// Creates a savings account with the provided interest rate.
  /// </summary>
  /// <param name="interestRate">The interest rate used to calculate account interest.</param>
  /// <exception cref="ArgumentException">Thrown when the interest rate is negative.</exception>
  public SavingsAccount(double interestRate)
  {
    if (interestRate < 0)
    {
      throw new ArgumentException("Interest rate cannot be negative.");
    }
    InterestRate = interestRate;
  }

  /// <summary>
  /// Adds interest to the current balance using Balance * InterestRate / 100.
  /// </summary>
  public void AddInterest()
  {
    Balance += Balance * InterestRate / 100;
  }

  /// <summary>
  /// Withdraws money without allowing the account to be overdrawn.
  /// </summary>
  /// <param name="amount">The amount to withdraw.</param>
  /// <returns>The amount withdrawn, or 0 when the requested amount is greater than the balance.</returns>
  /// <exception cref="ArgumentException">Thrown when the amount is less than or equal to zero.</exception>
  public override double Withdraw(double amount)
  {
    ValidateAmount(amount);

    if (amount > Balance)
    {
      return 0;
    }

    Balance -= amount;
    return amount;
  }
}
