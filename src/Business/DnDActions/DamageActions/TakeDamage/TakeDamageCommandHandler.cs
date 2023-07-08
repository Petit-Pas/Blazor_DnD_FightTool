using DnDActions.HitPointActions.LooseHp;
using DnDActions.HitPointActions.LooseTempHp;
using Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDActions.DamageActions.TakeDamage;

public class TakeDamageCommandHandler : CommandHandlerBase<TakeDamageCommand>
{
    private readonly IFightContext _fightContext;

    public TakeDamageCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public override Task<ICommandResponse<NoResponse>> Execute(TakeDamageCommand command)
    {
        var target = command.GetTarget(_fightContext);
        
        var remainingDamage = command.Damage;

        if (target.HitPoints.CurrentTempHps != 0)
        {
            var tempHpToRemove = Math.Min(command.Damage, target.HitPoints.CurrentTempHps);

            var looseTempHpCommand = new LooseTempHpCommand(target.Id, tempHpToRemove);
            remainingDamage -= tempHpToRemove;

            command.AddToSubCommands(looseTempHpCommand);
            _mediator.Execute(looseTempHpCommand);
        }

        if (remainingDamage > 0)
        {
            var looseHpCommand = new LooseHpCommand(target.Id, remainingDamage);
            command.AddToSubCommands(looseHpCommand);
            _mediator.Execute(looseHpCommand);
        }

        return Task.FromResult(CommandResponse.Success());
    }

    public override async Task Redo(TakeDamageCommand command)
    {
        // The subcommands of this one are applying damages based on the current hp/temp hps of the target
        // Since hitPoints might have changed, we clear the subcommands and re execute the command fully
        command.SubCommands.Clear();
        await Execute(command);
    }
}
