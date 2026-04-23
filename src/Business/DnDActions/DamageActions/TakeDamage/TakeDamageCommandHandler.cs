using DnDFightTool.Business.DnDActions.HitPointActions.LooseHp;
using DnDFightTool.Business.DnDActions.HitPointActions.LooseTempHp;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.DamageActions.TakeDamage;

public class TakeDamageCommandHandler : CommandHandlerBase<TakeDamageCommand>
{
    private readonly IFightContext _fightContext;

    public TakeDamageCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public override async Task<ICommandResponse<NoResponse>> ExecuteAsync(TakeDamageCommand command)
    {
        var target = _fightContext[command.TargetId] ?? throw new NullReferenceException($"{typeof(TakeDamageCommandHandler)} could not find target with id {command.TargetId}");

        var remainingDamage = command.Damage;

        if (target.HitPoints.CurrentTempHps != 0)
        {
            var tempHpToRemove = Math.Min(command.Damage, target.HitPoints.CurrentTempHps);
            remainingDamage -= tempHpToRemove;

            await _mediator.SendAsSubCommandAsync(new LooseTempHpCommand(target.Id, tempHpToRemove), parentCommand: command);
        }

        if (remainingDamage > 0)
        {
            await _mediator.SendAsSubCommandAsync(new LooseHpCommand(target.Id, remainingDamage), parentCommand: command);
        }

        return CommandResponse.Success();
    }

    public async override Task RedoAsync(TakeDamageCommand command)
    {
        // The subcommands of this one are applying damages based on the current hp/temp hps of the target
        // Since hitPoints might have changed, we clear the subcommands and re execute the command fully
        ClearSubCommands(command);
        await ExecuteAsync(command);
    }
}
