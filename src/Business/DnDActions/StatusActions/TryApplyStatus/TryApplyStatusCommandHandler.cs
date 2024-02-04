using DnDFightTool.Business.DnDActions.StatusActions.ApplyStatus;
using DnDFightTool.Business.DnDQueries.SaveQueries;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Statuses;
using DnDFightTool.Domain.Fight;
using Memory.Hashes;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;

namespace DnDFightTool.Business.DnDActions.StatusActions.TryApplyStatus;

public class TryApplyStatusCommandHandler : CommandHandlerBase<TryApplyStatusCommand>
{
    private readonly IFightContext _fightContext;

    public TryApplyStatusCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    public override async Task<ICommandResponse<NoResponse>> Execute(TryApplyStatusCommand command)
    {
        var caster = command.GetCaster(_fightContext);
        var target = command.GetTarget(_fightContext);
        var status = caster.GetPossiblyAppliedStatus(command.StatusId) ?? throw new ArgumentNullException("Could not get status to try to apply");

        command.StatusHash = status.Hash();

        var saveQueryResult = await QuerySaveRoll(command, status);
        if (saveQueryResult != RequestStatus.Success)
        {
            return new CommandResponse(saveQueryResult);
        }

        await TryApplyStatus(command, status, caster, target);

        return CommandResponse.Success();
    }

    private async Task TryApplyStatus(TryApplyStatusCommand command, StatusTemplate status, Character caster, Character target)
    {
        if (status.ShouldBeApplied(caster, target, command.SaveRollResult))
        {
            var applyStatusCommand = new ApplyStatusCommand(caster.Id, target.Id, status.Id, command.SaveRollResult);
            await _mediator.Execute(applyStatusCommand);
            command.AddToSubCommands(applyStatusCommand);
        }
    }

    private async Task<RequestStatus> QuerySaveRoll(TryApplyStatusCommand command, StatusTemplate status)
    {
        if (status.IsAppliedAutomatically)
        {
            return RequestStatus.Success;
        }

        var saveQuery = new SaveRollResultQuery(command.CasterId, command.TargetId, status.Save);
        var saveQueryResponse = await _mediator.Execute(saveQuery);
        command.SaveRollResult = saveQueryResponse.Response;
        return saveQueryResponse.Status;
    }

    public override async Task Redo(TryApplyStatusCommand command)
    {
        var caster = command.GetCaster(_fightContext);
        var target = command.GetTarget(_fightContext);
        var status = caster.GetPossiblyAppliedStatus(command.StatusId) ?? throw new ArgumentNullException("Could not get status to try to apply");

        var statusHash = status.Hash();
        if (statusHash != command.StatusHash)
        {
            command.SaveRollResult = null;
            command.StatusHash = statusHash;
            
            // Cannot accept a failure here, since we are in a redo operation
            var result = await QuerySaveRoll(command, status);
            while (result != RequestStatus.Success)
            {
                result = await QuerySaveRoll(command, status);
            }
        }

        command.SubCommands.Clear();

        await TryApplyStatus(command, status, caster, target);
    }
}
