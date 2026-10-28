using DnDFightTool.UiTests.Framework.Components;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.MartialAttacks.Components;

/// <summary>
///     Typed object over the martial-attack "Main Informations" panel: the attack's name and to-hit modifiers.
/// </summary>
public sealed class TestableAttackMainInfoEditor : ComponentObject
{
    // The error is a local re-render of the field, so it needs far less than Playwright's 5 s default.
    private const float ErrorTimeoutMs = 1000;

    /// <summary>
    ///     Initializes the editor scoped to the "Main Informations" panel.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="root">The locator scoping the main-info panel.</param>
    public TestableAttackMainInfoEditor(IPage page, ILocator root) : base(page, root)
    {
    }

    private ILocator NameField
    {
        get
        {
            return Root.GetByLabel("Name", new LocatorGetByLabelOptions { Exact = true });
        }
    }

    private ILocator ToHitModifiersField
    {
        get
        {
            return Root.GetByRole(AriaRole.Textbox, new LocatorGetByRoleOptions { Name = "To Hit Modifiers", Exact = true });
        }
    }

    /// <summary>
    ///     Sets the attack's name.
    /// </summary>
    /// <param name="name">The name to type into the Name field.</param>
    public async Task SetName(string name)
    {
        await NameField.FillAsync(name);
    }

    /// <summary>
    ///     Reads the attack's current name from the Name field.
    /// </summary>
    /// <returns>The value currently in the Name field.</returns>
    public async Task<string> GetName()
    {
        return await NameField.InputValueAsync();
    }

    /// <summary>
    ///     Sets the attack's to-hit modifiers expression.
    /// </summary>
    /// <param name="expression">The expression to type, e.g. <c>STR+2</c>.</param>
    public async Task SetToHitModifiers(string expression)
    {
        await ToHitModifiersField.FillAsync(expression);
    }

    /// <summary>
    ///     Reads the to-hit modifiers expression currently in the field.
    /// </summary>
    /// <returns>The value currently in the To Hit Modifiers field.</returns>
    public async Task<string> GetToHitModifiers()
    {
        return await ToHitModifiersField.InputValueAsync();
    }

    /// <summary>
    ///     Waits until the To Hit Modifiers field is invalid and its accessible description is <paramref name="message"/>.
    /// </summary>
    /// <param name="message">The expected error message.</param>
    public async Task WaitForToHitModifiersError(string message)
    {
        await Expect(ToHitModifiersField).ToHaveAttributeAsync("aria-invalid", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = ErrorTimeoutMs });
        await Expect(ToHitModifiersField).ToHaveAccessibleDescriptionAsync(message, new LocatorAssertionsToHaveAccessibleDescriptionOptions { Timeout = ErrorTimeoutMs });
    }
}
