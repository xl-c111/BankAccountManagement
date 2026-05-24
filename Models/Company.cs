namespace BankAccountManagement.Models;

/// <summary>
/// Represents a company customer.
/// </summary>
public class Company : Customer
{
  public string ABN { get; private set; } = null!;
  public string ACN { get; private set; } = null!;
  public string? Industry { get; set; }

  protected Company()
  {

  }

  /// <summary>
  /// Creates a company customer with name, address, ABN, and ACN.
  /// </summary>
  /// <param name="name">The company name.</param>
  /// <param name="address">The company address.</param>
  /// <param name="abn">The Australian Business Number.</param>
  /// <param name="acn">The Australian Company Number.</param>
  /// <exception cref="ArgumentException">Thrown when name, address, ABN, or ACN is empty.</exception>
  public Company(string name, string address, string abn, string acn) : base(name, address)
  {
    if (string.IsNullOrWhiteSpace(abn))
    {
      throw new ArgumentException("ABN cannot be empty.");
    }

    if (string.IsNullOrWhiteSpace(acn))
    {
      throw new ArgumentException("ACN cannot be empty.");
    }

    ABN = abn;
    ACN = acn;
  }

  /// <summary>
  /// Charges checking accounts normally and savings accounts at double amount.
  /// </summary>
  /// <param name="amount">The base amount to charge.</param>
  /// <exception cref="ArgumentException">Thrown when the amount is less than or equal to zero.</exception>
  public override void ChargeAllAccounts(double amount)
  {
    if (amount <= 0)
    {
      throw new ArgumentException("Charge amount must be greater than 0.");
    }

    foreach (Account account in Accounts)
    {
      if (account is SavingsAccount)
      {
        account.Withdraw(amount * 2);
      }
      else
      { account.Withdraw(amount); }
    }
  }
}
