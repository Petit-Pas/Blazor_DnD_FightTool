using DnDFightTool.Business.DnDQueries;
using DnDFightTool.Business.DnDQueries.FightQueries;
using MudBlazor;
using UndoableMediator.Queries;

namespace DnDFightTool.UI.DnDQueryPrompter.FightQueries;

/// <summary>
///     Opens the <see cref="InitiativeRollQueryHandlerModal"/> via the global <see cref="IDialogServiceProvider"/>.
/// </summary>
public class InitiativeRollQueryHandler : QueryHandlerBase<InitiativeRollQuery, int>
{
    private readonly IDialogServiceProvider _dialogServiceProvider;

    public InitiativeRollQueryHandler(IDialogServiceProvider dialogServiceProvider)
    {
        _dialogServiceProvider = dialogServiceProvider ?? throw new ArgumentNullException(nameof(dialogServiceProvider));
    }

    public async override Task<IQueryResponse<int>> ExecuteAsync(InitiativeRollQuery query)
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };
        var parameters = new DialogParameters<InitiativeRollQueryHandlerModal>
        {
            { x => x.CharacterId, query.CharacterId }
        };

        var dialog = await _dialogServiceProvider.GetDialogService()
            .ShowAsync<InitiativeRollQueryHandlerModal>("Roll for Initiative", parameters, options);
        var result = await dialog.Result;

        if (result is null || result.Canceled)
        {
            return QueryResponse<int>.Canceled(0);
        }

        return QueryResponse<int>.Success((int)result.Data!);
    }
}
