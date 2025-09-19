using DnDFightTool.Domain.Fight.Characters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FightBlazorComponents.Entities.FightingCharacters.Dialog;

public partial class InitiativeInputDialog
{
    [CascadingParameter]
    public required IMudDialogInstance MudDialog { get; set; }

    [Parameter]
    public FightingCharacter[] Fighters { get; set; } = [];

    private bool CanValidate => !Fighters.Any(x => x.InitiativeRoll == 0);

    private bool CanRoll => !CanValidate;

    private Task CloseAsync()
    {
        MudDialog.Close(DialogResult.Ok(true));

        return Task.CompletedTask;
    }

    private Task RollAsync()
    {
        // TODO clean this up
        var random = new Random();
        foreach (var fighter in Fighters.Where(x => x.InitiativeRoll == 0))
        {
            fighter.InitiativeRoll = random.Next(1, 20);
        }

        return Task.CompletedTask;
    }

}
