using Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDActions.HitPointActions.RegainHp;

public class RegainHpCommandHandler : CommandHandlerBase<RegainHpCommand>
{
    private readonly IFightContext _fightContext;

    public RegainHpCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public override ICommandResponse<NoResponse> Execute(RegainHpCommand command)
    {
        var hitPoints = command.GetHitPoints(_fightContext) ?? throw new ArgumentException($"Could not get hitpoints for this {command.GetType()}");

        command.CorrectedAmount = command.Amount;

        hitPoints.CurrentHps += command.Amount;
        if (hitPoints.CurrentHps > hitPoints.MaxHps)
        {
            command.CorrectedAmount -= hitPoints.CurrentHps - hitPoints.MaxHps;
            hitPoints.CurrentHps = hitPoints.MaxHps;
        }

        return CommandResponse.Success();
    }

    public override void Undo(RegainHpCommand command)
    {
        base.Undo(command);

        var hitPoints = command.GetHitPoints(_fightContext) ?? throw new ArgumentException($"Could not get hitpoints for this {command.GetType()}");

        if (command.CorrectedAmount == null)
        {
            throw new InvalidOperationException($"Cannot undo a {command.GetType()} when it has not been executed yet.");
        }

        hitPoints.CurrentHps -= command.CorrectedAmount.Value;
    }

    public override void Redo(RegainHpCommand command)
    {
        base.Redo(command);


        var hitPoints = command.GetHitPoints(_fightContext) ?? throw new ArgumentException($"Could not get hitpoints for this {command.GetType()}");

        if (command.CorrectedAmount == null)
        {
            throw new InvalidOperationException($"Cannot redo a {command.GetType()} when it has not been executed/undone yet.");
        }

        hitPoints.CurrentHps += command.CorrectedAmount.Value;
    }
}
