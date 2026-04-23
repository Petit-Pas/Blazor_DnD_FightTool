using DnDFightTool.Domain.CharacterSheet.MartialAttacks;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using Microsoft.AspNetCore.Components;

namespace DnDFightTool.UI.FightBlazorComponents.Entities.FightingCharacters.Components;

public partial class FightingCharacterSelectorComponent
{
    [Inject]
    public IFightContext FightContext { get; set; } = null!;

    [Parameter]
    public int Max { get; set; } = 1;

    private string _filter = string.Empty;
    private FightingCharacter[] _fighters = [];

    protected override void OnInitialized()
    {
        _fighters = [.. FightContext.Fighters];
    }

    public void FilterChanged(string newFilter)
    {
        _filter = newFilter;
    }
}
