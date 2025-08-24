using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using UndoableMediator.Queries;

namespace DnDFightTool.Business.DnDQueries;

/// <summary>
///     base class for queries where a caster affect a target
/// </summary>
/// <typeparam name="T"> type of the response of the query </typeparam>
public class CasterTargetQueryBase<T> : QueryBase<T>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="casterId"></param>
    /// <param name="targetId"></param>
    public CasterTargetQueryBase(Guid casterId, Guid targetId)
    {
        CasterId = casterId;
        TargetId = targetId;
    }

    /// <summary>
    ///     Id of the caster
    /// </summary>
    public Guid CasterId { get; }

    /// <summary>
    ///     Id of the target
    /// </summary>
    public Guid TargetId { get; }

    /// <summary>
    ///     Helper method for the handlers to get the caster
    /// </summary>
    /// <param name="fightContext"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public FightingCharacter GetCaster(IFightContext fightContext)
    {
        return fightContext[CasterId];
    }

    /// <summary>
    ///     Helper method for the handlers to get the target
    /// </summary>
    /// <param name="fightContext"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public FightingCharacter GetTarget(IFightContext fightContext)
    {
        return fightContext[TargetId];
    }
}
