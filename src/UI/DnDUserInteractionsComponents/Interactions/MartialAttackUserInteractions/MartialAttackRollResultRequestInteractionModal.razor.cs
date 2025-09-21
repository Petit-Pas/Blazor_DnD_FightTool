using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using Microsoft.AspNetCore.Components;

namespace DnDUserInteractionsComponents.Interactions.MartialAttackUserInteractions;

public partial class MartialAttackRollResultRequestInteractionModal
{
    [Parameter]
    public MartialAttackRollResult? MartialAttackRollResult { get; set; }
}
