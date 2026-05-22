namespace BankAccountManagement.Persistence;

public static class EnvFileLoader
{
  public static string? GetValue(string key, string envFilePath = ".env")
  {
    if (!File.Exists(envFilePath))
    {
      return null;
    }

    foreach (string rawLine in File.ReadLines(envFilePath))
    {
      string line = rawLine.Trim();

      if (line.Length == 0 || line.StartsWith('#'))
      {
        continue;
      }

      int separatorIndex = line.IndexOf('=');
      if (separatorIndex <= 0)
      {
        continue;
      }

      string currentKey = line[..separatorIndex].Trim();
      if (!string.Equals(currentKey, key, StringComparison.Ordinal))
      {
        continue;
      }

      string value = line[(separatorIndex + 1)..].Trim();
      if (value.StartsWith('"') && value.EndsWith('"') && value.Length >= 2)
      {
        value = value[1..^1];
      }
      return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    return null;
  }
}
