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

    public override Task<ICommandResponse<NoResponse>> Execute(RegainHpCommand command)
    {
        var hitPoints = command.GetHitPoints(_fightContext) ?? throw new ArgumentException($"Could not get hitpoints for this {command.GetType()}");

        command.CorrectedAmount = command.Amount;

        hitPoints.CurrentHps += command.Amount;
        if (hitPoints.CurrentHps > hitPoints.MaxHps)
        {
            command.CorrectedAmount -= hitPoints.CurrentHps - hitPoints.MaxHps;
            hitPoints.CurrentHps = hitPoints.MaxHps;
        }

        return Task.FromResult(CommandResponse.Success());
    }

    public override void Undo(RegainHpCommand command)
    {
        var hitPoints = command.GetHitPoints(_fightContext);

        if (command.CorrectedAmount == null)
        {
            throw new InvalidOperationException($"Cannot undo a {command.GetType()} when it has not been executed yet.");
        }

        hitPoints.CurrentHps -= command.CorrectedAmount.Value;
    }

    public override async Task Redo(RegainHpCommand command)
    {
        await Execute(command);
    }
}
