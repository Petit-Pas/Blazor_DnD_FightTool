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

        // Map each template individually (preserving MapContext for IgnoreWhenDuplicating),
        // then rebuild the dictionary keyed by the mapped template's Id.
        TypeAdapterConfig<MartialAttackTemplateCollection, MartialAttackTemplateCollection>
            .NewConfig()
            .AfterMapping((src, dest) =>
            {
                dest.Clear();
                foreach (var srcTemplate in src.Values)
                {
                    dest.Add(srcTemplate.Adapt<MartialAttackTemplate>());
                }
            });

        return services;
    }
}
