using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Queries;

namespace DnDFightTool.Business.DnDQueries;

public class CasterQueryBase<T> : QueryBase<T>
{
    public CasterQueryBase(Guid casterId)
    {
        CasterId = casterId;
    }

    public Guid CasterId { get; }

    public Character GetCaster(IFightContext fightContext) => fightContext.GetCharacterById(CasterId) ?? throw new InvalidOperationException($"Could not get caster for {GetType()}.");
}
