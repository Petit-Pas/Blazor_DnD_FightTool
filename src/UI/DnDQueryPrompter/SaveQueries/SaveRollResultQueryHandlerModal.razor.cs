using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Dices.Modifiers;
using DnDFightTool.Domain.DnDEntities.Saves;
using DnDFightTool.Domain.Fight;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DnDQueryPrompter.SaveQueries;

/// <summary>
///     Modal dialog for entering the result of a saving throw roll.
///     Displays the ability being saved, the resolved DC, saving modifier and total.
/// </summary>
public partial class SaveRollResultQueryHandlerModal
{
    [CascadingParameter]
    public required IMudDialogInstance MudDialog { get; set; }

    [Inject]
    public IFightContext FightContext { get; set; } = null!;

    /// <summary>
    ///     The save roll template describing the save to roll.
    /// </summary>
    [Parameter]
    public SaveRollTemplate Save { get; set; } = new();

    /// <summary>
    ///     The id of the caster (character imposing the save).
    /// </summary>
    [Parameter]
    public Guid CasterId { get; set; }

    /// <summary>
    ///     The id of the target (character making the save).
    /// </summary>
    [Parameter]
    public Guid TargetId { get; set; }

    private AbilityEnum _ability;
    private int? _resolvedDc;
    private ScoreModifier _savingModifier = ScoreModifier.Empty;
    private int _diceResult;

    internal bool CanValidate => _diceResult > 0;

    internal int Total => _diceResult + _savingModifier.Modifier;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        _ability = Save.TargetAbility;

        var caster = FightContext[CasterId];
        if (caster is not null)
        {
            _resolvedDc = Save.Difficulty.GetValue(caster);
        }

        var target = FightContext[TargetId];
        if (target is not null)
        {
            _savingModifier = target.AbilityScores.GetSavingModifier(_ability);
        }
    }

    private Task ConfirmAsync()
    {
        var result = Save.GetEmptyRollResult();
        result.RolledResult = _diceResult;

        MudDialog.Close(DialogResult.Ok(result));

        return Task.CompletedTask;
    }
}
