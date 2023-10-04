using DnDFightTool.Domain.DnDEntities.Saves;

namespace DnDFightTool.Domain.DnDEntities.Statuses;

public class StatusTemplate
{
    public StatusTemplate()
    {
        Name = "Default status name";
        Save = new SaveRollTemplate();
    }

    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     If set to true, the Saving can be ignored
    /// </summary>
    public bool IsAppliedAutomatically { get; set; } = false;

    /// <summary>
    ///     Can be:
    ///         - ignored if IsAppliedAutomatically is true
    ///         - set when defining the Status
    ///         - inherited from a spell that applied the status if InheritsSave is true
    /// </summary>
    public SaveRollTemplate Save { get; set; }

    /// <summary>
    ///     Whenever a status is applied by a spell that already has a saving throw, sometimes the status can be recovered from by succeeding the same saving than the spell used.
    ///     In those cases, the save is not defined in the satuts itself, but should be inherited when used.
    /// </summary>
    // Not used for now
    //public bool InheritsSave { get; set; }
}
