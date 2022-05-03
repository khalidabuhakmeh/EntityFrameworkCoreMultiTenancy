namespace EntityFrameworkCoreMultiTenancy;

public class MultiTenantServiceMiddleware : IMiddleware
{
    private readonly ITenantSetter setter;

    public MultiTenantServiceMiddleware(ITenantSetter setter)
    {
        this.setter = setter;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Query.TryGetValue("tenant", out var values))
        {
            var tenant = Tenants.Find(values.FirstOrDefault());
            setter.SetTenant(tenant);
        }
        else
        {
            // set default tenant
            setter.SetTenant(Tenants.Internet);
        }

        await next(context);
    }
}