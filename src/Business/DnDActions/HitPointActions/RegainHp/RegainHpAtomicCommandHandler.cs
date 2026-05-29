using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.HitPointActions.RegainHp;

public class RegainHpAtomicCommandHandler : CommandHandlerBase<RegainHpAtomicCommand, int>
{
    private readonly IFightContext _fightContext;

    public RegainHpAtomicCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public override Task<ICommandResponse<int>> ExecuteAsync(RegainHpAtomicCommand command)
    {
        var hitPoints = _fightContext[command.TargetId]?.HitPoints ?? throw new ArgumentException($"{typeof(RegainHpAtomicCommandHandler)} could not find target with id {command.TargetId}");

        var correctedAmount = command.Amount;

        hitPoints.CurrentHps += command.Amount;
        if (hitPoints.CurrentHps > hitPoints.MaxHps)
        {
            correctedAmount -= hitPoints.CurrentHps - hitPoints.MaxHps;
            hitPoints.CurrentHps = hitPoints.MaxHps;
        }

        command.CorrectedAmount = correctedAmount;
        _fightContext.NotifyFighterUpdated(command.TargetId);

        return Task.FromResult<ICommandResponse<int>>(CommandResponse.Success<int>(correctedAmount));
    }

    public override Task UndoAsync(RegainHpAtomicCommand command)
    {
        var hitPoints = _fightContext[command.TargetId]?.HitPoints ?? throw new ArgumentException($"{typeof(RegainHpAtomicCommandHandler)} could not find target with id {command.TargetId}");

        if (command.CorrectedAmount == null)
        {
            throw new InvalidOperationException($"Cannot undo a {command.GetType()} when it has not been executed yet.");
        }

        hitPoints.CurrentHps -= command.CorrectedAmount.Value;

        _fightContext.NotifyFighterUpdated(command.TargetId);
        return Task.CompletedTask;
    }

    public override Task RedoAsync(RegainHpAtomicCommand command)
    {
        return ExecuteAsync(command);
    }
}
