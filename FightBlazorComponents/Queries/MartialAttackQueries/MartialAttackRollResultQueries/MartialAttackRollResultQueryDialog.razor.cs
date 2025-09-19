using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using Microsoft.AspNetCore.Components;

namespace FightBlazorComponents.Queries.MartialAttackQueries.MartialAttackRollResultQueries;

public partial class MartialAttackRollResultQueryDialog
{
    [Parameter]
    public MartialAttackRollResult? MartialAttackRollResult { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }
}
