
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCoreMultiTenancy;

public class Database : DbContext
{

    public DbSet<Animal> Animals { get; set; } = default!;

    public Database(DbContextOptions<Database> options)
        : base(options)
    {
    }

    public static async Task Initialize(WebApplication app)
    {
        var data = new Animal[]
        {
            new() {Id = 1, Kind = "Dog", Name = "Samson", Tenant = "Khalid"},
            new() {Id = 2, Kind = "Dog", Name = "Guinness", Tenant = "Khalid"},
            new() {Id = 3, Kind = "Cat", Name = "Grumpy Cat", Tenant = "Internet"},
            new() {Id = 4, Kind = "Cat", Name = "Mr. Bigglesworth", Tenant = "Internet"}
        };
        
        
        // initialize the databases
        var tenantConfig = app.Configuration.Get<TenantConfigurationSection>()!;
        foreach (var tenant in tenantConfig.Tenants)
        {
            using var scope = app.Services.CreateScope();
            var tenantSetter = scope.ServiceProvider.GetRequiredService<ITenantSetter>();
            tenantSetter.SetTenant(tenant);

            var db = scope.ServiceProvider.GetRequiredService<Database>();
            await db.Database.MigrateAsync();

            // unique data
            if (!db.Animals.Any())
            {
                var unique = data.Where(a => a.Tenant == tenant.Name).ToList();
                db.Animals.AddRange(unique);
                await db.SaveChangesAsync();
            }
        }
    }
    
}

public class Animal
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string Tenant { get; set; } = string.Empty;
}