using AspNetCoreExtensions.Navigations;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreExtensions.IoC;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterAspNetCoreExtensions(this IServiceCollection services)
    {
        services.AddScoped<IStateFullNavigation, StateFullNavigation>();

        return services;
    }
}
