using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace DnDFightTool.Domain.CharacterSheet.Dices.Mapping;

internal static class MappingConfigurations
{
    internal static IServiceCollection RegisterDiceMappingConfigurations(this IServiceCollection services)
    {
        // The parsed state lives in private fields, so Mapster would otherwise reuse the source instance.
        TypeAdapterConfig<DiceRollTemplate, DiceRollTemplate>
            .NewConfig()
            .MapWith(src => new DiceRollTemplate(src));

        TypeAdapterConfig<DiceRollModifiersTemplate, DiceRollModifiersTemplate>
            .NewConfig()
            .MapWith(src => new DiceRollModifiersTemplate(src.Expression));

        return services;
    }
}
