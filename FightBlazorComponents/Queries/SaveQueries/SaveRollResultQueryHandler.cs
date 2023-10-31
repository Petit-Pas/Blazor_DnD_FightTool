using Blazored.Modal;
using Blazored.Modal.Services;
using DnDFightTool.Business.DnDQueries.SaveQueries;
using DnDFightTool.Domain.DnDEntities.Saves;
using DnDFightTool.Domain.Fight;
using FightBlazorComponents.Pages;
using UndoableMediator.Queries;

namespace FightBlazorComponents.Queries.SaveQueries;

public class SaveRollResultQueryHandler : QueryHandlerBase<SaveRollResultQuery, SaveRollResult>
{
    private readonly IModalService _modalService;
    private readonly IFightContext _fightContext;

    public SaveRollResultQueryHandler(IFightContext fightContext)
    {
        _modalService = FightPage.SingletonModalService;
        _fightContext = fightContext;
    }

    public override async Task<IQueryResponse<SaveRollResult>> Execute(SaveRollResultQuery query)
    {
        var caster = _fightContext.GetCharacterById(query.CasterId);
        var target = _fightContext.GetCharacterById(query.TargetId);
        var saveRollResult = query.Save.GetEmptyRollResult();

        if (caster == null || target == null)
        {
            throw new ArgumentNullException("Could not find caster or target.");
        }

        var parameters = new ModalParameters()
            .Add(nameof(SaveRollResultQueryModal.SaveRollResult), saveRollResult)
            .Add(nameof(SaveRollResultQueryModal.Caster), caster).
            Add(nameof(SaveRollResultQueryModal.Target), target);
        var options = new ModalOptions() { UseCustomLayout = true };
        var modal = _modalService.Show<SaveRollResultQueryModal>("Saving throw", parameters, options);

        var result = await modal.Result;

        if (result.Cancelled)
        {
            return QueryResponse<SaveRollResult>.Canceled(null!);
        }

        return QueryResponse<SaveRollResult>.Success(saveRollResult);
    }
}
