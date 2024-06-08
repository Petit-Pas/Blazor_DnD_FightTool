using Mapping;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace DnDFightTool.Domain.DnDEntities.MartialAttacks.Mapping;

internal static class MappingConfigurations
{
    internal static IServiceCollection RegisterMartialAttackMappingConfigurations(this IServiceCollection services)
    {
        TypeAdapterConfig<MartialAttackTemplate, MartialAttackTemplate>
            .NewConfig()
            .IgnoreWhenDuplicating(x => x.Id);

        return services;
    }
}
