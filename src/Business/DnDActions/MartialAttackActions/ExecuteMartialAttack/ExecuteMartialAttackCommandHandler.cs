using DnDFightTool.Business.DnDActions.DamageActions.ApplyDamageRollResults;
using DnDFightTool.Business.DnDActions.LogActions.CloseBlock;
using DnDFightTool.Business.DnDActions.LogActions.CloseScope;
using DnDFightTool.Business.DnDActions.LogActions.OpenBlock;
using DnDFightTool.Business.DnDActions.LogActions.OpenScope;
using DnDFightTool.Business.DnDActions.LogActions.WriteLog;
using DnDFightTool.Business.DnDActions.StatusActions.TryApplyStatus;
using DnDFightTool.Business.DnDQueries.MartialAttackQueries;
using DnDFightTool.Domain.CharacterSheet.MartialAttacks;
using DnDFightTool.Domain.Rolls;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using DnDFightTool.Infrastructure.Memory.Hashes;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;

namespace DnDFightTool.Business.DnDActions.MartialAttackActions.ExecuteMartialAttack;

/// <summary>
///     Command Handler for <see cref="ExecuteMartialAttackCommand"/>.
/// </summary>
public class ExecuteMartialAttackCommandHandler : CommandHandlerBase<ExecuteMartialAttackCommand>
{
    /// <summary>
    ///     Fight context dependency.
    /// </summary>
    private readonly IFightContext _fightContext;

    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="fightContext"></param>
    public ExecuteMartialAttackCommandHandler(
        IUndoableMediator mediator,
        IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext ?? throw new ArgumentNullException(nameof(fightContext));
    }

    public async override Task<ICommandResponse<NoResponse>> ExecuteAsync(ExecuteMartialAttackCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var caster = _fightContext[command.CasterId] ?? throw new NullReferenceException($"{typeof(ExecuteMartialAttackCommandHandler)} could not find caster with id {command.CasterId}");
        var attackTemplate = command.GetAttackTemplate(caster);

        command.AttackTemplateHash = attackTemplate.Hash();

        // Queries attack roll, as well as targetId
        if (command.MartialAttackRollResult == null)
        {
            var queryStatus = await QueryAttackRollResult(command);
            if (queryStatus != RequestStatus.Success)
            {
                return new CommandResponse(queryStatus);
            }
        }

        var target = _fightContext[command.MartialAttackRollResult!.TargetId] ?? throw new NullReferenceException($"{typeof(ExecuteMartialAttackCommandHandler)} could not find target with id {command.MartialAttackRollResult!.TargetId}");
        await OpenAttackLog(caster, target, command, attackTemplate);

        try
        {
            command.AttackDidHit = AttackHits(caster, target, command);
            await LogHitRoll(command.MartialAttackRollResult.HitRoll, caster, target, command, command.AttackDidHit);
            if (command.AttackDidHit)
            {
                await ApplyDamage(caster, target, command);
                await ApplyStatuses(caster, target, command, attackTemplate);
            }
        }
        finally
        {
            await CloseAttackLog(command);
        }

        return CommandResponse.Success();
    }

    public async override Task RedoAsync(ExecuteMartialAttackCommand command)
    {
        // TODO should warn in the console and stop
        var caster = _fightContext[command.CasterId] ?? throw new NotImplementedException($"Cannot redo a {command.GetType()} when the caster with id {command.CasterId} cannot be found.");
        // TODO should warn in the console and stop
        var target = (_fightContext[command.MartialAttackRollResult!.TargetId] ?? throw new NullReferenceException($"{typeof(ExecuteMartialAttackCommandHandler)} could not find target with id {command.MartialAttackRollResult!.TargetId}")) ?? throw new NotImplementedException($"Cannot redo a {command.GetType()} when the target with id {command.MartialAttackRollResult!.TargetId} cannot be found.");
        // TODO should warn in the console and stop
        var attackTemplate = command.GetAttackTemplate(caster) ?? throw new NotImplementedException($"Cannot redo a {command.GetType()} when the attack template with id {command.MartialAttackId} cannot be found.");

        // if the attack template has changed, we need to recompute everything
        // if the the attack did hit the first time, but does not hit anymore, or the opposite, we also need to recompute everything, since the subCommands will be different
        var newAttackTemplateHash = attackTemplate.Hash();
        if (newAttackTemplateHash != command.AttackTemplateHash || command.AttackDidHit != AttackHits(caster, target, command))
        {
            // This will retrigger query
            command.ClearCachedState();
            ClearSubCommands(command);
            await ExecuteAsync(command);
        }
        // Otherwise, entities have not changes, characters are still hit, we can safely redo the same subCommands without re-querying the attack roll or re-evaluating the hit, since the result should be the same, and we want to preserve the potential dice rolls in the subCommands.
        else {
            await base.RedoAsync(command);
        }
    }

