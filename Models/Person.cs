namespace BankAccountManagement.Models;

public class Person : Customer
{
  public DateTime DateOfBirth { get; set; }
  public string? Occupation { get; set; }

  protected Person() { }

  public Person(string name, string address) : base(name, address)
  {
  }

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
