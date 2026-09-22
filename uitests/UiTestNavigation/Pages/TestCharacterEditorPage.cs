using DnDFightTool.UiTests.UiTestNavigation.Components;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace DnDFightTool.UiTests.UiTestNavigation.Pages;

/// <summary>
///     Page object for the character editor (<c>/Characters/Edit</c>). It is not URL-navigable — the editor renders only
///     when a per-circuit character edit context is set — so this page is returned from the character list page's
///     create/edit/duplicate methods, which click the production control that drives the real navigation. This story wires
///     the "Basic infos" (name) and "Attacks" tabs; the abilities/skills/resistances editing is the deferred story.
/// </summary>
public sealed class TestCharacterEditorPage : PageObject
{
    /// <summary>
    ///     Initializes the character editor page object over an already-open editor.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="baseUrl">The host base URL.</param>
    public TestCharacterEditorPage(IPage page, string baseUrl) : base(page, baseUrl)
    {
    }

    /// <summary>
    ///     Gets the typed object over the character's main-info form (name-level editing). Assumes the "Basic infos" tab is
    ///     the active tab, which it is when the editor first opens.
    /// </summary>
    /// <returns>The character main-info editor.</returns>
    public TestCharacterMainInfoEditor MainInfo()
    {
        return new TestCharacterMainInfoEditor(Page, Page.Locator("form:visible").First);
    }

    /// <summary>
    ///     Switches to the "Attacks" tab and returns the typed object over its attack-list editor.
    /// </summary>
    /// <returns>The attack-list editor.</returns>
    public async Task<TestAttackListEditor> OpenAttacks()
    {
        await Page.GetByRole(AriaRole.Tab, new PageGetByRoleOptions { Name = "Attacks", Exact = true }).ClickAsync();
        var card = Page.Locator(".mud-card:has(.attack-list):visible");
        await Expect(card.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Add" })).ToBeVisibleAsync();
        return new TestAttackListEditor(Page, card, BaseUrl);
    }

    /// <summary>
    ///     Saves the character, returning to the list page it was opened from.
    /// </summary>
    public async Task Save()
    {
        await Page.Locator("button[aria-label='Save']:visible").ClickAsync();
        await WaitForList();
    }

    /// <summary>
    ///     Cancels the character edit, returning to the list page it was opened from.
    /// </summary>
    public async Task Cancel()
    {
        await Page.Locator("button[aria-label='Cancel']:visible").ClickAsync();
        await WaitForList();
    }

    // Save/Cancel return to the list via a same-document navigation; wait for the list's Players tab rather than a URL.
    private async Task WaitForList()
    {
        await Expect(Page.GetByRole(AriaRole.Tab, new PageGetByRoleOptions { Name = "Players", Exact = true }))
            .ToBeVisibleAsync();
    }
}
