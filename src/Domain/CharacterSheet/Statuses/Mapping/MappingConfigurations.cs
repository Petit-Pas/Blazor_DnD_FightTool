using DnDFightTool.Infrastructure.Mapping;
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

        // Map each template individually so IgnoreWhenDuplicating regenerates the Id,
        // then rebuild the dictionary keyed by the mapped template's Id.
        TypeAdapterConfig<StatusTemplateCollection, StatusTemplateCollection>
            .NewConfig()
            .AfterMapping((src, dest) =>
            {
                dest.Clear();
                foreach (var srcTemplate in src.Values)
                {
                    dest.Add(srcTemplate.Adapt<StatusTemplate>());
                }
            });

        return services;
    }
}
