namespace Business.Users;

public class UserPreferences
{
    public string UserName { get; set; }
    public IEnumerable<CompanyInfo> Companies { get; set; } = Array.Empty<CompanyInfo>();
}

public class CompanyInfo
{
    public string Name { get; set; }
    public IEnumerable<string> SearchTags { get; set; }
}