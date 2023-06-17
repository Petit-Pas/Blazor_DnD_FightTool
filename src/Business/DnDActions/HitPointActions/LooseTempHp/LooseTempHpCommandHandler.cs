using Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDActions.HitPointActions.LooseTempHp;

public class LooseTempHpCommandHandler : CommandHandlerBase<LooseTempHpCommand>
{
    private readonly IFightContext _fightContext;


    public LooseTempHpCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public override ICommandResponse<NoResponse> Execute(LooseTempHpCommand command)
    {
        var hitPoints = command.GetHitPoints(_fightContext) ?? throw new ArgumentException($"Could not get hitpoints for this {command.GetType()}");

        command.CorrectedAmount = command.Amount;

        hitPoints.CurrentTempHps -= command.Amount;
        if (hitPoints.CurrentTempHps < 0)
        {
            command.CorrectedAmount = command.Amount + hitPoints.CurrentTempHps;
            hitPoints.CurrentTempHps = 0;
        }

        return CommandResponse.Success();
    }

    public override void Undo(LooseTempHpCommand command)
    {
        base.Undo(command);

        var hitPoints = command.GetHitPoints(_fightContext) ?? throw new ArgumentException($"Could not get hitpoints for this {command.GetType()}");

        if (command.CorrectedAmount == null)
        {
            throw new InvalidOperationException($"Cannot undo a {command.GetType()} when it has not been executed yet.");
        }

        hitPoints.CurrentTempHps += command.CorrectedAmount.Value;
    }

    public override void Redo(LooseTempHpCommand command)
    {
        base.Redo(command);

        Execute(command);
    }
}
