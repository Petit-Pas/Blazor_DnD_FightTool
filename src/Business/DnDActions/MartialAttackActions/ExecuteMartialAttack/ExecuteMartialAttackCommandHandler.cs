using DnDFightTool.Business.DnDActions.DamageActions.ApplyDamageRollResults;
using DnDFightTool.Business.DnDQueries.MartialAttackQueries;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

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
        var attackRollResultQuery = new MartialAttackRollResultQuery(command.MartialAttackId, command.CasterId);
        var attackRollResultQueryResponse = await _mediator.Execute(attackRollResultQuery);

        if (attackRollResultQueryResponse.Status != UndoableMediator.Requests.RequestStatus.Success)
        {
            return new CommandResponse(attackRollResultQueryResponse);
        }
        command.MartialAttackRollResult = attackRollResultQueryResponse.Response;

        return await ApplyRollResult(command);
    }

    private async Task<ICommandResponse<NoResponse>> ApplyRollResult(ExecuteMartialAttackCommand command)
    {
        if (command.MartialAttackRollResult == null)
        {
            throw new ArgumentNullException($"Cannot apply a null roll result.");
        }

        var caster = command.GetCaster(_fightContext);
        var target = _fightContext.GetCharacterById(command.MartialAttackRollResult.TargetId) ?? throw new InvalidOperationException($"Could not get target.");
        var attackTemplate = command.GetAttackTemplate(caster) ?? throw new InvalidOperationException($"Could not get attack template.");

        if (command.MartialAttackRollResult.HitRoll.Hits(target, caster))
        {
            var applyDamageRollResultCommand = new ApplyDamageRollResultsCommand(target.Id, caster.Id, command.MartialAttackRollResult.DamageRolls);
            command.AddToSubCommands(applyDamageRollResultCommand);
            await _mediator.Execute(applyDamageRollResultCommand);

            // Add a hash code in the attack template, if the hash has been modified, you execute everything, otherwise you may reexecute everything ? 

            // TODO apply status ? maybe some status are applied no matter if the attack actually hits ? 
        }

        return CommandResponse.Success();
    }

    public override async Task Redo(ExecuteMartialAttackCommand command)
    {
        // TODO This is not ok in case of OnHitStatus => we should make it so that the command that applied it will be redone -
        // TODO so that it can reuse the saveRollResult that it will most probably contain
        command.SubCommands.Clear();
        await ApplyRollResult(command);
    }
}
