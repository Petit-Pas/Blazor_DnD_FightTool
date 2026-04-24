using DnDFightTool.Business.DnDActions.LogActions.WriteLog;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.HitPointActions.RegainHp;

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
        var hitPoints = fighter.HitPoints;

        command.CorrectedAmount = command.Amount;

        hitPoints.CurrentHps += command.Amount;
        if (hitPoints.CurrentHps > hitPoints.MaxHps)
        {
            command.CorrectedAmount -= hitPoints.CurrentHps - hitPoints.MaxHps;
            hitPoints.CurrentHps = hitPoints.MaxHps;
        }

        _fightContext.NotifyFighterUpdated(command.TargetId);

        await LogHpGain(fighter.Name, command);

        return CommandResponse.Success();
    }

    public async override Task UndoAsync(RegainHpCommand command)
    {
        await base.UndoAsync(command);

        var hitPoints = _fightContext[command.TargetId]?.HitPoints ?? throw new ArgumentException($"{typeof(RegainHpCommandHandler)} could not find target with id {command.TargetId}");

        if (command.CorrectedAmount == null)
        {
            throw new InvalidOperationException($"Cannot undo a {command.GetType()} when it has not been executed yet.");
        }

        hitPoints.CurrentHps -= command.CorrectedAmount.Value;

        _fightContext.NotifyFighterUpdated(command.TargetId);
    }

    public async override Task RedoAsync(RegainHpCommand command)
    {
        await ExecuteAsync(command);
    }

    private async Task LogHpGain(string fighterName, RegainHpCommand command)
    {
        await _mediator.SendAsSubCommandAsync(
            new WriteLogCommand($"[b]{fighterName}[/b] regains [c:heal][b]{command.CorrectedAmount}[/b] HPs[/c]"),
            parentCommand: command);
    }
        
}
