using DnDFightTool.Business.DnDActions.DamageActions.TakeDamage;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Damage;
using DnDFightTool.Domain.Rolls;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
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

    public async override Task<ICommandResponse<NoResponse>> ExecuteAsync(ApplyDamageRollResultsCommand command)
    {
        var target = _fightContext[command.TargetId] ?? throw new NullReferenceException($"{typeof(ApplyDamageRollResultsCommandHandler)} could not find target with id {command.TargetId}");
        var caster = _fightContext[command.CasterId] ?? throw new NullReferenceException($"{typeof(ApplyDamageRollResultsCommandHandler)} could not find caster with id {command.CasterId}");

        var totalDamage = 0;

        foreach (var damageRoll in command.DamageRolls)
        {
            var actualDamage = ApplyAffinity(damageRoll.Damage, damageRoll.DamageType, target);
            actualDamage = ApplySaveModifier(actualDamage, damageRoll.SuccessfulSaveModifier, command.Save, target, caster);

            totalDamage += (int)Math.Floor(actualDamage);
        }

        await _mediator.SendAsSubCommandAsync(new TakeDamageCommand(target.Id, totalDamage), parentCommand: command);

        return CommandResponse.Success();
    }

    private static double ApplyAffinity(int damage, DamageTypeEnum damageType, FightingCharacter target)
    {
        var damageFactor = target.DamageAffinities.GetDamageFactorFor(damageType);

        return damageFactor.ApplyOn(damage);
    }

    private static double ApplySaveModifier(double actualDamage, SituationalDamageModifierEnum modifier, SaveRollResult? save, ICharacter target, ICharacter caster)
    {
        if (save != null && save.IsSuccessful(caster, target))
        {
            var factor = modifier.GetFactor();
            return factor.ApplyOn(actualDamage);
        }
        return actualDamage;
    }

    public async override Task RedoAsync(ApplyDamageRollResultsCommand command)
    {
        // The subcommands of this one are applying damages that were computed with resistance.
        // Since resistance might have changed, we clear the subcommands and re execute the command fully
        ClearSubCommands(command);
        await ExecuteAsync(command);
    }
}
