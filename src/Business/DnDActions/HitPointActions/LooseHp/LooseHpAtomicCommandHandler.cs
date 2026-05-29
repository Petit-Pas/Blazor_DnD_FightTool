using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.HitPointActions.LooseHp;

public class LooseHpAtomicCommandHandler : CommandHandlerBase<LooseHpAtomicCommand, int>
{
    private readonly IFightContext _fightContext;

    public LooseHpAtomicCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public override Task<ICommandResponse<int>> ExecuteAsync(LooseHpAtomicCommand command)
    {
        var hitPoints = _fightContext[command.TargetId]?.HitPoints ?? throw new ArgumentException($"{typeof(LooseHpAtomicCommandHandler)} could not find target with id {command.TargetId}");

        var correctedAmount = command.Amount;

        hitPoints.CurrentHps -= command.Amount;
        if (hitPoints.CurrentHps < 0)
        {
            correctedAmount = command.Amount + hitPoints.CurrentHps;
            hitPoints.CurrentHps = 0;
        }

        command.CorrectedAmount = correctedAmount;
        _fightContext.NotifyFighterUpdated(command.TargetId);

        return Task.FromResult<ICommandResponse<int>>(CommandResponse.Success<int>(correctedAmount));
    }

    public override Task UndoAsync(LooseHpAtomicCommand command)
    {
        var hitPoints = _fightContext[command.TargetId]?.HitPoints ?? throw new ArgumentException($"{typeof(LooseHpAtomicCommandHandler)} could not find target with id {command.TargetId}");

        if (command.CorrectedAmount == null)
        {
            throw new InvalidOperationException($"Cannot undo a {command.GetType()} when it has not been executed yet.");
        }

        hitPoints.CurrentHps += command.CorrectedAmount.Value;

        _fightContext.NotifyFighterUpdated(command.TargetId);
        return Task.CompletedTask;
    }

    public override Task RedoAsync(LooseHpAtomicCommand command)
    {
        return ExecuteAsync(command);
    }
}
