using DnDFightTool.Domain.Rolls;
using DnDFightTool.Domain.Fight.Characters;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using DnDFightTool.UI.SharedComponents.Dices;

namespace DnDFightTool.UI.FightBlazorComponents.Entities.FightingCharacters.Dialog;

public partial class InitiativeInputDialog : IDisposable
{
    [CascadingParameter]
    public required IMudDialogInstance MudDialog { get; set; }

    [Inject]
    public IDiceRollNotifier DiceRollNotifier { get; set; } = null!;

    [Parameter]
    public IFightingCharacter[] Fighters { get; set; } = [];

    private (IFightingCharacter Fighter, RawD20RollResult Roll)[] _rows = [];

    private bool CanValidate => !DiceRollNotifier.CanRoll;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        DiceRollNotifier.StateChanged += OnDiceRollStateChanged;
        _rows = [.. Fighters.Select(f => (f, new RawD20RollResult()))];
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
