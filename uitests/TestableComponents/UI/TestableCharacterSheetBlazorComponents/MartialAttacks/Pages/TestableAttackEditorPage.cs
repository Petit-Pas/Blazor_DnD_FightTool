using DnDFightTool.UiTests.Framework.Pages;
using DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.Characters.Pages;
using DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.Damage.Components;
using DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.MartialAttacks.Components;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.MartialAttacks.Pages;

/// <summary>
///     Page object for the attack editor (<c>/Attacks/Edit</c>). It is not URL-navigable — the editor renders only when a
///     per-circuit attack edit context is set — so this page is returned from <see cref="TestableAttackListEditor"/>'s
///     add/edit/duplicate methods, which click the production control that drives the real navigation.
/// </summary>
public sealed class TestableAttackEditorPage : PageObject
{
    /// <summary>
    ///     Initializes the attack editor page object over an already-open editor.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="baseUrl">The host base URL.</param>
    public TestableAttackEditorPage(IPage page, string baseUrl) : base(page, baseUrl)
    {
    }

    /// <summary>
    ///     Gets the typed object over the attack's "Main Informations" panel (name and to-hit modifiers).
    /// </summary>
    /// <returns>The attack main-info editor.</returns>
    public TestableAttackMainInfoEditor MainInfo()
    {
        return new TestableAttackMainInfoEditor(Page, Region("Main Informations"));
    }

    /// <summary>
    ///     Gets the typed object over the attack's "Damages" panel.
    /// </summary>
    /// <returns>The damage-roll collection editor.</returns>
    public TestableDamageRollTemplateCollectionEditorComponent Damages()
    {
        return new TestableDamageRollTemplateCollectionEditorComponent(Page, Region("Damages"));
    }

    /// <summary>
    ///     Saves the attack, returning to the character editor it was opened from.
    /// </summary>
    /// <returns>The character editor page.</returns>
    public async Task<TestableCharacterEditorPage> Save()
    {
        await AttemptSave();
        return await WaitForCharacterEditor();
    }

    /// <summary>
    ///     Clicks Save without waiting for navigation, for cases where the editor refuses to save invalid input.
    /// </summary>
    public async Task AttemptSave()
    {
        await Button("Save").ClickAsync();
    }

    /// <summary>
    ///     Checks that the attack editor is still shown, for example after a refused Save.
    /// </summary>
    public async Task ExpectStillOpen()
    {
        await Expect(Region("Attack editor")).ToBeVisibleAsync();
    }

    /// <summary>
    ///     Cancels the attack edit, returning to the character editor it was opened from.
    /// </summary>
    /// <returns>The character editor page.</returns>
    public async Task<TestableCharacterEditorPage> Cancel()
    {
        await Button("Cancel").ClickAsync();
        return await WaitForCharacterEditor();
    }

    private ILocator Region(string name)
    {
        return Page.GetByRole(AriaRole.Region, new PageGetByRoleOptions { Name = name, Exact = true });
    }

    // Scoped to the attack editor so it never matches the character editor's own Save/Cancel.
    private ILocator Button(string name)
    {
        return Region("Attack editor").GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = name, Exact = true });
    }

    // Save/Cancel return to the character editor via a same-document navigation; wait for a marker unique to it (the
    // "Abilities & Skills" tab) rather than a URL.
    private async Task<TestableCharacterEditorPage> WaitForCharacterEditor()
    {
        await Expect(Page.GetByRole(AriaRole.Tab, new PageGetByRoleOptions { Name = "Abilities & Skills", Exact = true }))
            .ToBeVisibleAsync();
        return new TestableCharacterEditorPage(Page, BaseUrl);
    }
}
