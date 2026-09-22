using DnDFightTool.UiTests.UiTestNavigation.Components;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace DnDFightTool.UiTests.UiTestNavigation.Pages;

/// <summary>
///     Page object for the attack editor (<c>/Attacks/Edit</c>). It is not URL-navigable — the editor renders only when a
///     per-circuit attack edit context is set — so this page is returned from <see cref="Components.TestAttackListEditor"/>'s
///     add/edit/duplicate methods, which click the production control that drives the real navigation. This story wires
///     the name-level editing needed to save an attack; the to-hit and damage editing is the deferred field-coverage story.
/// </summary>
public sealed class TestAttackEditorPage : PageObject
{
    /// <summary>
    ///     Initializes the attack editor page object over an already-open editor.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="baseUrl">The host base URL.</param>
    public TestAttackEditorPage(IPage page, string baseUrl) : base(page, baseUrl)
    {
    }

    /// <summary>
    ///     Gets the typed object over the attack's main-info form (name-level editing).
    /// </summary>
    /// <returns>The attack main-info editor.</returns>
    public TestAttackMainInfoEditor MainInfo()
    {
        return new TestAttackMainInfoEditor(Page, Page.Locator("form:visible").First);
    }

    /// <summary>
    ///     Saves the attack, returning to the character editor it was opened from.
    /// </summary>
    public async Task Save()
    {
        await Page.Locator("button[aria-label='Save']:visible").ClickAsync();
        await WaitForCharacterEditor();
    }

    /// <summary>
    ///     Cancels the attack edit, returning to the character editor it was opened from.
    /// </summary>
    public async Task Cancel()
    {
        await Page.Locator("button[aria-label='Cancel']:visible").ClickAsync();
        await WaitForCharacterEditor();
    }

    // Save/Cancel return to the character editor via a same-document navigation; wait for a marker unique to it (the
    // "Abilities & Skills" tab) rather than a URL.
    private async Task WaitForCharacterEditor()
    {
        await Expect(Page.GetByRole(AriaRole.Tab, new PageGetByRoleOptions { Name = "Abilities & Skills", Exact = true }))
            .ToBeVisibleAsync();
    }
}
