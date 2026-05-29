using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.Rolls;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using Microsoft.AspNetCore.Components;

namespace DnDFightTool.UI.DnDQueryPrompter.MartialAttackQueries;

public partial class MartialAttackRollResultRequestInteractionModal
{
    [Inject]
    public IFightContext FightContext { get; set; } = null!;

    [Parameter]
    public MartialAttackRollResult? MartialAttackRollResult { get; set; }

    private string _filter = string.Empty;
    private IFightingCharacter[] _fighters = [];

    protected override void OnInitialized()
    {
        _fighters = [ .. FightContext.Fighters ];
    }

    public void FilterChanged(string newFilter)
    {
        _filter = newFilter;
    }
}
