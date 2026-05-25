namespace BankAccountManagement.Persistence;

// Lightweight loader for local .env files.
// This keeps database credentials out of source code while avoiding an extra dependency.
public static class EnvFileLoader
{
  public static string? GetValue(string key, string envFileName = ".env")
  {
    string? currentDirectory = Directory.GetCurrentDirectory();

    while (!string.IsNullOrWhiteSpace(currentDirectory))
    {
      string envFilePath = Path.Combine(currentDirectory, envFileName);

      if (File.Exists(envFilePath))
      {
        string? value = ReadValueFromFile(key, envFilePath);

        if (!string.IsNullOrWhiteSpace(value))
        {
          return value;
        }
      }

      currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
    }

    return null;
  }

  private static string? ReadValueFromFile(string key, string envFilePath)
  {
    foreach (string rawLine in File.ReadLines(envFilePath))
    {
      if (!TryParseKeyValue(rawLine, out string currentKey, out string value))
      {
        continue;
      }

      if (!string.Equals(currentKey, key, StringComparison.Ordinal))
      {
        continue;
      }

      value = UnquoteIfNeeded(value);

      return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    return null;
  }

  private static bool TryParseKeyValue(string rawLine, out string key, out string value)
  {
    key = string.Empty;
    value = string.Empty;

    string line = rawLine.Trim();

    // Ignore blank lines and comments so .env files can stay readable.
    if (line.Length == 0 || line.StartsWith('#'))
    {
      return false;
    }

    // Only parse simple KEY=VALUE lines.
    int separatorIndex = line.IndexOf('=');
    if (separatorIndex <= 0)
    {
      return false;
    }

    key = line[..separatorIndex].Trim();
    value = line[(separatorIndex + 1)..].Trim();
    return true;
  }

  private static string UnquoteIfNeeded(string value)
  {
    // Support quoted values such as BANK_DB_CONNECTION="server=..."
    if (value.StartsWith('"') && value.EndsWith('"') && value.Length >= 2)
    {
      return value[1..^1];
    }

    return value;
  }
}
