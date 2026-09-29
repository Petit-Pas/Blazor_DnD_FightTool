using DnDFightTool.UiTests.Framework.Pages;
using DnDFightTool.UiTests.TestableComponents.UI.TestableFightBlazorComponents.CombatStatus;
using DnDFightTool.UiTests.TestableComponents.UI.TestableFightBlazorComponents.Entities.FightingCharacters.Components;
using DnDFightTool.UiTests.TestableComponents.UI.TestableFightBlazorComponents.Entities.MartialAttacks;
using DnDFightTool.UiTests.TestableComponents.UI.TestableFightBlazorComponents.Log;
using Microsoft.Playwright;
using UndoableMediator.Mediators;
using static Microsoft.Playwright.Assertions;

namespace DnDFightTool.UiTests.TestableComponents.Components.TestableDndUi.Shared.Components.Pages;

/// <summary>
///     Page object for the fight dashboard (<c>/fight-dashboard</c>): read and select fighter tiles, and reach the combat
///     log, combat-status and attack-selector fragments as typed component objects. Fighters are arranged with a non-zero
///     initiative before navigation so the dashboard's initiative modal never opens (that modal is story 6).
/// </summary>
public sealed class TestableFightDashboardPage : PageObject
{
    private readonly IUndoableMediator _mediator;

    /// <summary>
    ///     Initializes the dashboard page object.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="baseUrl">The host base URL.</param>
    /// <param name="mediator">The host's undoable mediator, handed to <see cref="CombatStatus"/> for its undo/redo waits.</param>
    public TestableFightDashboardPage(IPage page, string baseUrl, IUndoableMediator mediator) : base(page, baseUrl)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    ///     Navigates to the fight dashboard.
    /// </summary>
    public async Task GoToAsync()
    {
        await GotoAsync("/");
        // Reach the dashboard through a real in-app navigation so the production navigation history is seeded (see
        // StateFullNavigation), keeping any editor "back" from a fighter tile reachable.
        await Page.Locator("a[href='fight-dashboard']").ClickAsync();
        await Expect(Page.Locator(".combat-status-panel")).ToBeVisibleAsync();
    }

    /// <summary>
    ///     Gets the typed object over the fighter tile matching <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The fighter name shown on the tile.</param>
    /// <returns>The fighter tile.</returns>
    public TestableFighterTile FighterTile(string name)
    {
        return new TestableFighterTile(Page, TileRoot(name), BaseUrl);
    }

    /// <summary>
    ///     Selects the fighter tile matching <paramref name="name"/> (only effective once combat has started) and waits for
    ///     it to be marked selected.
    /// </summary>
    /// <param name="name">The fighter name shown on the tile.</param>
    /// <returns>The selected fighter tile.</returns>
    public async Task<TestableFighterTile> SelectFighter(string name)
    {
        var root = TileRoot(name);
        await root.ClickAsync();
        // Token-anchored so it matches the " active" selected class only, not a substring like "inactive"; aligns with TestableFighterTile.IsSelected.
        await Expect(root).ToHaveClassAsync(new System.Text.RegularExpressions.Regex(@"(^|\s)active(\s|$)"));
        return new TestableFighterTile(Page, root, BaseUrl);
    }

    /// <summary>
    ///     Gets the typed object over the combat log panel.
    /// </summary>
    /// <returns>The combat log panel.</returns>
    public TestableCombatLogPanel Log()
    {
        return new TestableCombatLogPanel(Page, Page.Locator(".log-paper"));
    }

    /// <summary>
    ///     Gets the typed object over the combat-status panel.
    /// </summary>
    /// <returns>The combat-status panel.</returns>
    public TestableCombatStatusPanel CombatStatus()
    {
        return new TestableCombatStatusPanel(Page, Page.Locator(".combat-status-panel"), _mediator);
    }

    /// <summary>
    ///     Gets the typed object over the selected fighter's attack selector.
    /// </summary>
    /// <returns>The attack selector.</returns>
    public TestableMartialAttackSelector Attacks()
    {
        return new TestableMartialAttackSelector(Page, Page.Locator(".mud-table"));
    }

    private ILocator TileRoot(string name)
    {
        return Page.Locator(".fighter-card").Filter(new LocatorFilterOptions { HasText = name }).First;
    }
}
