
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCoreMultiTenancy;

public class Database : DbContext
{
    private readonly Tenant tenant;

    public DbSet<Animal> Animals { get; set; } = default!;

    public Database(DbContextOptions<Database> options, ITenantGetter tenantGetter)
        : base(options)
    {
        tenant = tenantGetter.Tenant;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Animal>()
            .HasQueryFilter(a => a.Tenant == tenant.Name)
            // the databases wouldn't share data, this is left
            // for ease of use and switching between branches
            .HasData(
                new() {Id = 1, Kind = "Dog", Name = "Samson", Tenant = "Khalid"},
                new() {Id = 2, Kind = "Dog", Name = "Guinness", Tenant = "Khalid"},
                new() {Id = 3, Kind = "Cat", Name = "Grumpy Cat", Tenant = "Internet"},
                new() {Id = 4, Kind = "Cat", Name = "Mr. Bigglesworth", Tenant = "Internet"}
            );
    }

    public static async Task Initialize(WebApplication app)
    {
        // initialize the databases
        var tenantConfig = app.Configuration.Get<TenantConfigurationSection>()!;
        foreach (var tenant in tenantConfig.Tenants)
        {
            using var scope = app.Services.CreateScope();
            var tenantSetter = scope.ServiceProvider.GetRequiredService<ITenantSetter>();
            tenantSetter.SetTenant(tenant);

            var db = scope.ServiceProvider.GetRequiredService<Database>();
            await db.Database.MigrateAsync();
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