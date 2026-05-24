namespace BankAccountManagement.Persistence;

// Lightweight loader for local .env files.
// This keeps database credentials out of source code while avoiding an extra dependency.
public static class EnvFileLoader
{
  public static string? GetValue(string key, string envFilePath = ".env")
  {
    // Missing .env files are allowed because production or CI can use real environment variables.
    if (!File.Exists(envFilePath))
    {
      return null;
    }

    foreach (string rawLine in File.ReadLines(envFilePath))
    {
      string line = rawLine.Trim();

      // Ignore blank lines and comments so .env files can stay readable.
      if (line.Length == 0 || line.StartsWith('#'))
      {
        continue;
      }

      // Only parse simple KEY=VALUE lines.
      int separatorIndex = line.IndexOf('=');
      if (separatorIndex <= 0)
      {
        continue;
      }

      // Extract the key from the left side of KEY=VALUE.
      string currentKey = line[..separatorIndex].Trim();
      if (!string.Equals(currentKey, key, StringComparison.Ordinal))
      {
        continue;
      }

      // Extract the value from the right side of KEY=VALUE.
      string value = line[(separatorIndex + 1)..].Trim();

      // Support quoted values such as BANK_DB_CONNECTION="server=..."
      if (value.StartsWith('"') && value.EndsWith('"') && value.Length >= 2)
      {
        // Remove the surrounding quote characters.
        value = value[1..^1];
      }

      return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    return null;
  }
}
