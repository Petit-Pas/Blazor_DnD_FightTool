using DnDFightTool.UiTests.Framework.Components;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.Damage.Components;

/// <summary>
///     Typed object over one damage-roll row: its damage type, its dice expression and its delete control.
/// </summary>
public sealed class TestableDamageRollTemplateEditorComponent : ComponentObject
{
    // The error is a local re-render of the field, so it needs far less than Playwright's 5 s default.
    private const float ErrorTimeoutMs = 1000;

    /// <summary>
    ///     Initializes the editor scoped to one "Damage {n}" row.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="root">The locator scoping the damage row.</param>
    public TestableDamageRollTemplateEditorComponent(IPage page, ILocator root) : base(page, root)
    {
    }

    private ILocator DiceField
    {
        get
        {
            return Root.GetByRole(AriaRole.Textbox, new LocatorGetByRoleOptions { Name = "Dice", Exact = true });
        }
    }

    /// <summary>
    ///     Sets the row's dice expression.
    /// </summary>
    /// <param name="expression">The expression to type, e.g. <c>2d8+STR</c>.</param>
    public async Task SetDice(string expression)
    {
        await DiceField.FillAsync(expression);
    }

    /// <summary>
    ///     Reads the row's dice expression currently in the field.
    /// </summary>
    /// <returns>The value currently in the Dice field.</returns>
    public async Task<string> GetDice()
    {
        return await DiceField.InputValueAsync();
    }

    /// <summary>
    ///     Waits until the Dice field is invalid and its accessible description is <paramref name="message"/>.
    /// </summary>
    /// <param name="message">The expected error message.</param>
    public async Task WaitForDiceError(string message)
    {
        await Expect(DiceField).ToHaveAttributeAsync("aria-invalid", "true", new LocatorAssertionsToHaveAttributeOptions { Timeout = ErrorTimeoutMs });
        await Expect(DiceField).ToHaveAccessibleDescriptionAsync(message, new LocatorAssertionsToHaveAccessibleDescriptionOptions { Timeout = ErrorTimeoutMs });
    }

    /// <summary>
    ///     Waits until the Dice field shows <paramref name="expression"/>.
    /// </summary>
    /// <param name="expression">The expected text, e.g. <c>2d8</c>.</param>
    public async Task WaitForDice(string expression)
    {
        await Expect(DiceField).ToHaveValueAsync(expression);
    }

    /// <summary>
    ///     Deletes this damage row.
    /// </summary>
    public async Task Delete()
    {
        await Root.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Delete damage", Exact = true }).ClickAsync();
    }

    /// <summary>
    ///     Gets the typed object over this row's damage-type picker.
    /// </summary>
    /// <returns>The damage-type editor.</returns>
    public TestableDamageTypeEditorComponent DamageType()
    {
        return new TestableDamageTypeEditorComponent(Page, Root);
    }
}
