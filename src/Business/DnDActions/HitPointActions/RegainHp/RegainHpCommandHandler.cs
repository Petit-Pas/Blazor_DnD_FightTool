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

    public override Task<ICommandResponse<NoResponse>> ExecuteAsync(RegainHpCommand command)
    {
        var hitPoints = _fightContext[command.TargetId]?.HitPoints ?? throw new ArgumentException($"{typeof(RegainHpCommandHandler)} could not find target with id {command.TargetId}");

        command.CorrectedAmount = command.Amount;

        hitPoints.CurrentHps += command.Amount;
        if (hitPoints.CurrentHps > hitPoints.MaxHps)
        {
            command.CorrectedAmount -= hitPoints.CurrentHps - hitPoints.MaxHps;
            hitPoints.CurrentHps = hitPoints.MaxHps;
        }

        _fightContext.NotifyFighterUpdated(command.TargetId);

        return Task.FromResult(CommandResponse.Success());
    }

    public override Task UndoAsync(RegainHpCommand command)
    {
        var hitPoints = _fightContext[command.TargetId]?.HitPoints ?? throw new ArgumentException($"{typeof(RegainHpCommandHandler)} could not find target with id {command.TargetId}");

        if (command.CorrectedAmount == null)
        {
            throw new InvalidOperationException($"Cannot undo a {command.GetType()} when it has not been executed yet.");
        }

        hitPoints.CurrentHps -= command.CorrectedAmount.Value;

        _fightContext.NotifyFighterUpdated(command.TargetId);

        return Task.CompletedTask;
    }

    public async override Task RedoAsync(RegainHpCommand command)
    {
        await ExecuteAsync(command);
    }
}
