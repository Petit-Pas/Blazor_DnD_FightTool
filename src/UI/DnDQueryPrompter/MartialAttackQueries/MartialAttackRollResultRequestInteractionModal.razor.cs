using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using Microsoft.AspNetCore.Components;

namespace DnDQueryPrompter.MartialAttackQueries;

public partial class MartialAttackRollResultRequestInteractionModal
{
    [Inject]
    public IFightContext FightContext { get; set; } = null!;

    [Parameter]
    public MartialAttackRollResult? MartialAttackRollResult { get; set; }

    private string _filter = string.Empty;
    private FightingCharacter[] _fighters = [];

    protected override void OnInitialized()
    {
        _fighters = [ .. FightContext.Fighters ];
    }

    public void FilterChanged(string newFilter)
    {
        _filter = newFilter;
    }
}
