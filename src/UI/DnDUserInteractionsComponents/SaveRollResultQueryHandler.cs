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
    private readonly IDialogService _dialogs;

    // TODO considering the dependency, this class should not be here and we should remove the dependency to MudBlazor
    public SaveRollResultQueryHandler(IDialogService dialogs)
    {
        _dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
    }

    public override async Task<IQueryResponse<SaveRollResult>> Execute(SaveRollResultQuery query)
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        var areTheSame = _dialogs == MartialAttackSelectorComponent.SingletonDialogService;

        var dialog = await _dialogs.ShowAsync<Dialog>("Simple Dialog", options);
        //var dialog = await MartialAttackSelectorComponent.SingletonDialogService.ShowAsync<Dialog>("Simple Dialog", options);
        var result = await dialog.Result;

        return default;
    }
}
