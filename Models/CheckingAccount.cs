namespace BankAccountManagement.Models;

public class CheckingAccount : Account
{
  public int NextCheckNumber { get; private set; }

  public CheckingAccount()
  {
    NextCheckNumber = 1;
  }

  public int GetNextCheckNumber()
  {
    int currentCheckNumber = NextCheckNumber;

    NextCheckNumber += 1;

    return currentCheckNumber;
  }
}
