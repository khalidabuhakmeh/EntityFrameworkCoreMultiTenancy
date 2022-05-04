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
    var tenant = s.GetService<ITenantGetter>()?.Tenant;
    // for migrations
    var connectionString = tenant?.ConnectionString ?? "Data Source=default.db";
    // multi-tenant databases
    o.UseSqlite(connectionString);
});

var app = builder.Build();
await Database.Initialize(app);

// middleware that reads and sets the tenant
app.UseMiddleware<MultiTenantServiceMiddleware>();

// multi-tenant request, try adding ?tenant=Khalid or ?tenant=Internet (default)
app.MapGet("/", async (Database db) => await db
    .Animals
    // hide the tenant, which is response noise
    .Select(x => new {x.Id, x.Name, x.Kind})
    .ToListAsync());

app.Run();