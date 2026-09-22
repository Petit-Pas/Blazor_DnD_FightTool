using System.Text.RegularExpressions;
using DnDFightTool.UiTests.UiTestNavigation.Pages;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace DnDFightTool.UiTests.UiTestNavigation.Components;

/// <summary>
///     Typed object over a single fighter tile on the dashboard: read its name, hit points, initiative and applied
///     statuses, edit it (driving the real navigation to the character editor) or remove it from the fight. Selecting a
///     tile is driven from <see cref="Pages.TestFightDashboardPage.SelectFighter"/>, since selection is a page-level intent.
/// </summary>
public sealed class TestFighterTile : ComponentObject
{
    private readonly string _baseUrl;

    /// <summary>
    ///     Initializes the tile scoped to its card element, carrying the host base URL so <see cref="Edit"/> can return the
    ///     character editor page.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="root">The locator scoping this fighter's card.</param>
    /// <param name="baseUrl">The host base URL, forwarded to the returned character editor page.</param>
    public TestFighterTile(IPage page, ILocator root, string baseUrl) : base(page, root)
    {
        _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
    }

    /// <summary>
    ///     Reads the fighter's name from the tile.
    /// </summary>
    /// <returns>The displayed fighter name.</returns>
    public async Task<string> GetName()
    {
        var text = await Root.Locator("h4").First.InnerTextAsync();
        return text.Trim();
    }

    /// <summary>
    ///     Reads the fighter's current hit points.
    /// </summary>
    /// <returns>The current HP value shown on the tile.</returns>
    public async Task<int> GetCurrentHp()
    {
        var (current, _) = await ReadHitPoints();
        return current;
    }

    /// <summary>
    ///     Reads the fighter's maximum hit points.
    /// </summary>
    /// <returns>The maximum HP value shown on the tile.</returns>
    public async Task<int> GetMaxHp()
    {
        var (_, max) = await ReadHitPoints();
        return max;
    }

    /// <summary>
    ///     Reads the fighter's initiative total.
    /// </summary>
    /// <returns>The initiative value shown on the tile.</returns>
    public async Task<int> GetInitiative()
    {
        var text = await Root.GetByText(new Regex("Initiative")).First.InnerTextAsync();
        var match = Regex.Match(text, @"-?\d+");
        return int.Parse(match.Value);
    }

    /// <summary>
    ///     Reads the names of the status chips currently applied to the fighter.
    /// </summary>
    /// <returns>The applied status names, empty when the fighter has none.</returns>
    public async Task<IReadOnlyList<string>> Statuses()
    {
        var chips = Root.Locator(".mud-chip");
        var texts = await chips.AllInnerTextsAsync();
        return [.. texts.Select(text => text.Trim())];
    }

    /// <summary>
    ///     Whether the tile is currently marked as the selected fighter.
    /// </summary>
    /// <returns><c>true</c> when the tile carries the selected styling.</returns>
    public async Task<bool> IsSelected()
    {
        var css = await Root.GetAttributeAsync("class") ?? string.Empty;
        return css.Split(' ').Contains("active");
    }

    /// <summary>
    ///     Opens the character editor for this fighter, driving the real edit navigation.
    /// </summary>
    /// <returns>The character editor page for this fighter.</returns>
    public async Task<TestCharacterEditorPage> Edit()
    {
        await Root.HoverAsync();
        await Root.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Edit" })
            .ClickAsync(new LocatorClickOptions { Force = true });
        // Same-document navigation; wait for a marker unique to the character editor rather than a URL.
        await Expect(Page.GetByRole(AriaRole.Tab, new PageGetByRoleOptions { Name = "Abilities & Skills", Exact = true }))
            .ToBeVisibleAsync();
        return new TestCharacterEditorPage(Page, _baseUrl);
    }

    /// <summary>
    ///     Removes this fighter from the fight.
    /// </summary>
    public async Task Delete()
    {
        await Root.HoverAsync();
        await Root.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Delete" })
            .ClickAsync(new LocatorClickOptions { Force = true });
    }

    private async Task<(int Current, int Max)> ReadHitPoints()
    {
        var text = await Root.GetByText(new Regex(@"\d+\s*/\s*\d+")).First.InnerTextAsync();
        var match = Regex.Match(text, @"(\d+)\s*/\s*(\d+)");
        return (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
    }
}
