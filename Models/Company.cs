namespace BankAccountManagement.Models;

public class Company : Customer
{
  public string ABN { get; private set; } = null!;
  public string ACN { get; private set; } = null!;
  public string? Industry { get; set; }

  protected Company()
  {

  }

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
