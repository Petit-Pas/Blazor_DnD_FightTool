using DnDFightTool.Business.DnDQueries;
using DnDFightTool.Business.DnDQueries.MartialAttackQueries;
using DnDFightTool.Domain.Rolls;
using MudBlazor;
using UndoableMediator.Queries;

namespace DnDQueryPrompter.MartialAttackQueries;

/// <summary>
///     Query handler for <see cref="MartialAttackRollResultQuery"/>.
///     Shows a dialog to the user to enter the attack roll result.
/// </summary>
public class MartialAttackRollResultQueryHandler : QueryHandlerBase<MartialAttackRollResultQuery, MartialAttackRollResult>
{
    private readonly IDialogServiceProvider _dialogServiceProvider;

    public MartialAttackRollResultQueryHandler(IDialogServiceProvider dialogServiceProvider)
    {
        _dialogServiceProvider = dialogServiceProvider ?? throw new ArgumentNullException(nameof(dialogServiceProvider));
    }

    public override async Task<IQueryResponse<MartialAttackRollResult>> ExecuteAsync(MartialAttackRollResultQuery query)
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        var parameters = new DialogParameters<MartialAttackRollResultQueryHandlerModal>
        {
            { x => x.CasterId, query.CasterId },
            { x => x.AttackId, query.AttackId }
        };

        var dialog = await _dialogServiceProvider.GetDialogService().ShowAsync<MartialAttackRollResultQueryHandlerModal>("Martial Attack Roll", parameters, options);
        var result = await dialog.Result;

        if (result is null || result.Canceled)
        {
            return QueryResponse<MartialAttackRollResult>.Canceled(null!);
        }

        return QueryResponse<MartialAttackRollResult>.Success((result.Data as MartialAttackRollResult)!);
    }
}
