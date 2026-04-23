using DnDFightTool.Infrastructure.AspNetCoreExtensions.Navigations;
using Microsoft.Extensions.DependencyInjection;

namespace DnDFightTool.Infrastructure.AspNetCoreExtensions.IoC;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterAspNetCoreExtensions(this IServiceCollection services)
    {
        services.AddScoped<IStateFullNavigation, StateFullNavigation>();

        return services;
    }
}
