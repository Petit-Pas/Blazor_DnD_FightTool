using Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDActions.HitPointActions.LooseHp;

public class LooseHpCommandHandler : CommandHandlerBase<LooseHpCommand>
{
    private readonly IFightContext _fightContext;

    public LooseHpCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public override Task<ICommandResponse<NoResponse>> Execute(LooseHpCommand command)
    {
        var hitPoints = command.GetHitPoints(_fightContext);

        command.CorrectedAmount = command.Amount;
        
        hitPoints.CurrentHps -= command.Amount;
        if (hitPoints.CurrentHps < 0)
        {
            command.CorrectedAmount = command.Amount + hitPoints.CurrentHps;
            hitPoints.CurrentHps = 0;
        }

        return Task.FromResult(CommandResponse.Success());
    }

    public override void Undo(LooseHpCommand command)
    {
        var hitPoints = command.GetHitPoints(_fightContext) ?? throw new ArgumentException($"Could not get hitpoints for this {command.GetType()}");

        if (command.CorrectedAmount == null)
        {
            throw new InvalidOperationException($"Cannot undo a {command.GetType()} when it has not been executed yet.");
        }

        hitPoints.CurrentHps += command.CorrectedAmount.Value;
    }

    public override async Task Redo(LooseHpCommand command)
    {
        await Execute(command);
    }
}
