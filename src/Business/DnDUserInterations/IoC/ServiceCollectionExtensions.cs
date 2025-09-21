using Microsoft.Extensions.DependencyInjection;

namespace DnDFightTool.Business.DnDUserInteraction.IoC;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterDnDUserInteractionServices(this IServiceCollection services)
    {
        services.AddSingleton<IUserInteractionService, UserInteractionService>();
        return services;
    }
}
