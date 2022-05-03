using Microsoft.Extensions.Options;

namespace EntityFrameworkCoreMultiTenancy;

public class MultiTenantServiceMiddleware : IMiddleware
{
    private readonly ITenantSetter setter;
    private readonly IOptions<TenantConfigurationSection> config;

    public MultiTenantServiceMiddleware(ITenantSetter setter, IOptions<TenantConfigurationSection> config)
    {
        this.setter = setter;
        this.config = config;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var tenant = config.Value.Tenants.First();
        
        if (context.Request.Query.TryGetValue("tenant", out var values))
        {
            var key = values.First();
            tenant = config.Value
                .Tenants
                .FirstOrDefault(t => t.Name.Equals(key?.Trim(), StringComparison.OrdinalIgnoreCase)) ?? tenant;
        }

        setter.SetTenant(tenant);
        
        await next(context);
    }
}