using DnDFightTool.Domain.CharacterSheet.Characters.Mapping;
using DnDFightTool.Domain.CharacterSheet.MartialAttacks.Mapping;
using DnDFightTool.Domain.CharacterSheet.Skills.Mapping;
using DnDFightTool.Domain.CharacterSheet.Statuses.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace DnDFightTool.Domain.CharacterSheet.IoC;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterCharacterSheetMappingConfigurations(this IServiceCollection services)
    {
        services.RegisterSkillMappingConfigurations();
        services.RegisterCharacterMappingConfigurations();
        services.RegisterMartialAttackMappingConfigurations();
        services.RegisterStatusMappingConfigurations();

        return services;
    }
}
