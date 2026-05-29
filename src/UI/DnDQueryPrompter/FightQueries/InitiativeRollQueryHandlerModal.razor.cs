using DnDFightTool.Domain.CharacterSheet.AbilityScores;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Dices.Modifiers;
using DnDFightTool.Domain.Rolls;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using DnDFightTool.UI.SharedComponents.Dices;

namespace DnDFightTool.UI.DnDQueryPrompter.FightQueries;

/// <summary>
///     Modal dialog prompting the user for a single character's initiative roll (raw d20).
///     Displays the character's dexterity modifier informationally.
/// </summary>
public partial class InitiativeRollQueryHandlerModal : IDisposable
{
    [CascadingParameter]
    public required IMudDialogInstance MudDialog { get; set; }

    [Inject]
    public ICharacterRepository CharacterRepository { get; set; } = null!;

    [Inject]
    public IDiceRollNotifier DiceRollNotifier { get; set; } = null!;

    /// <summary>
    ///     The id of the character whose initiative is being rolled.
    ///     At prompt time the character has not yet been added to the fight,
    ///     so resolution always goes through <see cref="ICharacterRepository"/>.
    /// </summary>
    [Parameter]
    public Guid CharacterId { get; set; }

    private RawD20RollResult _d20Roll = new();
    private ScoreModifier _dexterityModifier = ScoreModifier.Empty;

    internal bool CanValidate => !DiceRollNotifier.CanRoll;

    internal int Total => _d20Roll.Result + _dexterityModifier.Modifier;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        DiceRollNotifier.StateChanged += OnDiceRollStateChanged;

        var character = CharacterRepository.GetCharacterById(CharacterId);
        if (character is not null)
        {
            _dexterityModifier = character.AbilityScores.GetModifier(AbilityEnum.Dexterity);
        }
    }

    private void OnDiceRollStateChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        DiceRollNotifier.StateChanged -= OnDiceRollStateChanged;
    }

    private Task ConfirmAsync()
    {
        MudDialog.Close(DialogResult.Ok(_d20Roll.Result));
        return Task.CompletedTask;
    }
}
