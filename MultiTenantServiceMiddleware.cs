using Microsoft.Extensions.Options;

namespace EntityFrameworkCoreMultiTenancy;

public class MultiTenantServiceMiddleware : IMiddleware
{
    private readonly ITenantSetter setter;
    private readonly IOptions<TenantConfigurationSection> config;
    private readonly ILogger<MultiTenantServiceMiddleware> logger;

    public MultiTenantServiceMiddleware(
        ITenantSetter setter, 
        IOptions<TenantConfigurationSection> config, 
        ILogger<MultiTenantServiceMiddleware> logger)
    {
        this.setter = setter;
        this.config = config;
        this.logger = logger;
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

        logger.LogInformation("Using the tenant {tenant}", tenant.Name);
        setter.SetTenant(tenant);
        
        await next(context);
    }
}