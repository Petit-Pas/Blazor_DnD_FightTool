using DnDFightTool.Domain.DnDEntities.Characters.Mapping;
using DnDFightTool.Domain.DnDEntities.MartialAttacks.Mapping;
using DnDFightTool.Domain.DnDEntities.Skills.Mapping;
using DnDFightTool.Domain.DnDEntities.Statuses.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace DnDFightTool.Domain.DnDEntities.IoC;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterDnDEntitiesMappingConfigurations(this IServiceCollection services)
    {
        services.RegisterSkillMappingConfigurations();
        services.RegisterCharacterMappingConfigurations();
        services.RegisterMartialAttackMappingConfigurations();
        services.RegisterStatusMappingConfigurations();

        return services;
    }
}
