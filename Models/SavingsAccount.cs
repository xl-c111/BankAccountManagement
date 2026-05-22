namespace BankAccountManagement.Models;

public class SavingsAccount : Account
{
  public double InterestRate { get; private set; }

  protected SavingsAccount()
  {
  }
  public SavingsAccount(double interestRate)
  {
    if (interestRate < 0)
    {
      throw new ArgumentException("Interest rate cannot be negative.");
    }
    InterestRate = interestRate;
  }

  public void AddInterest()
  {
    Balance += Balance * InterestRate / 100;
  }

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
