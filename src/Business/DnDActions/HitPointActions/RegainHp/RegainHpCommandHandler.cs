using DnDFightTool.Business.DnDActions.LogActions.WriteLog;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.HitPointActions.RegainHp;

/// <summary>
///     Orchestrator: dispatches <see cref="RegainHpAtomicCommand"/> (mutation) then logs the corrected amount.
///     Undo cascades to sub-commands via <c>base.UndoAsync</c>.
/// </summary>
public class RegainHpCommandHandler : CommandHandlerBase<RegainHpCommand>
{
    private readonly IFightContext _fightContext;

    public RegainHpCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public async override Task<ICommandResponse<NoResponse>> ExecuteAsync(RegainHpCommand command)
    {
        var fighter = _fightContext[command.TargetId] ?? throw new ArgumentException($"{typeof(RegainHpCommandHandler)} could not find target with id {command.TargetId}");

        var atomicCmd = new RegainHpAtomicCommand(command.TargetId, command.Amount);
        var atomicResponse = await _mediator.SendAsSubCommandAsync(atomicCmd, parentCommand: command);

        await LogHpGain(fighter.Name, atomicResponse.Response, command);

        return CommandResponse.Success();
    }

    public override Task UndoAsync(RegainHpCommand command)
    {
        return base.UndoAsync(command);
    }

    public async override Task RedoAsync(RegainHpCommand command)
    {
        ClearSubCommands(command);
        await ExecuteAsync(command);
    }

    private async Task LogHpGain(string fighterName, int correctedAmount, RegainHpCommand command)
    {
        await _mediator.SendAsSubCommandAsync(
            new WriteLogCommand($"[b]{fighterName}[/b] regains [c:heal][b]{correctedAmount}[/b] HPs[/c]"),
            parentCommand: command);
    }
}
