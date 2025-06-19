namespace AdminPanelBack;

public static class UsersStore
{
    public static readonly Dictionary<string, string> Users = new()
    {
        ["admin"] = "1234",
        ["admin2"] = "pass5678"
    };
}