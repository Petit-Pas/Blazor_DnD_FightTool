using DnDEntities.Characters;
using Fight;
using Microsoft.AspNetCore.Components;

namespace FightBlazorComponents.Entities.FightingCharacter.Components;

public partial class FightingCharacterTile : ComponentBase
{
    [Inject]
    public IFightContext FightContext { get; set; }

    [Parameter]
    public Guid CharacterId { get; set; } = Guid.Empty;

    private Character? Character = null;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        InitCharacter();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        InitCharacter();
    }

    private void InitCharacter()
    {
        if (Character == null || Character.Id != CharacterId)
        {
            Character = FightContext[CharacterId];
        }
    }
}