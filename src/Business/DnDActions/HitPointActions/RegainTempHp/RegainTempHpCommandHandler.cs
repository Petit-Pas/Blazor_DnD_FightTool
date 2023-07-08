using Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDActions.HitPointActions.RegainTempHp;

public class RegainTempHpCommandHandler : CommandHandlerBase<RegainTempHpCommand>
{
    private readonly IFightContext _fightContext;

    public RegainTempHpCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public override Task<ICommandResponse<NoResponse>> Execute(RegainTempHpCommand command)
    {
        var hitPoints = command.GetHitPoints(_fightContext);

        var expectedTotalCurrentHps = Math.Max(command.Amount, hitPoints.CurrentTempHps);
        command.CorrectedAmount = expectedTotalCurrentHps - hitPoints.CurrentTempHps;

        hitPoints.CurrentTempHps += command.CorrectedAmount.Value;

        return Task.FromResult(CommandResponse.Success());
    }

    public override void Undo(RegainTempHpCommand command)
    {
        base.Undo(command);

        var hitPoints = command.GetHitPoints(_fightContext);
        
        if (command.CorrectedAmount == null)
        {
            throw new InvalidOperationException($"Cannot undo a {command.GetType()} when it has not been executed yet.");
        }

        hitPoints.CurrentTempHps -= command.CorrectedAmount.Value;

    }

    public override async Task Redo(RegainTempHpCommand command)
    {
        await Execute(command);
    }
}
