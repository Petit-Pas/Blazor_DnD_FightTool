using DnDFightTool.Infrastructure.Mapping;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace DnDFightTool.Domain.Fight.Mapping;

internal static class MappingConfigurations
{
    internal static IServiceCollection RegisterAppliedStatusMappingConfigurations(this IServiceCollection services)
    {
        TypeAdapterConfig<AppliedStatus, AppliedStatus>
            .NewConfig()
            .IgnoreWhenDuplicating(x => x.Id);

        return services;
    }
}
