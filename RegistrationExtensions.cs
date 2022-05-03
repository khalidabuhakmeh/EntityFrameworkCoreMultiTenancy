namespace EntityFrameworkCoreMultiTenancy;

public static class RegistrationExtensions
{
    public static IServiceCollection AddScopedAs<T>(
        this IServiceCollection services, IEnumerable<Type> types) 
        where T : class
    {
        // register the type first
        services.AddScoped<T>();

        foreach (var type in types)
        {
            // register a scoped 
            services.AddScoped(type, svc =>
            {
                var rs = svc.GetRequiredService<T>();
                return rs;
            });
        }

        return services;
    }
}