namespace EntityFrameworkCoreMultiTenancy;

public class MultiTenantServiceMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var db = context.RequestServices.GetRequiredService<Database>();
        if (context.Request.Query.TryGetValue("tenant", out var values))
        {
            var tenant = Tenants.Find(values.FirstOrDefault());
            db.Tenants = new[] { tenant };
        }
        else
        {
            // set default tenant
            db.Tenants = new[] { Tenants.Internet };
        }

        await next(context);
    }
}