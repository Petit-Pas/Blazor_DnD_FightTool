using DnDFightTool.Business.DnDActions.LogActions.WriteLog;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.HitPointActions.LooseHp;

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
        var hitPoints = fighter.HitPoints;

        command.CorrectedAmount = command.Amount;
        
        hitPoints.CurrentHps -= command.Amount;
        if (hitPoints.CurrentHps < 0)
        {
            command.CorrectedAmount = command.Amount + hitPoints.CurrentHps;
            hitPoints.CurrentHps = 0;
        }

        _fightContext.NotifyFighterUpdated(command.TargetId);

        await LogHpLoss(fighter.Name, command);

        return CommandResponse.Success();
    }

    public async override Task UndoAsync(LooseHpCommand command)
    {
        await base.UndoAsync(command);

        var hitPoints = _fightContext[command.TargetId]?.HitPoints ?? throw new ArgumentException($"{typeof(LooseHpCommandHandler)} could not find target with id {command.TargetId}");

        if (command.CorrectedAmount == null)
        {
            throw new InvalidOperationException($"Cannot undo a {command.GetType()} when it has not been executed yet.");
        }

        hitPoints.CurrentHps += command.CorrectedAmount.Value;

        _fightContext.NotifyFighterUpdated(command.TargetId);
    }

    public async override Task RedoAsync(LooseHpCommand command)
    {
        await ExecuteAsync(command);
    }

    private async Task LogHpLoss(string fighterName, LooseHpCommand command)
    {
        await _mediator.SendAsSubCommandAsync(
            new WriteLogCommand($"[b]{fighterName}[/b] loses [b]{command.CorrectedAmount}[/b] HPs"),
            parentCommand: command);
    }
}
