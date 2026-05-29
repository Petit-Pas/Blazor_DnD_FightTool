using DnDFightTool.Business.DnDActions.LogActions.WriteLog;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.HitPointActions.RegainTempHp;

/// <summary>
///     Orchestrator: dispatches <see cref="RegainTempHpAtomicCommand"/> (mutation) then logs the corrected amount.
///     Undo cascades to sub-commands via <c>base.UndoAsync</c>.
/// </summary>
public class RegainTempHpCommandHandler : CommandHandlerBase<RegainTempHpCommand>
{
    private readonly IFightContext _fightContext;

    public RegainTempHpCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public async override Task<ICommandResponse<NoResponse>> ExecuteAsync(RegainTempHpCommand command)
    {
        var fighter = _fightContext[command.TargetId] ?? throw new ArgumentException($"{typeof(RegainTempHpCommandHandler)} could not find target with id {command.TargetId}");

        var atomicCmd = new RegainTempHpAtomicCommand(command.TargetId, command.Amount);
        var atomicResponse = await _mediator.SendAsSubCommandAsync(atomicCmd, parentCommand: command);

        await LogTempHpGain(fighter.Name, atomicResponse.Response, command);

        return CommandResponse.Success();
    }

    public override Task UndoAsync(RegainTempHpCommand command)
    {
        return base.UndoAsync(command);
    }

    public async override Task RedoAsync(RegainTempHpCommand command)
    {
        ClearSubCommands(command);
        await ExecuteAsync(command);
    }

    private async Task LogTempHpGain(string fighterName, int correctedAmount, RegainTempHpCommand command)
    {
        await _mediator.SendAsSubCommandAsync(
            new WriteLogCommand($"[b]{fighterName}[/b] gains [c:heal][b]{correctedAmount}[/b] temp HPs[/c]"),
            parentCommand: command);
    }
}
