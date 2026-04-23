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

    public override Task<ICommandResponse<NoResponse>> ExecuteAsync(LooseHpCommand command)
    {
        var hitPoints = _fightContext[command.TargetId]?.HitPoints ?? throw new ArgumentException($"{typeof(LooseHpCommandHandler)} could not find target with id {command.TargetId}");

        command.CorrectedAmount = command.Amount;
        
        hitPoints.CurrentHps -= command.Amount;
        if (hitPoints.CurrentHps < 0)
        {
            command.CorrectedAmount = command.Amount + hitPoints.CurrentHps;
            hitPoints.CurrentHps = 0;
        }

        _fightContext.NotifyFighterUpdated(command.TargetId);

        return Task.FromResult(CommandResponse.Success());
    }

    public override Task UndoAsync(LooseHpCommand command)
    {
        var hitPoints = _fightContext[command.TargetId]?.HitPoints ?? throw new ArgumentException($"{typeof(LooseHpCommandHandler)} could not find target with id {command.TargetId}");

        if (command.CorrectedAmount == null)
        {
            throw new InvalidOperationException($"Cannot undo a {command.GetType()} when it has not been executed yet.");
        }

        hitPoints.CurrentHps += command.CorrectedAmount.Value;

        _fightContext.NotifyFighterUpdated(command.TargetId);

        return Task.CompletedTask;
    }

    public async override Task RedoAsync(LooseHpCommand command)
    {
        await ExecuteAsync(command);
    }
}
