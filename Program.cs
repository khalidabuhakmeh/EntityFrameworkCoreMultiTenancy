using EntityFrameworkCoreMultiTenancy;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// tenant setter & getter
builder.Services.AddScopedAs<TenantService>(new[] {typeof(ITenantGetter), typeof(ITenantSetter)});

// IOptions version of tenants
builder.Services.Configure<TenantConfigurationSection>(builder.Configuration);

// middleware that sets the current tenant
builder.Services.AddScoped<MultiTenantServiceMiddleware>();
builder.Services.AddDbContext<Database>((s, o) =>
{
    var tenant = s.GetRequiredService<ITenantGetter>().Tenant;
    // multi-tenant databases, happy Maarten?!
    o.UseSqlite(tenant.ConnectionString);
});
var app = builder.Build();

// initialize the databases
var tenantConfig = builder.Configuration.Get<TenantConfigurationSection>()!;
foreach (var tenant in tenantConfig.Tenants)
{
    using var scope = app.Services.CreateScope();
    var tenantSetter = scope.ServiceProvider.GetRequiredService<ITenantSetter>();
    tenantSetter.SetTenant(tenant);

    var db = scope.ServiceProvider.GetRequiredService<Database>();
    await db.Database.MigrateAsync();
}

// middleware that reads and sets the tenant
app.UseMiddleware<MultiTenantServiceMiddleware>();

// multi-tenant request, try adding ?tenant=Khalid or ?tenant=Internet (default)
app.MapGet("/", async (Database db) => await db
    .Animals
    // hide the tenant, which is response noise
    .Select(x => new {x.Id, x.Name, x.Kind})
    .ToListAsync());

app.Run();