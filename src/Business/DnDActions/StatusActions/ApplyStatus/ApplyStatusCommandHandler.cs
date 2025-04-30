using DnDFightTool.Domain.DnDEntities.Statuses;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.StatusActions.ApplyStatus;

public class ApplyStatusCommandHandler : CommandHandlerBase<ApplyStatusCommand>
{
    private readonly IFightContext _fightContext;
    private readonly IAppliedStatusRepository _appliedStatusCollection;

    public ApplyStatusCommandHandler(IUndoableMediator mediator, IFightContext fightContext, IAppliedStatusRepository appliedStatusCollection) : base(mediator)
    {
        _fightContext = fightContext ?? throw new ArgumentNullException(nameof(fightContext));
        _appliedStatusCollection = appliedStatusCollection ?? throw new ArgumentNullException(nameof(appliedStatusCollection));
    }

    public override Task<ICommandResponse<NoResponse>> Execute(ApplyStatusCommand command)
    {
        // TODO should warn in the console and stop
#pragma warning disable
        var caster = _fightContext.GetCharacterById(command.CasterId) ?? throw new ArgumentNullException(nameof(command.CasterId));
        var target = _fightContext.GetCharacterById(command.TargetId) ?? throw new ArgumentNullException(nameof(command.TargetId));
        var status = caster.GetPossiblyAppliedStatus(command.StatusId) ?? throw new ArgumentNullException(nameof(command.StatusId));
#pragma warning restore
        var appliedStatus = new AppliedStatus(caster.Id, target.Id, status.Name);

        command.AppliedStatusId = appliedStatus.Id;

        _appliedStatusCollection.Add(appliedStatus);

        return Task.FromResult(CommandResponse.Success());
    }

    public override void Undo(ApplyStatusCommand command)
    {
        base.Undo(command);

        _appliedStatusCollection.RemoveIfExists(command.AppliedStatusId);
    }

    // No need to redo, the status should never be applied blindly, so the TryApplyStatus.Redo will never use the history
}
