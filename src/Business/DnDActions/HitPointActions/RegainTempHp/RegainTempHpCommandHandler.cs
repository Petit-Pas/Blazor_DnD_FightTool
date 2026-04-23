using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.HitPointActions.RegainTempHp;

public class RegainTempHpCommandHandler : CommandHandlerBase<RegainTempHpCommand>
{
    private readonly IFightContext _fightContext;

    public RegainTempHpCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public override Task<ICommandResponse<NoResponse>> ExecuteAsync(RegainTempHpCommand command)
    {
        var hitPoints = _fightContext[command.TargetId]?.HitPoints ?? throw new ArgumentException($"{typeof(RegainTempHpCommandHandler)} could not find target with id {command.TargetId}");

        var expectedTotalCurrentHps = Math.Max(command.Amount, hitPoints.CurrentTempHps);
        command.CorrectedAmount = expectedTotalCurrentHps - hitPoints.CurrentTempHps;

        hitPoints.CurrentTempHps += command.CorrectedAmount.Value;

        _fightContext.NotifyFighterUpdated(command.TargetId);

        return Task.FromResult(CommandResponse.Success());
    }

    public async override Task UndoAsync(RegainTempHpCommand command)
    {
        await base.UndoAsync(command);

        var hitPoints = _fightContext[command.TargetId]?.HitPoints ?? throw new ArgumentException($"{typeof(RegainTempHpCommandHandler)} could not find target with id {command.TargetId}");
        
        if (command.CorrectedAmount == null)
        {
            throw new InvalidOperationException($"Cannot undo a {command.GetType()} when it has not been executed yet.");
        }

        hitPoints.CurrentTempHps -= command.CorrectedAmount.Value;

        _fightContext.NotifyFighterUpdated(command.TargetId);
    }

    public async override Task RedoAsync(RegainTempHpCommand command)
    {
        await ExecuteAsync(command);
    }
}
