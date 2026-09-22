using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace DnDFightTool.UiTests.UiTestNavigation.Pages;

/// <summary>
///     Page object for the character list editor (<c>/</c>, <c>/Characters</c>): switch between the Players and Monsters
///     tabs, create/edit/duplicate/delete a character, and read the listed names. Create/edit/duplicate drive the real
///     production navigation to the character editor and return its page object, because the editor renders only from a
///     scoped edit context this process cannot seed.
/// </summary>
public sealed class TestCharacterListEditorPage : PageObject
{
    private const string PlayersTab = "Players";
    private const string MonstersTab = "Monsters";

    /// <summary>
    ///     Initializes the list page object.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="baseUrl">The host base URL.</param>
    public TestCharacterListEditorPage(IPage page, string baseUrl) : base(page, baseUrl)
    {
    }

    // The bottom "add" control is a full-width MudButton with no accessible name (it is not an atomic preset, and adding an
    // aria-label to it is outside this story's sanctioned production change). Within the active, visible tab panel it is the
    // only full-width button, so it is located structurally here — inside the page object, never in a scenario.
    private ILocator AddNewButton
    {
        get
        {
            return Page.Locator("button.w-100:visible");
        }
    }

    // Rows are the paper cards carrying the hover action row; ":visible" keeps them to the active tab even if MudTabs keeps
    // inactive panels in the DOM.
    private ILocator ActiveRows
    {
        get
        {
            return Page.Locator(".mud-paper:has(.hoverable-button-row):visible");
        }
    }

    /// <summary>
    ///     Navigates to the character list home page.
    /// </summary>
    public async Task GoToAsync()
    {
        await GotoAsync("/");
        // Follow with a real in-app navigation so the production navigation records this page in its history; otherwise the
        // editor's "back" falls through to its unreachable bootstrap URL. See StateFullNavigation.
        await Page.Locator("a[href='characters']").ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Tab, new PageGetByRoleOptions { Name = PlayersTab, Exact = true }))
            .ToBeVisibleAsync();
    }

    /// <summary>
    ///     Switches to the Players tab.
    /// </summary>
    public async Task GoToPlayersTab()
    {
        await GoToTab(PlayersTab);
    }

    /// <summary>
    ///     Switches to the Monsters tab.
    /// </summary>
    public async Task GoToMonstersTab()
    {
        await GoToTab(MonstersTab);
    }

    /// <summary>
    ///     Creates a new player and opens its editor.
    /// </summary>
    /// <returns>The character editor for the new player.</returns>
    public async Task<TestCharacterEditorPage> CreatePlayer()
    {
        return await Create(PlayersTab);
    }

    /// <summary>
    ///     Creates a new monster and opens its editor.
    /// </summary>
    /// <returns>The character editor for the new monster.</returns>
    public async Task<TestCharacterEditorPage> CreateMonster()
    {
        return await Create(MonstersTab);
    }

    /// <summary>
    ///     Opens the editor for the player whose row matches <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The player name shown in the row.</param>
    /// <returns>The character editor for that player.</returns>
    public async Task<TestCharacterEditorPage> EditPlayer(string name)
    {
        return await RowAction(PlayersTab, name, "Edit");
    }

    /// <summary>
    ///     Opens the editor for the monster whose row matches <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The monster name shown in the row.</param>
    /// <returns>The character editor for that monster.</returns>
    public async Task<TestCharacterEditorPage> EditMonster(string name)
    {
        return await RowAction(MonstersTab, name, "Edit");
    }

    /// <summary>
    ///     Opens the editor for a duplicate of the player whose row matches <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The player name shown in the row.</param>
    /// <returns>The character editor for the duplicate.</returns>
    public async Task<TestCharacterEditorPage> DuplicatePlayer(string name)
    {
        return await RowAction(PlayersTab, name, "Duplicate");
    }

    /// <summary>
    ///     Opens the editor for a duplicate of the monster whose row matches <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The monster name shown in the row.</param>
    /// <returns>The character editor for the duplicate.</returns>
    public async Task<TestCharacterEditorPage> DuplicateMonster(string name)
    {
        return await RowAction(MonstersTab, name, "Duplicate");
    }

    /// <summary>
    ///     Deletes the player whose row matches <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The player name shown in the row.</param>
    public async Task DeletePlayer(string name)
    {
        await Delete(PlayersTab, name);
    }

    /// <summary>
    ///     Deletes the monster whose row matches <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The monster name shown in the row.</param>
    public async Task DeleteMonster(string name)
    {
        await Delete(MonstersTab, name);
    }

    /// <summary>
    ///     Reads the names of the listed players.
    /// </summary>
    /// <returns>The player names in list order.</returns>
    public async Task<IReadOnlyList<string>> PlayerNames()
    {
        return await Names(PlayersTab);
    }

    /// <summary>
    ///     Reads the names of the listed monsters.
    /// </summary>
    /// <returns>The monster names in list order.</returns>
    public async Task<IReadOnlyList<string>> MonsterNames()
    {
        return await Names(MonstersTab);
    }

    private async Task GoToTab(string tabName)
    {
        var tab = Page.GetByRole(AriaRole.Tab, new PageGetByRoleOptions { Name = tabName, Exact = true });
        await Expect(tab).ToBeVisibleAsync();
        await tab.ClickAsync();
        await Expect(tab).ToHaveAttributeAsync("aria-selected", "true");
    }

    private async Task<TestCharacterEditorPage> Create(string tabName)
    {
        await GoToTab(tabName);
        await AddNewButton.ClickAsync();
        await WaitForCharacterEditor();
        return new TestCharacterEditorPage(Page, BaseUrl);
    }

    private async Task<TestCharacterEditorPage> RowAction(string tabName, string name, string action)
    {
        await GoToTab(tabName);
        var row = RowNamed(name);
        // The row's action buttons are hidden (opacity 0, shifted) until the row is hovered; hover reveals them and a force
        // click avoids waiting on the reveal animation to settle.
        await row.HoverAsync();
        await row.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = action })
            .ClickAsync(new LocatorClickOptions { Force = true });
        await WaitForCharacterEditor();
        return new TestCharacterEditorPage(Page, BaseUrl);
    }

    // The character editor is reached by a same-document (SPA) navigation, for which URL waits are unreliable; wait instead
    // for a marker unique to that page — the "Abilities & Skills" tab, which the attack editor does not have.
    private async Task WaitForCharacterEditor()
    {
        await Expect(Page.GetByRole(AriaRole.Tab, new PageGetByRoleOptions { Name = "Abilities & Skills", Exact = true }))
            .ToBeVisibleAsync();
    }

    private async Task Delete(string tabName, string name)
    {
        await GoToTab(tabName);
        var before = await ActiveRows.CountAsync();
        var row = RowNamed(name);
        await row.HoverAsync();
        await row.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Delete" })
            .ClickAsync(new LocatorClickOptions { Force = true });
        await Expect(ActiveRows).ToHaveCountAsync(before - 1);
    }

    private async Task<IReadOnlyList<string>> Names(string tabName)
    {
        await GoToTab(tabName);
        await Expect(AddNewButton).ToBeVisibleAsync();

        var count = await ActiveRows.CountAsync();
        var names = new List<string>();
        for (var index = 0; index < count; index++)
        {
            var text = await ActiveRows.Nth(index).InnerTextAsync();
            names.Add(text.Trim());
        }

        return names;
    }

    private ILocator RowNamed(string name)
    {
        return ActiveRows.Filter(new LocatorFilterOptions { HasText = name }).First;
    }
}
