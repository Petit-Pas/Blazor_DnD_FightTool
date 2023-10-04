using DnDFightTool.Business.DnDActions.StatusActions.ApplyStatus;
using DnDFightTool.Business.DnDQueries.SaveQueries;
using DnDFightTool.Domain.Fight;
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
        
        if (!status.IsAppliedAutomatically)
        {
            // TODO add a SaveContext that tells wether its magic or not, poison, charm etc so we can provide for defaul resistance to these
            var saveQuery = new SaveRollResultQuery(caster.Id, target.Id, status.Save);

            var saveRollResultResponse = await _mediator.Execute(saveQuery);
            if (saveRollResultResponse.Status != RequestStatus.Success) 
            {
                return new CommandResponse(saveRollResultResponse);
            }
            command.SaveRollResult = saveRollResultResponse.Response;

            if (command.SaveRollResult!.IsSuccesfull(target, caster))
            {
                return CommandResponse.Success();
            }
        }

        var applyStatusCommand = new ApplyStatusCommand(caster.Id, target.Id, status.Id, command.SaveRollResult);
        await _mediator.Execute(applyStatusCommand);
        command.AddToSubCommands(applyStatusCommand);

        return CommandResponse.Success();
    }
}
