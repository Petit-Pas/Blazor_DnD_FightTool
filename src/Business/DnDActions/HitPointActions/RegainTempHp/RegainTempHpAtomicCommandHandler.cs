using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.HitPointActions.RegainTempHp;

public class RegainTempHpAtomicCommandHandler : CommandHandlerBase<RegainTempHpAtomicCommand, int>
{
    private readonly IFightContext _fightContext;

    public RegainTempHpAtomicCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public override Task<ICommandResponse<int>> ExecuteAsync(RegainTempHpAtomicCommand command)
    {
        var hitPoints = _fightContext[command.TargetId]?.HitPoints ?? throw new ArgumentException($"{typeof(RegainTempHpAtomicCommandHandler)} could not find target with id {command.TargetId}");

        var expectedTotalCurrentHps = Math.Max(command.Amount, hitPoints.CurrentTempHps);
        var correctedAmount = expectedTotalCurrentHps - hitPoints.CurrentTempHps;

        command.CorrectedAmount = correctedAmount;
        hitPoints.CurrentTempHps += correctedAmount;

        _fightContext.NotifyFighterUpdated(command.TargetId);

        return Task.FromResult<ICommandResponse<int>>(CommandResponse.Success<int>(correctedAmount));
    }

    public override Task UndoAsync(RegainTempHpAtomicCommand command)
    {
        var hitPoints = _fightContext[command.TargetId]?.HitPoints ?? throw new ArgumentException($"{typeof(RegainTempHpAtomicCommandHandler)} could not find target with id {command.TargetId}");
        
        if (command.CorrectedAmount == null)
        {
            throw new InvalidOperationException($"Cannot undo a {command.GetType()} when it has not been executed yet.");
        }

        hitPoints.CurrentTempHps -= command.CorrectedAmount.Value;

        _fightContext.NotifyFighterUpdated(command.TargetId);
        return Task.CompletedTask;
    }

    public override Task RedoAsync(RegainTempHpAtomicCommand command)
    {
        return ExecuteAsync(command);
    }
}