    /// <summary>
    ///     Query the attack roll result from the template and store it in the command.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    private async Task<RequestStatus> QueryAttackRollResult(ExecuteMartialAttackCommand command)
    {
        var requestResult = await _mediator.QueryAsync(new MartialAttackRollResultQuery(command.CasterId, command.MartialAttackId));
        command.MartialAttackRollResult = requestResult.Response;
        return requestResult.Status;
    }

    /// <summary>
    ///     Try applying the potential statuses of the <see cref="MartialAttackTemplate"/> stored in the command.
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    /// <param name="command"></param>
    /// <param name="attackTemplate"></param>
    /// <returns></returns>
    private async Task ApplyStatuses(FightingCharacter caster, FightingCharacter target, ExecuteMartialAttackCommand command, MartialAttackTemplate attackTemplate)
    {
        foreach (var onHitStatus in attackTemplate.Statuses.Values)
        {
            await _mediator.SendAsSubCommandAsync(new TryApplyStatusCommand(caster.Id, target.Id, onHitStatus.Id), parentCommand: command);
        }
    }

    /// <summary>
    ///     Applies the damage of the <see cref="MartialAttackRollResult"/> stored in the command.
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private async Task ApplyDamage(FightingCharacter caster, FightingCharacter target, ExecuteMartialAttackCommand command)
    {
        if (command.MartialAttackRollResult == null)
        {
            // TODO should warn in the console and stop
#pragma warning disable
            throw new ArgumentNullException($"Cannot apply damage of a null roll result.");
#pragma warning restore
        }

        await _mediator.SendAsSubCommandAsync(new ApplyDamageRollResultsCommand(caster.Id, target.Id, command.MartialAttackRollResult.DamageRolls), parentCommand: command);
    }

    /// <summary>
    ///     Checks if the Attack roll result stored in the command hits.
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private static bool AttackHits(FightingCharacter caster, FightingCharacter target, ExecuteMartialAttackCommand command)
    {
        if (command.MartialAttackRollResult == null)
        {
            // TODO should warn in the console and stop
#pragma warning disable
            throw new ArgumentNullException($"Cannot evaluate a rollResult that was not requested first.");
#pragma warning restore
        }
        return command.MartialAttackRollResult.HitRoll.Hits(target, caster);
    }

    private async Task OpenAttackLog(FightingCharacter caster, FightingCharacter target, ExecuteMartialAttackCommand command, MartialAttackTemplate attackTemplate)
    {
        await _mediator.SendAsSubCommandAsync(new OpenBlockCommand("Martial Attack"), parentCommand: command);
        await _mediator.SendAsSubCommandAsync(new WriteLogCommand($"[b]{caster.Name}[/b] attacks [b]{target.Name}[/b] using [b]{attackTemplate.Name}[/b]"), parentCommand: command);
        await _mediator.SendAsSubCommandAsync(new OpenScopeCommand(), parentCommand: command);
    }

    private async Task LogHitRoll(HitRollResult hitRoll, FightingCharacter caster, FightingCharacter target, ExecuteMartialAttackCommand command, bool didHit)
    {
        var totalAttack = hitRoll.Modifiers.GetScoreModifier(caster).ApplyTo(hitRoll.Result);
        await _mediator.SendAsSubCommandAsync(
            new WriteLogCommand($"[hover:d20 = {hitRoll.Result}, modifiers = {hitRoll.Modifiers.Expression}][b]{totalAttack}[/b][/hover] to hit (AC {target.ArmorClass.EffectiveAC}) => [b]{(didHit ? "Hit" : "Miss")}[/b]"),
            parentCommand: command);
    }

    private async Task CloseAttackLog(ExecuteMartialAttackCommand command)
    {
        await _mediator.SendAsSubCommandAsync(new CloseScopeCommand(), parentCommand: command);
        await _mediator.SendAsSubCommandAsync(new CloseBlockCommand(), parentCommand: command);
    }
}
