using Microsoft.Extensions.DependencyInjection;

namespace DnDFightTool.UI.CharacterSheetBlazorComponents.IoC;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterCharacterSheetBlazorComponentsServices(this IServiceCollection services)
    {
        services.AddScoped<ICharacterEditContext>(serviceProvider => serviceProvider.GetService<IGlobalEditContext>()!);
        services.AddScoped<IAttackEditContext>(serviceProvider => serviceProvider.GetService<IGlobalEditContext>()!);
        services.AddScoped<IGlobalEditContext, GlobalEditContext>();
        
        return services;
    }
}
