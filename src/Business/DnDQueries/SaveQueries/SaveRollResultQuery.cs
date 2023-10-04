using DnDFightTool.Domain.DnDEntities.Saves;

namespace DnDFightTool.Business.DnDQueries.SaveQueries;

public class SaveRollResultQuery : CasterTargetQueryBase<SaveRollResult>
{
    public SaveRollResultQuery(Guid casterId, Guid targetId, SaveRollTemplate save) : base(casterId, targetId)
    {
        Save = save;
    }

    public SaveRollTemplate Save { get; }
}
