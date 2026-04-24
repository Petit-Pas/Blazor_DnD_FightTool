using DnDFightTool.Business.DnDActions.DamageActions.TakeDamage;
using DnDFightTool.Business.DnDActions.LogActions;
using DnDFightTool.Business.DnDActions.LogActions.CloseScope;
using DnDFightTool.Business.DnDActions.LogActions.OpenScope;
using DnDFightTool.Business.DnDActions.LogActions.WriteLog;
using DnDFightTool.Domain.CharacterSheet.AbilityScores;
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

        if (command.Save != null)
        {
            await LogSaveRoll(command.Save, caster, target, command);
        }

        await _mediator.SendAsSubCommandAsync(new OpenScopeCommand(), parentCommand: command);

        foreach (var damageRoll in command.DamageRolls)
        {
            var actualDamage = ApplyAffinity(damageRoll.Damage, damageRoll.DamageType, target);
            actualDamage = ApplySaveModifier(actualDamage, damageRoll.SuccessfulSaveModifier, command.Save, target, caster);

            var damageInt = (int)Math.Floor(actualDamage);
            totalDamage += damageInt;

            await LogDamageRoll(damageRoll, damageInt, command);
        }

        await _mediator.SendAsSubCommandAsync(new CloseScopeCommand(), parentCommand: command);

        await _mediator.SendAsSubCommandAsync(new TakeDamageCommand(target.Id, totalDamage), parentCommand: command);

        return CommandResponse.Success();
    }

    private async Task LogSaveRoll(SaveRollResult save, FightingCharacter caster, FightingCharacter target, ApplyDamageRollResultsCommand command)
    {
        var dc = save.Target.GetValue(caster);
        var modifier = target.AbilityScores.GetSavingModifier(save.Ability);
        var totalSave = save.Result + modifier.Modifier;
        var successful = save.IsSuccessful(caster, target);
        var sign = modifier.Modifier >= 0 ? "+" : "";
        await _mediator.SendAsSubCommandAsync(
            new WriteLogCommand($"[hover:d20 = {save.Result}, {save.Ability} modifier = {sign}{modifier.Modifier}][b]{totalSave}[/b][/hover] {save.Ability} save (DC {dc}) => [b]{(successful ? "Success" : "Failure")}[/b]"),
            parentCommand: command);
    }

    private async Task LogDamageRoll(DamageRollResult damageRoll, int damage, ApplyDamageRollResultsCommand command)
    {
        var tokenName = damageRoll.DamageType.ToLogColorToken();
        await _mediator.SendAsSubCommandAsync(
            new WriteLogCommand($"[c:{tokenName}][b]{damage}[/b] {damageRoll.DamageType.ToReadableString()} damage[/c]"),
            parentCommand: command);
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
