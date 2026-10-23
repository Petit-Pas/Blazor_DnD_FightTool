using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace DnDFightTool.Domain.CharacterSheet.AbilityScores.Mapping;

internal static class MappingConfigurations
{
    internal static IServiceCollection RegisterAbilityScoresMappingConfigurations(this IServiceCollection services)
    {
        // AbilityScoresCollection is a List<AbilityScore> with an extra MasteryBonus property.
        // Mapster's default collection mapping only copies the list items, not extra
        // properties on the derived type, so MasteryBonus must be copied explicitly.
        TypeAdapterConfig<AbilityScoresCollection, AbilityScoresCollection>
            .NewConfig()
            .AfterMapping((src, dest) => dest.MasteryBonus = src.MasteryBonus);

        return services;
    }
}
