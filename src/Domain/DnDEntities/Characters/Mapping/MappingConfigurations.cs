using Mapping;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace DnDFightTool.Domain.DnDEntities.Characters.Mapping;

internal static class MappingConfigurations
{
    internal static IServiceCollection RegisterCharacterMappingConfigurations(this IServiceCollection services)
    {
        TypeAdapterConfig<Character, Character>
            .NewConfig()
            .IgnoreWhenDuplicating(x => x.Id);

        return services;
    }
}
