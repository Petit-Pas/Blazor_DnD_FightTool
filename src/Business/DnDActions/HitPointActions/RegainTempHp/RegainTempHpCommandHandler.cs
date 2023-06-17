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

    public override ICommandResponse<NoResponse> Execute(RegainTempHpCommand command)
    {
        var hitPoints = command.GetHitPoints(_fightContext) ?? throw new ArgumentException($"Could not get hitpoints for this {command.GetType()}");

        var expectedTotalCurrentHps = Math.Max(command.Amount, hitPoints.CurrentTempHps);
        command.CorrectedAmount = expectedTotalCurrentHps - hitPoints.CurrentTempHps;

        hitPoints.CurrentTempHps += command.CorrectedAmount.Value;

        return CommandResponse.Success();
    }

    public override void Undo(RegainTempHpCommand command)
    {
        base.Undo(command);

        var hitPoints = command.GetHitPoints(_fightContext) ?? throw new ArgumentException($"Could not get hitpoints for this {command.GetType()}");
        
        if (command.CorrectedAmount == null)
        {
            throw new InvalidOperationException($"Cannot undo a {command.GetType()} when it has not been executed yet.");
        }

        hitPoints.CurrentTempHps -= command.CorrectedAmount.Value;

    }

    public override void Redo(RegainTempHpCommand command)
    {
        base.Redo(command);

        Execute(command);
    }
}
