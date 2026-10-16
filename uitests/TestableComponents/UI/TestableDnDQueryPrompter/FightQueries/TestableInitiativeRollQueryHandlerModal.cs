using DnDFightTool.UiTests.Framework.Dialogs;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace DnDFightTool.UiTests.TestableComponents.UI.TestableDnDQueryPrompter.FightQueries;

/// <summary>
///     Drives the initiative prompt shown when adding a player or the first monster of a kind.
/// </summary>
public sealed class TestableInitiativeRollQueryHandlerModal : DialogSeam
{
    /// <summary>
    ///     Initializes the initiative dialog with the scenario's browser page.
    /// </summary>
    /// <param name="page">The browser page containing the dialog.</param>
    public TestableInitiativeRollQueryHandlerModal(IPage page) : base(page)
    {
    }

    /// <summary>
    ///     Rolls initiative and confirms the result through the dialog buttons.
    /// </summary>
    public async Task RollAndConfirm()
    {
        var dialog = Page.Locator(".mud-dialog:has(.initiative-roll)");
        await dialog.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Roll", Exact = true })
            .ClickAsync();
        await dialog.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Confirm", Exact = true })
            .ClickAsync();
        await Expect(dialog).Not.ToBeVisibleAsync();
    }
}