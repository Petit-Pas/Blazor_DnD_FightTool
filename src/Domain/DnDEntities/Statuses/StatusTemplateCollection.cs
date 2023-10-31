using Memory.Hashes;

namespace DnDFightTool.Domain.DnDEntities.Statuses;

public class StatusTemplateCollection : List<StatusTemplate>, IHashable
{
    public StatusTemplateCollection() : this(false)
    {
    }

    public StatusTemplateCollection(bool withDefault)
    {
        if (withDefault)
        {
            this.Add(new StatusTemplate()
            {
                Name = "TestStatus"
            });
        }
    }
}
