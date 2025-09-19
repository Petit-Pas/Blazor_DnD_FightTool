using DnDFightTool.Business.DnDQueries.SaveQueries;
using DnDFightTool.Domain.DnDEntities.Saves;
using DnDFightTool.Domain.Fight;
using MudBlazor;
using UndoableMediator.Queries;

namespace FightBlazorComponents.Queries.SaveQueries;

public class SaveRollResultQueryHandler : QueryHandlerBase<SaveRollResultQuery, SaveRollResult>
{
    private readonly IFightContext _fightContext;
    private readonly IDialogService _dialogService;

    public SaveRollResultQueryHandler(IFightContext fightContext, IDialogService dialogService)
    {
        _fightContext = fightContext;
        _dialogService = dialogService;
    }

    public override Task<IQueryResponse<SaveRollResult>> Execute(SaveRollResultQuery query)
    {

        // TODO
        throw new NotImplementedException("Modals have to be reimplemented");

        //var modalService = _modalServiceProvider.GetModalService();
        //ArgumentNullException.ThrowIfNull(modalService, nameof(modalService));

        //var caster = _fightContext[query.CasterId];
        //var target = _fightContext[query.TargetId];
        //var saveRollResult = query.Save.GetEmptyRollResult();


        //var parameters = new ModalParameters()
        //    .Add(nameof(SaveRollResultQueryModal.SaveRollResult), saveRollResult)
        //    .Add(nameof(SaveRollResultQueryModal.Caster), caster).
        //    Add(nameof(SaveRollResultQueryModal.Target), target);
        //var options = new ModalOptions() { UseCustomLayout = true };
        //var modal = modalService.Show<SaveRollResultQueryModal>("Saving throw", parameters, options);

        //var result = await modal.Result;

        //if (result.Cancelled)
        //{
        //    return QueryResponse<SaveRollResult>.Canceled(null!);
        //}

        // return QueryResponse<SaveRollResult>.Success(saveRollResult);
    }
}
