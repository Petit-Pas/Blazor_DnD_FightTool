using Mapping;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace DnDFightTool.Domain.CharacterSheet.Statuses.Mapping;

internal static class MappingConfigurations
{
    internal static IServiceCollection RegisterStatusMappingConfigurations(this IServiceCollection services)
    {
        TypeAdapterConfig<StatusTemplate, StatusTemplate>
            .NewConfig()
            .IgnoreWhenDuplicating(x => x.Id);

        return services;
    }
}
