using DnDFightTool.UiTests.UiTestNavigation.Pages;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace DnDFightTool.UiTests.UiTestNavigation.Components;

/// <summary>
///     Typed object over the attack-list editor hosted on the character editor's "Attacks" tab: add a new attack, or edit,
///     duplicate and delete an existing one by name. Add/edit/duplicate drive the real production navigation to the attack
///     editor and return its page object, because the attack editor renders only from a scoped edit context.
/// </summary>
public sealed class TestAttackListEditor : ComponentObject
{
    private readonly string _baseUrl;

    /// <summary>
    ///     Initializes the editor scoped to the attack-list card, carrying the host base URL so it can return the attack
    ///     editor page object reached by add/edit/duplicate.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="root">The locator scoping the attack-list card.</param>
    /// <param name="baseUrl">The host base URL, forwarded to the returned attack editor page.</param>
    public TestAttackListEditor(IPage page, ILocator root, string baseUrl) : base(page, root)
    {
        _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
    }

    private ILocator Rows
    {
        get
        {
            return Root.Locator(".attack-list > .mud-paper");
        }
    }

    private ILocator RowNamed(string name)
    {
        return Rows.Filter(new LocatorFilterOptions { HasText = name }).First;
    }

    private async static Task ClickOnRowButton(ILocator row, string buttonName)
    {
        // The row's action buttons are hidden (opacity 0, shifted) until the row is hovered; hover reveals them and a force
        // click avoids waiting on the reveal animation to settle.
        await row.HoverAsync();
        await row.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = buttonName })
            .ClickAsync(new LocatorClickOptions { Force = true });
    }

    /// <summary>
    ///     Opens the editor for a brand-new attack by clicking the add control.
    /// </summary>
    /// <returns>The attack editor page for the new attack.</returns>
    public async Task<TestAttackEditorPage> AddAttack()
    {
        await Root.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Add" }).ClickAsync();
        return await OpenedAttackEditor();
    }

    /// <summary>
    ///     Opens the editor for the existing attack whose row matches <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The attack name shown in the row.</param>
    /// <returns>The attack editor page for that attack.</returns>
    public async Task<TestAttackEditorPage> EditAttack(string name)
    {
        await ClickOnRowButton(RowNamed(name), "Edit");
        return await OpenedAttackEditor();
    }

    /// <summary>
    ///     Opens the editor for a duplicate of the existing attack whose row matches <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The attack name shown in the row.</param>
    /// <returns>The attack editor page for the duplicate.</returns>
    public async Task<TestAttackEditorPage> DuplicateAttack(string name)
    {
        await ClickOnRowButton(RowNamed(name), "Duplicate");
        return await OpenedAttackEditor();
    }

    /// <summary>
    ///     Deletes the attack whose row matches <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The attack name shown in the row.</param>
    public async Task DeleteAttack(string name)
    {
        var before = await Rows.CountAsync();
        await ClickOnRowButton(RowNamed(name), "Delete");
        await Expect(Rows).ToHaveCountAsync(before - 1);
    }

    /// <summary>
    ///     Reads the names of every attack currently listed.
    /// </summary>
    /// <returns>The attack names in list order.</returns>
    public async Task<IReadOnlyList<string>> AttackNames()
    {
        var count = await Rows.CountAsync();
        var names = new List<string>();
        for (var index = 0; index < count; index++)
        {
            var text = await Rows.Nth(index).InnerTextAsync();
            names.Add(text.Trim());
        }

        return names;
    }

    private async Task<TestAttackEditorPage> OpenedAttackEditor()
    {
        // The attack editor is reached by a same-document (SPA) navigation; wait for its unique "Main Informations" header
        // rather than a URL, which is unreliable for SPA navigations.
        await Expect(Page.GetByText("Main Informations")).ToBeVisibleAsync();
        return new TestAttackEditorPage(Page, _baseUrl);
    }
}