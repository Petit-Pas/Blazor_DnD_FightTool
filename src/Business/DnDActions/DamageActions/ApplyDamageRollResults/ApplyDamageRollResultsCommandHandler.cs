using DnDFightTool.Business.DnDActions.DamageActions.TakeDamage;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Damage;
using DnDFightTool.Domain.DnDEntities.Saves;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.DamageActions.ApplyDamageRollResults;

public class ApplyDamageRollResultsCommandHandler : CommandHandlerBase<ApplyDamageRollResultsCommand>
{
    private readonly IFightContext _fightContext;

    public ApplyDamageRollResultsCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public override async Task<ICommandResponse<NoResponse>> Execute(ApplyDamageRollResultsCommand command)
    {
        var target = command.GetTarget(_fightContext);
        var caster = command.GetCaster(_fightContext);

        var totalDamage = 0;

        foreach (var damageRoll in command.DamageRolls)
        {
            var actualDamage = ApplyAffinity(damageRoll.Damage, damageRoll.DamageType, target);
            actualDamage = ApplySaveModifier(actualDamage, damageRoll.SuccessfullSaveModifier, command.Save, target, caster);

            totalDamage += (int)Math.Floor(actualDamage);
        }

        var takeDamageCommand = new TakeDamageCommand(target.Id, totalDamage);
        command.AddToSubCommands(takeDamageCommand);
        await _mediator.Execute(takeDamageCommand);

        return CommandResponse.Success();
    }

    private double ApplyAffinity(int damage, DamageTypeEnum damageType, Character target)
    {
        var damageFactor = target.DamageAffinities.GetDamageFactorFor(damageType);

        return damageFactor.ApplyOn(damage);
    }

    private double ApplySaveModifier(double actualDamage, SituationalDamageModifierEnum modifier, SaveRollResult? save, Character target, Character caster)
    {
        if (save != null && save.IsSuccesfull(target, caster))
        {
            var factor = modifier.GetFactor();
            return factor.ApplyOn(actualDamage);
        }
        return actualDamage;
    }

    public override async Task Redo(ApplyDamageRollResultsCommand command)
    {
        // The subcommands of this one are applying damages that were computed with resistance.
        // Since resistance might have changed, we clear the subcommands and re execute the command fully
        command.SubCommands.Clear();
        await Execute(command);
    }
}
