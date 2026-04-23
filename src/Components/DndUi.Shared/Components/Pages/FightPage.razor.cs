using DnDFightTool.Business.DnDQueries;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using DnDFightTool.UI.FightBlazorComponents.Entities.FightingCharacters.Dialog;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DndUi.Shared.Components.Pages;

public partial class FightPage : IDisposable
{
    [Inject]
    public required IDialogServiceProvider DialogServiceProvider { get; set; }

    [Inject]
    public required IDialogService DialogService { get; set; }

    [Inject]
    public required IFightContext FightContext { get; set; }

    protected async override Task OnInitializedAsync()
    {
        FightContext.OnFighterRemoved += FighterRemoved;

        DialogServiceProvider.SetDialogService(DialogService);

        await CheckForFightersWithoutInitiative();
    }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await CheckForFightersWithoutInitiative();
        }
    }

    private async Task CheckForFightersWithoutInitiative()
    {
        if (FightContext.Fighters.Any(x => x.InitiativeRoll == 0))
        {
            var options = new DialogOptions { BackdropClick = false };
            var parameters = new DialogParameters<InitiativeInputDialog>
            {
                { x => x.Fighters, FightContext.Fighters.ToArray() }
            };

            var dialog = await DialogService.ShowAsync<InitiativeInputDialog>("Roll for Initiative!", parameters, options);
            await dialog.Result;

            // TODO in async method, StateHasChanged should always be called via InvokeAsync
            await InvokeAsync(StateHasChanged);
        }
    }

    public void FighterRemoved(object? _, FightingCharacter __)
    {
        StateHasChanged();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        FightContext.OnFighterRemoved -= FighterRemoved;
    }
}
