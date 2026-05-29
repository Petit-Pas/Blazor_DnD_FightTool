using DnDFightTool.Business.DnDActions.LogActions.WriteLog;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.HitPointActions.LooseTempHp;

/// <summary>
///     Orchestrator: dispatches <see cref="LooseTempHpAtomicCommand"/> (mutation) then logs the corrected amount.
///     Undo cascades to sub-commands via <c>base.UndoAsync</c>.
/// </summary>
public class LooseTempHpCommandHandler : CommandHandlerBase<LooseTempHpCommand>
{
    private readonly IFightContext _fightContext;

    public LooseTempHpCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public async override Task<ICommandResponse<NoResponse>> ExecuteAsync(LooseTempHpCommand command)
    {
        var fighter = _fightContext[command.TargetId] ?? throw new ArgumentException($"{typeof(LooseTempHpCommandHandler)} could not find target with id {command.TargetId}");

        var atomicCmd = new LooseTempHpAtomicCommand(command.TargetId, command.Amount);
        var atomicResponse = await _mediator.SendAsSubCommandAsync(atomicCmd, parentCommand: command);

        await LogTempHpLoss(fighter.Name, atomicResponse.Response, command);

        return CommandResponse.Success();
    }

    public override Task UndoAsync(LooseTempHpCommand command)
    {
        return base.UndoAsync(command);
    }

    public async override Task RedoAsync(LooseTempHpCommand command)
    {
        ClearSubCommands(command);
        await ExecuteAsync(command);
    }

    private async Task LogTempHpLoss(string fighterName, int correctedAmount, LooseTempHpCommand command)
    {
        await _mediator.SendAsSubCommandAsync(
            new WriteLogCommand($"[b]{fighterName}[/b] loses [b]{correctedAmount}[/b] temp HPs"),
            parentCommand: command);
    }
}
