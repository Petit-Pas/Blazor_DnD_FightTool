using DnDFightTool.Business.DnDQueries;
using DnDFightTool.Business.DnDQueries.SaveQueries;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using DnDFightTool.Domain.DnDEntities.Saves;
using DnDUserInteractionsComponents.Interactions.MartialAttackUserInteractions;
using FightBlazorComponents.Entities.MartialAttacks;
using MudBlazor;
using UndoableMediator.Queries;

namespace DnDUserInteractionsComponents;

public class SaveRollResultQueryHandler : QueryHandlerBase<SaveRollResultQuery, SaveRollResult>
{
    private readonly IDialogServiceProvider _dialogServiceProvider;

    // TODO considering the dependency, this class should not be here and we should remove the dependency to MudBlazor
    public SaveRollResultQueryHandler(IDialogServiceProvider dialogServiceProvider)
    {
        _dialogServiceProvider = dialogServiceProvider ?? throw new ArgumentNullException(nameof(dialogServiceProvider));
    }

    public override async Task<IQueryResponse<SaveRollResult>> Execute(SaveRollResultQuery query)
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        var dialog = await _dialogServiceProvider.GetDialogService().ShowAsync<Dialog>("Simple Dialog", options);
        var result = await dialog.Result;

        return QueryResponse<SaveRollResult>.Success(result.Data as SaveRollResult);
    }
}
