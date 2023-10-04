using DnDFightTool.Domain.DnDEntities.Statuses;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.StatusActions.ApplyStatus;

public class ApplyStatusCommandHandler : CommandHandlerBase<ApplyStatusCommand>
{
    private readonly IFightContext _fightContext;
    private readonly IAppliedStatusCollection _appliedStatusCollection;

    public ApplyStatusCommandHandler(IUndoableMediator mediator, IFightContext fightContext, IAppliedStatusCollection appliedStatusCollection) : base(mediator)
    {
        _fightContext = fightContext ?? throw new ArgumentNullException(nameof(fightContext));
        _appliedStatusCollection = appliedStatusCollection ?? throw new ArgumentNullException(nameof(appliedStatusCollection));
    }

    public override Task<ICommandResponse<NoResponse>> Execute(ApplyStatusCommand command)
    {
        var caster = _fightContext.GetCharacterById(command.CasterId) ?? throw new ArgumentNullException(nameof(command.CasterId));
        var target = _fightContext.GetCharacterById(command.TargetId) ?? throw new ArgumentNullException(nameof(command.TargetId));
        var status = caster.GetPossiblyAppliedStatus(command.StatusId) ?? throw new ArgumentNullException(nameof(command.StatusId));

        var appliedStatus = new AppliedStatus(caster.Id, target.Id, status.Name);

        _appliedStatusCollection.Add(appliedStatus);

        return Task.FromResult(CommandResponse.Success());
    }
}
