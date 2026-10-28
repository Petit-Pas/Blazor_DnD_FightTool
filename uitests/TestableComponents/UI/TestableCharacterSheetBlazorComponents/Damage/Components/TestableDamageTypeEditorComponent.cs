using System.Text.RegularExpressions;
using DnDFightTool.UiTests.Framework.Components;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.Damage.Components;

/// <summary>
///     Typed object over a damage-type picker: an icon button named <c>"Damage type: {type}"</c> that opens a list of types.
/// </summary>
public sealed class TestableDamageTypeEditorComponent : ComponentObject
{
    private static readonly Regex _activatorName = new("^Damage type: ");

    /// <summary>
    ///     Initializes the editor scoped to the element that hosts one damage-type picker.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="root">The locator scoping the picker, e.g. its damage row.</param>
    public TestableDamageTypeEditorComponent(IPage page, ILocator root) : base(page, root)
    {
    }

    private ILocator Activator
    {
        get
        {
            return Root.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { NameRegex = _activatorName });
        }
    }

    /// <summary>
    ///     Picks a damage type from the picker's list and waits until the picker shows it.
    /// </summary>
    /// <param name="damageType">The type's readable name, e.g. <c>Cold</c>.</param>
    public async Task SetDamageType(string damageType)
    {
        await Activator.ClickAsync();
        // The list is rendered in a popover outside the row, so its options are resolved against the page.
        var option = Page.GetByRole(AriaRole.Option, new PageGetByRoleOptions { Name = damageType, Exact = true });
        await option.ClickAsync();
        await Expect(option).ToBeHiddenAsync();
        await WaitForDamageType(damageType);
    }

    /// <summary>
    ///     Reads the damage type from the picker's accessible name.
    /// </summary>
    /// <returns>The type's readable name, e.g. <c>Fire</c>.</returns>
    public async Task<string> GetDamageType()
    {
        return await GetAccessibleNameState(Activator);
    }

    /// <summary>
    ///     Waits until the picker shows <paramref name="damageType"/>; the button is re-rendered after the menu closes.
    /// </summary>
    /// <param name="damageType">The type's readable name, e.g. <c>Cold</c>.</param>
    public async Task WaitForDamageType(string damageType)
    {
        await Expect(Activator).ToHaveAccessibleNameAsync($"Damage type: {damageType}");
    }
}
