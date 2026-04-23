using DnDFightTool.Domain.CharacterSheet.AbilityScores;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Dices.Modifiers;
using DnDFightTool.Domain.CharacterSheet.Saves;
using DnDFightTool.Domain.Rolls;
using DnDFightTool.Domain.Rolls.Extensions;
using DnDFightTool.Domain.Fight;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using DnDFightTool.UI.SharedComponents.Dices;

namespace DnDFightTool.UI.DnDQueryPrompter.SaveQueries;

/// <summary>
///     Modal dialog for entering the result of a saving throw roll.
///     Displays the ability being saved, the resolved DC, saving modifier and total.
/// </summary>
public partial class SaveRollResultQueryHandlerModal : IDisposable
{
    [CascadingParameter]
    public required IMudDialogInstance MudDialog { get; set; }

    [Inject]
    public IFightContext FightContext { get; set; } = null!;

    [Inject]
    public IDiceRollNotifier DiceRollNotifier { get; set; } = null!;

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
    private SaveRollResult? _saveResult;

    internal bool CanValidate => !DiceRollNotifier.CanRoll;

    internal int Total => (_saveResult?.Result ?? 0) + _savingModifier.Modifier;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        DiceRollNotifier.StateChanged += OnDiceRollStateChanged;
        _ability = Save.TargetAbility;
        _saveResult = Save.GetEmptyRollResult();

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

    private void OnDiceRollStateChanged() => InvokeAsync(StateHasChanged);

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        DiceRollNotifier.StateChanged -= OnDiceRollStateChanged;
    }

    private Task ConfirmAsync()
    {
        if (_saveResult is null)
        {
            return Task.CompletedTask;
        }

        MudDialog.Close(DialogResult.Ok(_saveResult));

        return Task.CompletedTask;
    }
}
