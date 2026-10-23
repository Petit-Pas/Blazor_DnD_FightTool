using System.Text.RegularExpressions;
using DnDFightTool.UiTests.Framework.Components;
using Microsoft.Playwright;

namespace DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.DamageAffinities.Components;

/// <summary>
///     Typed object over the resistances editor hosted on the character editor's "Resistances" tab: affinity
///     increase/decrease per damage type.
/// </summary>
public sealed class TestableResistancesEditorComponent : ComponentObject
{
    /// <summary>
    ///     Initializes the editor scoped to the resistances panel.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="root">The locator scoping the resistances panel.</param>
    public TestableResistancesEditorComponent(IPage page, ILocator root) : base(page, root)
    {
    }

    /// <summary>
    ///     Increases the given damage type's affinity by one step (left-click): Weak → Normal → Resistant → Immune → Heal,
    ///     capped at Heal.
    /// </summary>
    /// <param name="damageType">The damage type's readable name, e.g. <c>Fire</c>.</param>
    public async Task IncreaseAffinity(string damageType)
    {
        await AffinityButton(damageType).ClickAsync();
    }

    /// <summary>
    ///     Decreases the given damage type's affinity by one step (right-click), floored at Weak.
    /// </summary>
    /// <param name="damageType">The damage type's readable name, e.g. <c>Fire</c>.</param>
    public async Task DecreaseAffinity(string damageType)
    {
        await AffinityButton(damageType).ClickAsync(new LocatorClickOptions { Button = MouseButton.Right });
    }

    /// <summary>
    ///     Reads the given damage type's current affinity from the affinity button's accessible name.
    /// </summary>
    /// <param name="damageType">The damage type's readable name, e.g. <c>Fire</c>.</param>
    /// <returns>The damage type's affinity, e.g. <c>Normal</c>, <c>Resistant</c> or <c>Immune</c>.</returns>
    public async Task<string> GetAffinity(string damageType)
    {
        return await GetAccessibleNameState(AffinityButton(damageType));
    }

    private ILocator AffinityButton(string damageType)
    {
        var escaped = Regex.Escape(damageType);
        return Root.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { NameRegex = new Regex($"^{escaped} affinity: ") });
    }
}
