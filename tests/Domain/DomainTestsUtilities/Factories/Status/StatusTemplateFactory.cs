using DnDFightTool.Domain.DnDEntities.Saves;
using DnDFightTool.Domain.DnDEntities.Statuses;
using DomainTestsUtilities.Factories.Saves;

namespace DomainTestsUtilities.Factories.Status;

public static class StatusTemplateFactory
{
    public static StatusTemplate Build(
        string? name = null,
        bool? appliedAutomatically = null,
        SaveRollTemplate saveRollTemplate = null!,
        Guid? id = null
        )
    {
        return new StatusTemplate()
        {
            Name = name ?? "Status template",
            IsAppliedAutomatically = appliedAutomatically ?? false,
            Save = saveRollTemplate ?? SaveRollTemplateFactory.Build(),
            Id = id ?? Guid.NewGuid(),
        };
    }

    public static StatusTemplateCollection BuildCollection(StatusTemplate[] ? statusTemplates = null)
    {
        var statusTemplateCollection = new StatusTemplateCollection();
        if (statusTemplates != null)
        {
            statusTemplateCollection.AddRange(statusTemplates);
        }
        else
        {
            statusTemplateCollection.Add(Build());
        }

        return statusTemplateCollection;
    }
}
