using DnDFightTool.Domain.CharacterSheet.AbilityScores;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CharacterSheetBlazorComponents.AbilityScores;

public partial class AbilityChip
{
    [Parameter]
    public EventCallback OnClick { get; set; }

    [Parameter]
    public AbilityEnum Ability { get; set; }

    [Parameter]
    public string? BadgeContent { get; set; }

    [Parameter]
    public Variant Variant { get; set; } = Variant.Filled;

    [Parameter]
    public Color Color { get; set; } = Color.Default;

    [Parameter]
    public Color BadgeColor { get; set; } = Color.Secondary;
}
