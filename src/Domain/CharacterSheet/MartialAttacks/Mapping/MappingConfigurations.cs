using DnDFightTool.Infrastructure.Mapping;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace DnDFightTool.Domain.CharacterSheet.MartialAttacks.Mapping;

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
