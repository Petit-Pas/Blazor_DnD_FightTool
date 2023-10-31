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
using Extensions.Mediator;
using Extensions;

namespace DnDFightTool.Business.DnDActions.MartialAttackActions.ExecuteMartialAttack;

public class ExecuteMartialAttackCommandHandler : CommandHandlerBase<ExecuteMartialAttackCommand>
{
    private readonly IFightContext _fightContext;

    public ExecuteMartialAttackCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public override async Task<ICommandResponse<NoResponse>> Execute(ExecuteMartialAttackCommand command)
    {
        var caster = command.GetCaster(_fightContext);
        var attackTemplate = command.GetAttackTemplate(caster) ?? throw new InvalidOperationException($"Could not get attack template.");

        command.Hash = attackTemplate.Hash();

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

    private async Task<RequestStatus> QueryAttackRollResult(ExecuteMartialAttackCommand command)
    {
        var attackRollResultQuery = new MartialAttackRollResultQuery(command.MartialAttackId, command.CasterId);
        var attackRollResponse = await _mediator.Execute(attackRollResultQuery);
        command.MartialAttackRollResult = attackRollResponse.Response;
        return attackRollResponse.Status;
    }

    private async Task ApplyStatuses(Character caster, Character target, ExecuteMartialAttackCommand command, MartialAttackTemplate attackTemplate)
    {
        foreach (var onHitStatus in attackTemplate.Statuses)
        {
            var tryApplyStatusCommand = new TryApplyStatusCommand(caster.Id, target.Id, onHitStatus.Id);
            await _mediator.Execute(tryApplyStatusCommand);
            command.AddToSubCommands(tryApplyStatusCommand);
        }
    }

    private async Task ApplyDamage(Character caster, Character target, ExecuteMartialAttackCommand command)
    {
        if (command.MartialAttackRollResult == null)
        {
            throw new ArgumentNullException($"Cannot apply damage of a null roll result.");
        }

        var applyDamageRollResultCommand = new ApplyDamageRollResultsCommand(caster.Id, target.Id, command.MartialAttackRollResult.DamageRolls);
        command.AddToSubCommands(applyDamageRollResultCommand);
        await _mediator.Execute(applyDamageRollResultCommand);
    }

    private bool AttackHits(Character caster, Character target, ExecuteMartialAttackCommand command)
    {
        if (command.MartialAttackRollResult == null)
        {
            throw new ArgumentNullException($"Cannot evaluate a rollResult that was not requested first.");
        }
        return command.MartialAttackRollResult.HitRoll.Hits(target, caster);
    }

    public override async Task Redo(ExecuteMartialAttackCommand command)
    {
        var caster = command.GetCaster(_fightContext);
        var target = _fightContext.GetCharacterById(command.MartialAttackRollResult!.TargetId) ?? throw new InvalidOperationException($"Could not get target.");
        var attackTemplate = command.GetAttackTemplate(caster) ?? throw new InvalidOperationException($"Could not get attack template.");
        
        if (attackTemplate.Hash() != command.Hash)
        {
            command.SubCommands.Clear();
            var result = await QueryAttackRollResult(command);
            if (result != RequestStatus.Success)
            {
                // Canceled
                return;
            }
        }

        if (AttackHits(caster, target, command))
        {
            // attack was already executed and had effect, so we can reuse the same commands
            if (command.SubCommands.Count != 0)
            {
                await base.Redo(command);
            }
            // attack has has no effect before, we need to compute everything
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
}
