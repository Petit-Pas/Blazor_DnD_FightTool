using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace DnDFightTool.Domain.DnDEntities.Skills.Mapping;

internal static class MappingConfigurations
{
    internal static IServiceCollection RegisterSkillMappingConfigurations(this IServiceCollection services)
    {
        TypeAdapterConfig<Skill, Skill>
            .NewConfig()
            .ConstructUsing(x => new Skill(x.Name))
            .Ignore(x => x.Name);

        return services;
    }
}
