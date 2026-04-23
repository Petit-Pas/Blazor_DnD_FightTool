using DnDFightTool.Domain.DnDEntities.Dices;
using DnDFightTool.Domain.Fight.Characters;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedComponents.Dices;

namespace FightBlazorComponents.Entities.FightingCharacters.Dialog;

public partial class InitiativeInputDialog : IDisposable
{
    [CascadingParameter]
    public required IMudDialogInstance MudDialog { get; set; }

    [Inject]
    public IDiceRollNotifier DiceRollNotifier { get; set; } = null!;

    [Parameter]
    public FightingCharacter[] Fighters { get; set; } = [];

    private (FightingCharacter Fighter, RawD20RollResult Roll)[] _rows = [];

    private bool CanValidate => !DiceRollNotifier.CanRoll;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        DiceRollNotifier.StateChanged += OnDiceRollStateChanged;
        _rows = Fighters.Select(f => (f, new RawD20RollResult())).ToArray();
    }

    private void OnDiceRollStateChanged() => InvokeAsync(StateHasChanged);

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        DiceRollNotifier.StateChanged -= OnDiceRollStateChanged;
    }

    private Task CloseAsync()
    {
        foreach (var (fighter, roll) in _rows)
        {
            fighter.InitiativeRoll = roll.Result;
        }

        MudDialog.Close(DialogResult.Ok(true));

        return Task.CompletedTask;
    }
}
