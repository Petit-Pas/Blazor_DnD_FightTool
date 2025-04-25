using Memory.Hashes;

namespace DnDFightTool.Domain.DnDEntities.Statuses;

public class StatusTemplateCollection : Dictionary<Guid, StatusTemplate>, IHashable
{
    /// <summary>
    ///     Empty ctor
    /// </summary>
    public StatusTemplateCollection() : this(false)
    {
    }

    /// <summary>
    ///     Ctor that allows to ask for a default template
    /// </summary>
    /// <param name="withDefault"></param>
    public StatusTemplateCollection(bool withDefault)
    {
        if (withDefault)
        {
            Add(new StatusTemplate("TestStatus"));
        }
    }

    /// <summary>
    ///     Helper method to add a new template to the collection
    /// </summary>
    /// <param name="template"></param>
    public void Add(StatusTemplate template)
    {
        Add(template.Id, template);
    }

    /// <summary>
    ///     Bulk insert status templates
    /// </summary>
    /// <param name="templates"></param>
    public void AddRange(IEnumerable<StatusTemplate> templates)
    {
        foreach (var template in templates)
        {
            Add(template);
        }
    }
}
