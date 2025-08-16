using Microsoft.Extensions.DependencyInjection;

namespace DnDEntitiesBlazorComponents.IoC;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterDnDEntitiesBlazorComponentsServices(this IServiceCollection services)
    {
        services.AddScoped<ICharacterEditContext>(serviceProvider => serviceProvider.GetService<IGlobalEditContext>()!);
        services.AddScoped<IAttackEditContext>(serviceProvider => serviceProvider.GetService<IGlobalEditContext>()!);
        services.AddScoped<IGlobalEditContext, GlobalEditContext>();
        
        return services;
    }
}
