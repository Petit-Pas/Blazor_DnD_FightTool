using DnDFightTool.Business.DnDActions.LogActions.WriteLog;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.HitPointActions.LooseHp;

/// <summary>
///     Orchestrator: dispatches <see cref="LooseHpAtomicCommand"/> (mutation) then logs the corrected amount.
///     Undo cascades to sub-commands via <c>base.UndoAsync</c>.
/// </summary>
public class LooseHpCommandHandler : CommandHandlerBase<LooseHpCommand>
{
    private readonly IFightContext _fightContext;

    public LooseHpCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public async override Task<ICommandResponse<NoResponse>> ExecuteAsync(LooseHpCommand command)
    {
        var fighter = _fightContext[command.TargetId] ?? throw new ArgumentException($"{typeof(LooseHpCommandHandler)} could not find target with id {command.TargetId}");

        var atomicCmd = new LooseHpAtomicCommand(command.TargetId, command.Amount);
        var atomicResponse = await _mediator.SendAsSubCommandAsync(atomicCmd, parentCommand: command);

        await LogHpLoss(fighter.Name, atomicResponse.Response, command);

        return CommandResponse.Success();
    }

    public override Task UndoAsync(LooseHpCommand command)
    {
        return base.UndoAsync(command);
    }

    public async override Task RedoAsync(LooseHpCommand command)
    {
        ClearSubCommands(command);
        await ExecuteAsync(command);
    }

    private async Task LogHpLoss(string fighterName, int correctedAmount, LooseHpCommand command)
    {
        await _mediator.SendAsSubCommandAsync(
            new WriteLogCommand($"[b]{fighterName}[/b] loses [b]{correctedAmount}[/b] HPs"),
            parentCommand: command);
    }
}
