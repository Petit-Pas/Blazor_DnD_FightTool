using DnDEntities.Characters;
using Fight;
using Fight.Characters;
using Microsoft.AspNetCore.Components;

namespace FightBlazorComponents.Pages;

public partial class FightPage
{
    [Inject]
    public IFightContext FightContext { get; set; }

    [Inject]
    public ICharacterRepository CharacterRepository { get; set; }

    private Character? MovingCharacter = null;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ResetMovingCharacter();
        FightContext.MovingCharacterChanged += FightContext_MovingCharacterChanged;
    }

    private void FightContext_MovingCharacterChanged(object? sender, FightingCharacter? e)
    {
        ResetMovingCharacter();
    }

    private void ResetMovingCharacter()
    {
        MovingCharacter = FightContext.GetMovingCharacter();
        StateHasChanged();
    }
}
