using System.Runtime.CompilerServices;
using UndoableMediator.Commands;

namespace Extensions.Mediator;

public static class CommandBaseExtensions
{
    /// <summary>
    ///     Watch out that this will clear the subCommand property
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static IEnumerable<CommandBase> PopAllSubCommands(this CommandBase command)
    {
        while (command.SubCommands.Count != 0)
        {
            yield return command.SubCommands.Pop();
        }
    }
}
