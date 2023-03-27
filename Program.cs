using EntityFrameworkCoreMultiTenancy;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<MultiTenantServiceMiddleware>();
builder.Services.AddDbContextPool<Database>(db => {
    db.UseSqlite("Data Source=multi-tenant.db");
});
var app = builder.Build();

// initialize the database
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<Database>();
    await db.Database.MigrateAsync();    
}

// middleware that reads and sets the tenant
app.UseMiddleware<MultiTenantServiceMiddleware>();

// multi-tenant request, try adding ?tenant=Khalid or ?tenant=Internet (default)
app.MapGet("/", async (Database db) => await db
    .Animals
    // hide the tenant, which is response noise
    .Select(x => new { x.Id, x.Name, x.Kind })
    .ToListAsync());

app.Run();