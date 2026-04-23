using DnDFightTool.Business.DnDQueries;
using DnDFightTool.Business.DnDQueries.SaveQueries;
using DnDFightTool.Domain.Rolls;
using MudBlazor;
using UndoableMediator.Queries;

namespace DnDFightTool.UI.DnDQueryPrompter.SaveQueries;

public class SaveRollResultQueryHandler : QueryHandlerBase<SaveRollResultQuery, SaveRollResult>
{
    private readonly IDialogServiceProvider _dialogServiceProvider;

    public SaveRollResultQueryHandler(IDialogServiceProvider dialogServiceProvider)
    {
        _dialogServiceProvider = dialogServiceProvider ?? throw new ArgumentNullException(nameof(dialogServiceProvider));
    }

    public async override Task<IQueryResponse<SaveRollResult>> ExecuteAsync(SaveRollResultQuery query)
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        var parameters = new DialogParameters<SaveRollResultQueryHandlerModal>
        {
            { x => x.Save, query.Save },
            { x => x.CasterId, query.CasterId },
            { x => x.TargetId, query.TargetId }
        };

        var dialog = await _dialogServiceProvider.GetDialogService().ShowAsync<SaveRollResultQueryHandlerModal>("Saving Throw", parameters, options);
        var result = await dialog.Result;

        if (result is null || result.Canceled)
        {
            return QueryResponse<SaveRollResult>.Canceled(null!);
        }

        return QueryResponse<SaveRollResult>.Success((result.Data as SaveRollResult)!);
    }
}
