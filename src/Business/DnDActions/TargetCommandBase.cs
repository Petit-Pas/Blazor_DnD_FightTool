using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions;

/// <summary>
///    Base class for commands that affect a target
/// </summary>
public class TargetCommandBase : CommandBase
{
    public TargetCommandBase(Guid targetId)
    {
        TargetId = targetId;
    }

    /// <summary>
    ///     Guid of the character affected by the command
    /// </summary>
    public Guid TargetId { get; }
}

/// <summary>
///    Base class for commands that affect a target and return a typed value
/// </summary>
/// <typeparam name="T">The type of value returned by the command</typeparam>
public class TargetCommandBase<T> : CommandBase<T>
{
    public TargetCommandBase(Guid targetId)
    {
        TargetId = targetId;
    }

    /// <summary>
    ///     Guid of the character affected by the command
    /// </summary>
    public Guid TargetId { get; }
}