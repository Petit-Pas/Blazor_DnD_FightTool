using DnDEntities.Characters;
using Fight;
using UndoableMediator.Queries;

namespace DnDQueries;

public class CasterQueryBase<T> : QueryBase<T>
{
    public CasterQueryBase(Guid casterId)
    {
        CasterId = casterId;
    }

    public Guid CasterId { get; }

    public Character GetCaster(IFightContext fightContext) => fightContext.GetCharacterById(CasterId) ?? throw new InvalidOperationException($"Could not get caster for {GetType()}.");
}
