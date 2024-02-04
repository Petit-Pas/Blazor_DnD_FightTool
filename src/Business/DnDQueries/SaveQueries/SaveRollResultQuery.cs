using DnDFightTool.Domain.DnDEntities.Saves;

namespace DnDFightTool.Business.DnDQueries.SaveQueries;

/// <summary>
///    Query to get the result of a save roll
/// </summary>
public class SaveRollResultQuery : CasterTargetQueryBase<SaveRollResult>
{
    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="casterId"></param>
    /// <param name="targetId"></param>
    /// <param name="save"></param>
    public SaveRollResultQuery(Guid casterId, Guid targetId, SaveRollTemplate save) : base(casterId, targetId)
    {
        Save = save;
    }

    /// <summary>
    ///     The template of the save to roll
    /// </summary>
    public SaveRollTemplate Save { get; }
}
