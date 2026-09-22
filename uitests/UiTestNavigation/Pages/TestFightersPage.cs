using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace DnDFightTool.UiTests.UiTestNavigation.Pages;

/// <summary>
///     Page object for the fighters page (<c>/fighters</c>): search the available players and monsters, add one to the
///     fight, remove one from it, and read the three lists. The available and in-fight groups are located by their section
///     headers so the two search boxes and the repeated row shape never collide.
/// </summary>
public sealed class TestFightersPage : PageObject
{
    /// <summary>
    ///     Initializes the fighters page object.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="baseUrl">The host base URL.</param>
    public TestFightersPage(IPage page, string baseUrl) : base(page, baseUrl)
    {
    }

    private ILocator PlayersGroup
    {
        get
        {
            return Page.Locator(".group-section:has-text('Players') .group-list");
        }
    }

    private ILocator MonstersGroup
    {
        get
        {
            return Page.Locator(".group-section:has-text('Monsters') .group-list");
        }
    }

    private ILocator InFightGroup
    {
        get
        {
            return Page.Locator(".fighters-column:has-text('In Fight') .group-list");
        }
    }

    /// <summary>
    ///     Navigates to the fighters page.
    /// </summary>
    public async Task GoToAsync()
    {
        await GotoAsync("/");
        // Reach the page through a real in-app navigation so the production navigation history is seeded (see
        // StateFullNavigation), keeping any editor "back" from this page reachable.
        await Page.Locator("a[href='fighters']").ClickAsync();
        await Expect(Page.GetByPlaceholder("Search players...")).ToBeVisibleAsync();
    }

    /// <summary>
    ///     Filters the available players by <paramref name="text"/>.
    /// </summary>
    /// <param name="text">The search text; pass empty to clear the filter.</param>
    public async Task SearchPlayers(string text)
    {
        await Search("Search players...", PlayersGroup, text);
    }

    /// <summary>
    ///     Filters the available monsters by <paramref name="text"/>.
    /// </summary>
    /// <param name="text">The search text; pass empty to clear the filter.</param>
    public async Task SearchMonsters(string text)
    {
        await Search("Search monsters...", MonstersGroup, text);
    }

    /// <summary>
    ///     Adds the available player matching <paramref name="name"/> to the fight.
    /// </summary>
    /// <param name="name">The player name shown in the available list.</param>
    public async Task AddPlayer(string name)
    {
        await Add(PlayersGroup, name);
    }

    /// <summary>
    ///     Adds the available monster matching <paramref name="name"/> to the fight.
    /// </summary>
    /// <param name="name">The monster name shown in the available list.</param>
    public async Task AddMonster(string name)
    {
        await Add(MonstersGroup, name);
    }

    /// <summary>
    ///     Removes the in-fight entry matching <paramref name="name"/> from the fight (one instance).
    /// </summary>
    /// <param name="name">The name shown in the in-fight list.</param>
    public async Task RemoveFromFight(string name)
    {
        var before = await InFightGroup.InnerTextAsync();
        await InFightRow(name).First.GetByRole(AriaRole.Button).ClickAsync();
        await Expect(InFightGroup).Not.ToHaveTextAsync(before);
    }

    /// <summary>
    ///     Reads the names of the available players.
    /// </summary>
    /// <returns>The available player names.</returns>
    public async Task<IReadOnlyList<string>> AvailablePlayerNames()
    {
        return await RowTexts(PlayersGroup);
    }

    /// <summary>
    ///     Reads the names of the available monsters.
    /// </summary>
    /// <returns>The available monster names.</returns>
    public async Task<IReadOnlyList<string>> AvailableMonsterNames()
    {
        return await RowTexts(MonstersGroup);
    }

    /// <summary>
    ///     Reads the entries currently in the fight (each line includes its initiative).
    /// </summary>
    /// <returns>The in-fight entry texts.</returns>
    public async Task<IReadOnlyList<string>> InFightNames()
    {
        return await RowTexts(InFightGroup);
    }

    private async Task Search(string placeholder, ILocator group, string text)
    {
        await Page.GetByPlaceholder(placeholder).FillAsync(text);
        if (!string.IsNullOrEmpty(text))
        {
            await Expect(group.Locator(".fighter-row").Filter(new LocatorFilterOptions { HasNotText = text }))
                .ToHaveCountAsync(0);
        }
    }

    private async Task Add(ILocator group, string name)
    {
        var before = await InFightGroup.InnerTextAsync();
        await group.Locator(".fighter-row").Filter(new LocatorFilterOptions { HasText = name }).First
            .GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Add" }).ClickAsync();
        await Expect(InFightGroup).Not.ToHaveTextAsync(before);
    }

    private ILocator InFightRow(string name)
    {
        return InFightGroup.Locator(".fighter-row").Filter(new LocatorFilterOptions { HasText = name });
    }

    private static async Task<IReadOnlyList<string>> RowTexts(ILocator group)
    {
        var rows = group.Locator(".fighter-row");
        var count = await rows.CountAsync();
        var texts = new List<string>();
        for (var index = 0; index < count; index++)
        {
            var text = await rows.Nth(index).InnerTextAsync();
            texts.Add(text.Trim());
        }

        return texts;
    }
}
