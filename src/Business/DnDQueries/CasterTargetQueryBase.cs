using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Queries;

namespace DnDFightTool.Business.DnDQueries;

public class CasterTargetQueryBase<T> : QueryBase<T>
{
    public CasterTargetQueryBase(Guid casterId, Guid targetId)
    {
        CasterId = casterId;
        TargetId = targetId;
    }

    public Guid CasterId { get; }
    public Guid TargetId { get; }

    public Character GetCaster(IFightContext fightContext) => fightContext.GetCharacterById(CasterId) ?? throw new InvalidOperationException($"Could not get caster for {GetType()}.");
    public Character GetTarget(IFightContext fightContext) => fightContext.GetCharacterById(TargetId) ?? throw new InvalidOperationException($"Could not get target for {GetType()}.");
}
