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

public class TenantService : ITenantService, ITenantSetter
{
    public string Tenant { get; private set; } = Tenants.Internet;

    public void SetTenant(string tenant)
    {
        Tenant = tenant;
    }
}

public interface ITenantService 
{
    string Tenant { get; }
}

public interface ITenantSetter
{
    void SetTenant(string tenant);
}