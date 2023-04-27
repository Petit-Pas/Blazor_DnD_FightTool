using DnDEntities.Characters;
using Fight;
using Microsoft.AspNetCore.Components;
using Fight.Characters;
using Microsoft.AspNetCore.Components.Web;
using NeoBlazorphic.StyleParameters;

namespace FightBlazorComponents.Entities.FightingCharacters.Components;

public partial class FightingCharacterTile : ComponentBase, IDisposable
{
    [Inject]
    public ICharacterRepository CharacterRepository { get; set; }

    [Inject]
    public IFightContext FightContext { get; set; }

    [Parameter]
    public FightingCharacter Fighter { get; set; }

    private Character? Character = null;

    private BorderRadius BorderRadius = new (2, "em");

    protected override void OnInitialized()
    {
        base.OnInitialized();
        InitCharacter();

        FightContext.MovingCharacterChanged += OnMovingCharacterChanged;
    }

    private void OnMovingCharacterChanged(object? sender, FightingCharacter? fightingCharacter)
    {
        StateHasChanged();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        InitCharacter();
    }

    private void InitCharacter()
    {
        if (Character == null || Character.Id != Fighter.CharacterId)
        {
            Character = CharacterRepository.GetCharacterById(Fighter.CharacterId);
        }
    }

    public void Dispose()
    {
        FightContext.MovingCharacterChanged -= OnMovingCharacterChanged;
    }

    private void TileClicked(MouseEventArgs _)
    {
        FightContext.SetFightingCharacter(Fighter);
    }

    // UI Methods
    private ThemeColor CardTheme => FightContext.MovingCharacter == Fighter
        ? ThemeColor.Primary 
        : ThemeColor.Base;
}