using DnDFightTool.Business.DnDActions.TurnActions.StartNextTurn;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.TurnTracking;
using DnDFightTool.UI.SharedComponents;
using Microsoft.AspNetCore.Components;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.UI.FightBlazorComponents.CombatStatus;

/// <summary>
///     Displays the current round, active turn, and a button to start combat or advance to the next turn.
/// </summary>
public partial class CombatStatusComponent : StylableComponentBase, IDisposable
{
    [Inject]
    private ICombatTurnService _combatTurnService { get; set; } = null!;

    [Inject]
    private IFightContext _fightContext { get; set; } = null!;

    [Inject]
    private IUndoableMediator _mediator { get; set; } = null!;

    private string ButtonLabel => _combatTurnService.IsStarted ? "Next Turn" : "Start Combat";

    private string TurnText => _combatTurnService.CurrentTurnFighter?.Name is { } name
        ? $"{name}'s turn"
        : string.Empty;

    private bool IsDisabled => !_fightContext.Fighters.Any();

    private bool CanUndo => _mediator.HistoryLength > 0;

    private bool CanRedo => _mediator.RedoHistoryLength > 0;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        _combatTurnService.OnChanged += HandleStateChanged;
        _mediator.OnCommandExecuted += HandleMediatorEvent;
        _mediator.OnCommandUndone += HandleMediatorEvent;
        _mediator.OnCommandRedone += HandleMediatorEvent;
    }

    private async Task OnButtonClick()
    {
        await _mediator.SendAsync(new StartNextTurnCommand());
    }

    private async Task OnUndoClick()
    {
        await _mediator.UndoLastCommandAsync();
    }

    private async Task OnRedoClick()
    {
        await _mediator.RedoLastUndoneCommandAsync();
    }

    private void HandleStateChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    private void HandleMediatorEvent(object? sender, ICommand command)
    {
        InvokeAsync(StateHasChanged);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _combatTurnService.OnChanged -= HandleStateChanged;
        _mediator.OnCommandExecuted -= HandleMediatorEvent;
        _mediator.OnCommandUndone -= HandleMediatorEvent;
        _mediator.OnCommandRedone -= HandleMediatorEvent;
    }
}
