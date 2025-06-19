namespace AdminPanelBack;
public static class UsersStore
{
    
    private static readonly string AdminsRaw = Environment.GetEnvironmentVariable("ADMINS") ?? "";

  
    public static readonly Dictionary<string, string> Users = ParseAdmins(AdminsRaw);

    private static Dictionary<string, string> ParseAdmins(string raw)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var pairs = raw.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach (var pair in pairs)
        {
            var parts = pair.Split(':', 2);
            if (parts.Length == 2)
            {
                var username = parts[0].Trim();
                var password = parts[1].Trim();
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    dict[username] = password;
                }
            }
        }

        return dict;
    }
}