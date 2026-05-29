using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.HitPointActions.LooseTempHp;

public class LooseTempHpAtomicCommandHandler : CommandHandlerBase<LooseTempHpAtomicCommand, int>
{
    private readonly IFightContext _fightContext;

    public LooseTempHpAtomicCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public override Task<ICommandResponse<int>> ExecuteAsync(LooseTempHpAtomicCommand command)
    {
        var hitPoints = _fightContext[command.TargetId]?.HitPoints ?? throw new ArgumentException($"{typeof(LooseTempHpAtomicCommandHandler)} could not find target with id {command.TargetId}");

        var correctedAmount = command.Amount;

        hitPoints.CurrentTempHps -= command.Amount;
        if (hitPoints.CurrentTempHps < 0)
        {
            correctedAmount = command.Amount + hitPoints.CurrentTempHps;
            hitPoints.CurrentTempHps = 0;
        }

        command.CorrectedAmount = correctedAmount;
        _fightContext.NotifyFighterUpdated(command.TargetId);

        return Task.FromResult<ICommandResponse<int>>(CommandResponse.Success<int>(correctedAmount));
    }

    public override Task UndoAsync(LooseTempHpAtomicCommand command)
    {
        var hitPoints = _fightContext[command.TargetId]?.HitPoints ?? throw new ArgumentException($"{typeof(LooseTempHpAtomicCommandHandler)} could not find target with id {command.TargetId}");

        if (command.CorrectedAmount == null)
        {
            throw new InvalidOperationException($"Cannot undo a {command.GetType()} when it has not been executed yet.");
        }

        hitPoints.CurrentTempHps += command.CorrectedAmount.Value;

        _fightContext.NotifyFighterUpdated(command.TargetId);
        return Task.CompletedTask;
    }

    public override Task RedoAsync(LooseTempHpAtomicCommand command)
    {
        return ExecuteAsync(command);
    }
}
