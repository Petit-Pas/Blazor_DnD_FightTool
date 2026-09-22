using System.Text.RegularExpressions;
using Microsoft.Playwright;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;
using static Microsoft.Playwright.Assertions;

namespace DnDFightTool.UiTests.UiTestNavigation.Components;

/// <summary>
///     Typed object over the dashboard's combat-status panel: start combat or advance the turn (both the same production
///     command, no modal), undo and redo the last command, and read the current round, turn text and started state.
/// </summary>
public sealed class TestCombatStatusPanel : ComponentObject
{
    private static readonly TimeSpan EventTimeout = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan RenderSettleTimeout = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan RenderSettlePollInterval = TimeSpan.FromMilliseconds(100);

    private readonly IUndoableMediator _mediator;

    /// <summary>
    ///     Initializes the panel scoped to the combat-status element.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="root">The locator scoping the combat-status panel.</param>
    /// <param name="mediator">
    ///     The host's undoable mediator, shared across every command in the app. Undo and redo target whatever it last
    ///     executed/undid, not necessarily a turn command, so <see cref="Undo"/> and <see cref="Redo"/> await its own
    ///     completion events rather than the panel's DOM text.
    /// </param>
    public TestCombatStatusPanel(IPage page, ILocator root, IUndoableMediator mediator) : base(page, root)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    private ILocator RoundText
    {
        get
        {
            return Root.GetByText(new Regex(@"Round\s*\d+"));
        }
    }

    private ILocator CombatInfo
    {
        get
        {
            return Root.Locator(".combat-info");
        }
    }

    /// <summary>
    ///     Starts combat by clicking the primary button, then waits until the round indicator appears.
    /// </summary>
    public async Task StartCombat()
    {
        await Root.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Start Combat" }).ClickAsync();
        await Expect(RoundText).ToBeVisibleAsync();
    }

    /// <summary>
    ///     Advances to the next turn by clicking the primary button, then waits for the turn text to settle.
    /// </summary>
    public async Task NextTurn()
    {
        var before = await CombatInfo.InnerTextAsync();
        await Root.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Next Turn" }).ClickAsync();
        await Expect(CombatInfo).Not.ToHaveTextAsync(before);
    }

    /// <summary>
    ///     Undoes the last command, then waits for the mediator's own undo-completed event — the last command may be
    ///     anything sent through the shared history, not necessarily one that changes the combat-status text.
    /// </summary>
    public async Task Undo()
    {
        var tcs = new TaskCompletionSource();
        void OnCommandUndone(object? sender, ICommand command)
        {
            tcs.TrySetResult();
        }

        _mediator.OnCommandUndone += OnCommandUndone;
        try
        {
            await Root.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Undo" }).ClickAsync();
            await tcs.Task.WaitAsync(EventTimeout);
        }
        finally
        {
            _mediator.OnCommandUndone -= OnCommandUndone;
        }
    }

    /// <summary>
    ///     Redoes the last undone command, then waits for the mediator's own redo-completed event — the last undone
    ///     command may be anything sent through the shared history, not necessarily one that changes the combat-status
    ///     text.
    /// </summary>
    public async Task Redo()
    {
        var tcs = new TaskCompletionSource();
        void OnCommandRedone(object? sender, ICommand command)
        {
            tcs.TrySetResult();
        }

        _mediator.OnCommandRedone += OnCommandRedone;
        try
        {
            await Root.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Redo" }).ClickAsync();
            await tcs.Task.WaitAsync(EventTimeout);
        }
        finally
        {
            _mediator.OnCommandRedone -= OnCommandRedone;
        }
    }

    /// <summary>
    ///     Reads the current round number.
    /// </summary>
    /// <returns>The current round, or <c>0</c> when combat has not started.</returns>
    public async Task<int> GetRound()
    {
        if (await RoundText.CountAsync() == 0)
        {
            return 0;
        }

        var text = await RoundText.InnerTextAsync();
        return int.Parse(Regex.Match(text, @"\d+").Value);
    }

    /// <summary>
    ///     Reads the turn text (e.g. the active fighter's turn), or the idle message when combat has not started.
    /// </summary>
    /// <returns>The current combat-info body text.</returns>
    public async Task<string> GetTurnText()
    {
        var text = await CombatInfo.Locator(".mud-body1").InnerTextAsync();
        return text.Trim();
    }

    /// <summary>
    ///     Whether combat is currently started. Polls briefly, since <see cref="Undo"/>/<see cref="Redo"/> only await the
    ///     mediator's own completion event, which can race the subsequent Blazor render.
    /// </summary>
    /// <returns><c>true</c> when a round is shown.</returns>
    public async Task<bool> IsCombatStarted()
    {
        var deadline = DateTime.UtcNow + RenderSettleTimeout;
        while (true)
        {
            if (await RoundText.CountAsync() > 0)
            {
                return true;
            }

            if (DateTime.UtcNow >= deadline)
            {
                return false;
            }

            await Task.Delay(RenderSettlePollInterval);
        }
    }
}
