using System.Net.NetworkInformation;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Saves;
using Memory.Hashes;
using UndoableMediator.Mediators;

namespace DnDFightTool.Domain.DnDEntities.Statuses;

/// <summary>
///     A template for an applicable status. Whether it is applied through an attack or a spell
/// </summary>
public class StatusTemplate : IHashable
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public StatusTemplate()
    {
        Name = "Default status name";
        Save = new SaveRollTemplate();
    }

    /// <summary>
    ///     A unique, non meaningful id for the status
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    ///    A meaningful name for the status
    /// </summary>
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
    ///    Tells whether the status should be applied or not
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    /// <param name="saveRoll"></param>
    /// <returns></returns>
    public bool ShouldBeApplied(Character caster, Character target, SaveRollResult? saveRoll)
    {
        if (IsAppliedAutomatically || (saveRoll?.IsSuccessful(caster, target) ?? false))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    ///     Whenever a status is applied by a spell that already has a saving throw, sometimes the status can be recovered from by succeeding the same saving than the spell used.
    ///     In those cases, the save is not defined in the status itself, but should be inherited when used.
    /// </summary>
    // TODO Not used for now, will see when spells are implemented
    //public bool InheritsSave { get; set; }
}
