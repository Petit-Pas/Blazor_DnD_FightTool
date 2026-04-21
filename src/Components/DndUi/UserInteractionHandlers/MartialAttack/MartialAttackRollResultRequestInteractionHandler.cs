using DnDFightTool.Business.DnDUserInteraction.MartialAttackUserInteractions;
using DnDFightTool.Domain.Fight.DomainExtensions.MartialAttacks;
using DnDUserInteractionsComponents.Interactions.MartialAttackUserInteractions;
using MudBlazor;
using UndoableMediator.Queries;

namespace DndUi.Components.Layout;

public partial class MainLayout
{
    private async Task HandleInteractionAsync(MartialAttackRollResultRequestInteraction interaction)
    {
        var caster = _fightContext[interaction.CasterId] ?? throw new NullReferenceException($"{typeof(MartialAttackRollResultRequestInteraction)} could not find caster with id {interaction.CasterId}");
        var attackTemplate = caster.MartialAttacks.GetTemplateByIdOrDefault(interaction.AttackId) ?? throw new NullReferenceException($"{typeof(MartialAttackRollResultRequestInteraction)} could not find martial attack template with id {interaction.AttackId} for caster with id {interaction.CasterId}");

        var rollableResult = attackTemplate.GetRollableResult();

        var options = new DialogOptions { BackdropClick = false, CloseButton = true };
        var parameters = new DialogParameters<MartialAttackRollResultRequestInteractionModal>
        {
            { x => x.MartialAttackRollResult, rollableResult }
        };

        var dialog = await Dialogs.ShowAsync<MartialAttackRollResultRequestInteractionModal>($"{caster.Name} uses {attackTemplate.Name}", parameters, options);
        var result = await dialog.Result;

        if (result?.Canceled ?? true)
        {
            UserInteractionService.CompleteCanceled(interaction.InteractionId);
            return;
        }
        UserInteractionService.CompleteSuccess(interaction.InteractionId, rollableResult);
    }
}
