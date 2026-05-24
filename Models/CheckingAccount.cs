namespace BankAccountManagement.Models;

/// <summary>
/// Represents a checking account with check number tracking.
/// </summary>
public class CheckingAccount : Account
{
  public int NextCheckNumber { get; private set; }

  public CheckingAccount()
  {
    NextCheckNumber = 1;
  }

  /// <summary>
  /// Returns the next check number and increments it by one.
  /// </summary>
  /// <returns>The current check number before it is incremented.</returns>
  public int GetNextCheckNumber()
  {
    int currentCheckNumber = NextCheckNumber;

    NextCheckNumber += 1;

    return currentCheckNumber;
  }
}
