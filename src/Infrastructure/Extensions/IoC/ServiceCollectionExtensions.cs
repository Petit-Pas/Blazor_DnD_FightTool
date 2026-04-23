using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DnDFightTool.Infrastructure.Extensions.IoC;

/// <summary>
///     IoC registration helpers for the Extensions infrastructure project.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Scans <paramref name="assembly"/> and registers every non-abstract, non-interface
    ///     subclass of <see cref="PropertyTargetedValidator{T}"/> as a transient service bound
    ///     to its open-generic base type.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="assembly">The assembly to scan for validator implementations.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection RegisterPropertyTargetedValidators(this IServiceCollection services, Assembly assembly)
    {
        var openGenericType = typeof(PropertyTargetedValidator<>);

        var types = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Select(t => new
            {
                Type = t,
                Base = t.BaseType
            })
            .Where(x => x.Base != null
                && x.Base.IsGenericType
                && x.Base.GetGenericTypeDefinition() == openGenericType)
            .ToList();

        foreach (var x in types)
        {
            services.AddTransient(x.Base!, x.Type);
        }

        return services;
    }
}
