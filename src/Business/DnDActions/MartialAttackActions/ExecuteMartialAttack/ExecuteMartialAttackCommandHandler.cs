using DnDFightTool.Business.DnDActions.DamageActions.ApplyDamageRollResults;
using DnDFightTool.Business.DnDActions.StatusActions.TryApplyStatus;
using DnDFightTool.Business.DnDQueries.MartialAttackQueries;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using Memory.Hashes;

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
    public ExecuteMartialAttackCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public async override Task<ICommandResponse<NoResponse>> Execute(ExecuteMartialAttackCommand command)
    {
        var caster = command.GetCaster(_fightContext);
        var attackTemplate = command.GetAttackTemplate(caster) ?? throw new InvalidOperationException($"Could not get attack template.");

        command.AttackTemplateHash = attackTemplate.Hash();

        // Query attack roll, as well as target
        var queryStatus = await QueryAttackRollResult(command);
        if (queryStatus != RequestStatus.Success)
        {
            return new CommandResponse(queryStatus);
        }

        var target = _fightContext.GetCharacterById(command.MartialAttackRollResult!.TargetId) ?? throw new InvalidOperationException($"Could not get target.");
        if (AttackHits(caster, target, command))
        {
            await ApplyDamage(caster, target, command);
            await ApplyStatuses(caster, target, command, attackTemplate);
        }

        return CommandResponse.Success();
    }

    public async override Task Redo(ExecuteMartialAttackCommand command)
    {
        var caster = command.GetCaster(_fightContext);
        var attackTemplate = command.GetAttackTemplate(caster) ?? throw new InvalidOperationException($"Could not get attack template.");

        // if the attack template has changed, we need to recompute everything
        var newAttackTemplateHash = attackTemplate.Hash();
        if (newAttackTemplateHash != command.AttackTemplateHash)
        {
            command.AttackTemplateHash = newAttackTemplateHash;
            command.SubCommands.Clear();
            var result = await QueryAttackRollResult(command);
            if (result != RequestStatus.Success)
            {
                // Canceled
                return;
            }
        }

        var target = _fightContext.GetCharacterById(command.MartialAttackRollResult!.TargetId) ?? throw new InvalidOperationException($"Could not get target.");
        if (AttackHits(caster, target, command))
        {
            // attack was already executed and had effect, so we can reuse the same commands
            if (command.SubCommands.Count != 0)
            {
                await base.Redo(command);
            }
            // attack has had no effect before (or we cleared it because the template changed), we need to re-compute everything
            else
            {
                await ApplyDamage(caster, target, command);
                await ApplyStatuses(caster, target, command, attackTemplate);
            }
        }
        // attack does not hit, and since the Undo button could be clicked again, we need to make sure that no subCommands remains.
        // it also means that after that, we lose the potential dice throws in the subCommands,
        //      as next time this is re-executed and the attack hits, we will call ApplyDamage & ApplyStatus again.
        else
        {
            command.SubCommands.Clear();
        }
    }

    /// <summary>
    ///     Query the attack roll result from the template and store it in the command.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    private async Task<RequestStatus> QueryAttackRollResult(ExecuteMartialAttackCommand command)
    {
        var attackRollResultQuery = new MartialAttackRollResultQuery(command.MartialAttackId, command.CasterId);
        var attackRollResponse = await _mediator.Execute(attackRollResultQuery);
        command.MartialAttackRollResult = attackRollResponse.Response;
        return attackRollResponse.Status;
    }

    /// <summary>
    ///     Try applying the potential statuses of the <see cref="MartialAttackTemplate"/> stored in the command.
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    /// <param name="command"></param>
    /// <param name="attackTemplate"></param>
    /// <returns></returns>
    private async Task ApplyStatuses(Character caster, Character target, ExecuteMartialAttackCommand command, MartialAttackTemplate attackTemplate)
    {
        foreach (var onHitStatus in attackTemplate.Statuses.Values)
        {
            var tryApplyStatusCommand = new TryApplyStatusCommand(caster.Id, target.Id, onHitStatus.Id);
            await _mediator.Execute(tryApplyStatusCommand);
            command.AddToSubCommands(tryApplyStatusCommand);
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
    private async Task ApplyDamage(Character caster, Character target, ExecuteMartialAttackCommand command)
    {
        if (command.MartialAttackRollResult == null)
        {
            // TODO should warn in the console and stop
#pragma warning disable
            throw new ArgumentNullException($"Cannot apply damage of a null roll result.");
#pragma warning restore
        }

            var applyDamageRollResultCommand = new ApplyDamageRollResultsCommand(caster.Id, target.Id, command.MartialAttackRollResult.DamageRolls);
        command.AddToSubCommands(applyDamageRollResultCommand);
        await _mediator.Execute(applyDamageRollResultCommand);
    }

    /// <summary>
    ///     Checks if the Attack roll result stored in the command hits.
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private static bool AttackHits(Character caster, Character target, ExecuteMartialAttackCommand command)
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
}
