using DnDFightTool.Business.DnDActions.HitPointActions.LooseHp;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.HitPointActions.LooseTempHp;

public class LooseTempHpCommandHandler : CommandHandlerBase<LooseTempHpCommand>
{
    private readonly IFightContext _fightContext;


    public LooseTempHpCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public override Task<ICommandResponse<NoResponse>> ExecuteAsync(LooseTempHpCommand command)
    {
        var hitPoints = _fightContext[command.TargetId]?.HitPoints ?? throw new ArgumentException($"{typeof(LooseTempHpCommandHandler)} could not find target with id {command.TargetId}");

        command.CorrectedAmount = command.Amount;

        hitPoints.CurrentTempHps -= command.Amount;
        if (hitPoints.CurrentTempHps < 0)
        {
            command.CorrectedAmount = command.Amount + hitPoints.CurrentTempHps;
            hitPoints.CurrentTempHps = 0;
        }

        _fightContext.NotifyFighterUpdated(command.TargetId);

        return Task.FromResult(CommandResponse.Success());
    }

    public override Task UndoAsync(LooseTempHpCommand command)
    {
        var hitPoints = _fightContext[command.TargetId]?.HitPoints ?? throw new ArgumentException($"{typeof(LooseTempHpCommandHandler)} could not find target with id {command.TargetId}");

        if (command.CorrectedAmount == null)
        {
            throw new InvalidOperationException($"Cannot undo a {command.GetType()} when it has not been executed yet.");
        }

        hitPoints.CurrentTempHps += command.CorrectedAmount.Value;

        _fightContext.NotifyFighterUpdated(command.TargetId);

        return Task.CompletedTask;
    }

    public async override Task RedoAsync(LooseTempHpCommand command)
    {
        await ExecuteAsync(command);
    }
}
