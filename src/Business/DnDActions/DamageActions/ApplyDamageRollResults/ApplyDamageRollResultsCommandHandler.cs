using DnDActions.DamageActions.TakeDamage;
using DnDEntities.Characters;
using DnDEntities.Damage;
using Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDActions.DamageActions.ApplyDamageRollResults;

public class ApplyDamageRollResultsCommandHandler : CommandHandlerBase<ApplyDamageRollResultsCommand>
{
    private readonly IFightContext _fightContext;

    public ApplyDamageRollResultsCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public override ICommandResponse<NoResponse> Execute(ApplyDamageRollResultsCommand command)
    {
        var target = command.GetTarget(_fightContext) ?? throw new ArgumentException($"Could not get target for this {command.GetType()}");

        var totalDamage = 0;

        foreach (var damageRoll in command.DamageRolls)
        {
            var actualDamage = ApplyAffinity(damageRoll.Damage, damageRoll.DamageType, target);

            totalDamage += (int)Math.Floor(actualDamage);
        }

        var takeDamageCommand = new TakeDamageCommand(target.Id, totalDamage);
        command.AddToSubCommands(takeDamageCommand);
        _mediator.Execute(takeDamageCommand);

        return CommandResponse.Success();
    }

    private double ApplyAffinity(int damage, DamageTypeEnum damageType, Character target)
    {
        var damageFactor = target.DamageAffinities.GetDamageFactorFor(damageType);

        return damageFactor.ApplyOn(damage);
    }

    public override void Redo(ApplyDamageRollResultsCommand command)
    {
        // The subcommands of this one are applying damages that were computed with resistance.
        // Since resistance might have changed, we clear the subcommands and re execute the command fully
        command.SubCommands.Clear();
        Execute(command);
    }
}
