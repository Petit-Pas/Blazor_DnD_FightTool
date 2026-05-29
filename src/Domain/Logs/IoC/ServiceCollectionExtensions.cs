using Microsoft.Extensions.DependencyInjection;

namespace DnDFightTool.Domain.Logs.IoC;

/// <summary>
///     Dependency injection registration for the Logs domain project.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Registers <see cref="IDnDLogService"/> as a singleton.
    /// </summary>
    public static IServiceCollection RegisterLogsServices(this IServiceCollection services)
    {
        services.AddSingleton<IDnDLogService, DnDLogService>();

        return services;
    }
}
