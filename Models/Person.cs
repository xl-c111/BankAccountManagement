namespace BankAccountManagement.Models;

/// <summary>
/// Represents an individual customer.
/// </summary>
public class Person : Customer
{
  public DateTime DateOfBirth { get; set; }
  public string? Occupation { get; set; }

  protected Person() { }

  /// <summary>
  /// Creates a person customer with name and address.
  /// </summary>
  /// <param name="name">The customer's name.</param>
  /// <param name="address">The customer's address.</param>
  /// <exception cref="ArgumentException">Thrown when name or address is empty.</exception>
  public Person(string name, string address) : base(name, address)
  {
  }

  /// <summary>
  /// Charges the same amount from each account owned by the person.
  /// </summary>
  /// <param name="amount">The amount to charge from each account.</param>
  /// <exception cref="ArgumentException">Thrown when the amount is less than or equal to zero.</exception>
  public override void ChargeAllAccounts(double amount)
  {
    if (amount <= 0)
    {
      throw new ArgumentException("Charge amount must be greater than 0.");
    }
    foreach (Account account in Accounts)
    {
      account.Withdraw(amount);
    }
  }
}
