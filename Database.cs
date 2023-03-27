using Microsoft.EntityFrameworkCore;
using static EntityFrameworkCoreMultiTenancy.Tenants;

namespace EntityFrameworkCoreMultiTenancy;

public class Database : DbContext
{
    /// <summary>
    /// Will get set by middleware
    /// </summary>
    public IEnumerable<string> Tenants { get; set; } 
        = All;

    public DbSet<Animal> Animals => Set<Animal>();

    public Database(DbContextOptions<Database> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Animal>()
            // Uses the property on the instance
            // this needs to get set on each request
            .HasQueryFilter(a => Tenants.Contains(a.Tenant))
            .HasData(
                new() {Id = 1, Kind = "Dog", Name = "Samson", Tenant = "Khalid"},
                new() {Id = 2, Kind = "Dog", Name = "Guinness", Tenant = "Khalid"},
                new() {Id = 3, Kind = "Cat", Name = "Grumpy Cat", Tenant = "Internet"},
                new() {Id = 4, Kind = "Cat", Name = "Mr. Bigglesworth", Tenant = "Internet"}
            );
    }
}

public class Animal
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string Tenant { get; set; } = Internet;
}