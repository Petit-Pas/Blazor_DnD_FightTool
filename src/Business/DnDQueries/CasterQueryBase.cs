using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using UndoableMediator.Queries;

namespace DnDFightTool.Business.DnDQueries;

/// <summary>
///     base class for queries that have a caster
/// </summary>
/// <typeparam name="T"></typeparam>
public class CasterQueryBase<T> : QueryBase<T>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="casterId"></param>
    public CasterQueryBase(Guid casterId)
    {
        CasterId = casterId;
    }

    /// <summary>
    ///     Id of the caster
    /// </summary>
    public Guid CasterId { get; }

    /// <summary>
    ///     Helper method for the handlers to get the caster easily
    /// </summary>
    /// <param name="fightContext"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public FightingCharacter GetCaster(IFightContext fightContext)
    {
        return fightContext[CasterId];
    }
}
