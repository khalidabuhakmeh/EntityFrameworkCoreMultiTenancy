namespace EntityFrameworkCoreMultiTenancy;

#nullable disable
public class Tenant
{
    public string Name { get; set; }
    public string ConnectionString { get; set; }
}

public class TenantConfigurationSection
{
    public List<Tenant> Tenants { get; set; }
}
#nullable enable

public class TenantService : ITenantGetter, ITenantSetter
{
    public Tenant Tenant { get; private set; } = default!;

    public void SetTenant(Tenant tenant)
    {
        Tenant = tenant;
    }
}

public interface ITenantGetter 
{
    Tenant Tenant { get; }
}

public interface ITenantSetter
{
    void SetTenant(Tenant key);
}