namespace EntityFrameworkCoreMultiTenancy;

public static class Tenants
{
    public const string Internet = nameof(Internet);
    public const string Khalid = nameof(Khalid);

    public static IReadOnlyCollection<string> All = new[] {Internet, Khalid};

    public static string Find(string? value)
    {
        return All.FirstOrDefault(t => t.Equals(value?.Trim(), StringComparison.OrdinalIgnoreCase)) ?? Internet;
    }
}