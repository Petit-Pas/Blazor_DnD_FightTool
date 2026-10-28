using DnDFightTool.UiTests.Framework.Components;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.Damage.Components;

/// <summary>
///     Typed object over an attack's "Damages" panel: add a damage roll, count them, and reach one row by its 1-based number.
/// </summary>
public sealed class TestableDamageRollTemplateCollectionEditorComponent : ComponentObject
{
    /// <summary>
    ///     Initializes the editor scoped to the "Damages" panel.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="root">The locator scoping the damages panel.</param>
    public TestableDamageRollTemplateCollectionEditorComponent(IPage page, ILocator root) : base(page, root)
    {
    }

    private ILocator Rows
    {
        get
        {
            return Root.GetByRole(AriaRole.Group, new LocatorGetByRoleOptions { NameRegex = new("^Damage \\d+$") });
        }
    }

    /// <summary>
    ///     Adds a new damage roll and waits for its row to appear.
    /// </summary>
    public async Task AddDamage()
    {
        var countBefore = await Rows.CountAsync();
        await Root.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Add damage", Exact = true }).ClickAsync();
        await Expect(Rows).ToHaveCountAsync(countBefore + 1);
    }

    /// <summary>
    ///     Gets the typed object over one damage row.
    /// </summary>
    /// <param name="number">The row's 1-based position, as in its "Damage {n}" name.</param>
    /// <returns>The damage-roll row editor.</returns>
    public TestableDamageRollTemplateEditorComponent Damage(int number)
    {
        var row = Root.GetByRole(AriaRole.Group, new LocatorGetByRoleOptions { Name = $"Damage {number}", Exact = true });
        return new TestableDamageRollTemplateEditorComponent(Page, row);
    }

    /// <summary>
    ///     Counts the damage rows currently shown.
    /// </summary>
    /// <returns>The number of damage rows.</returns>
    public async Task<int> GetDamageCount()
    {
        return await Rows.CountAsync();
    }

    /// <summary>
    ///     Waits until the panel shows exactly <paramref name="count"/> damage rows.
    /// </summary>
    /// <param name="count">The expected number of rows.</param>
    public async Task WaitForDamageCount(int count)
    {
        await Expect(Rows).ToHaveCountAsync(count);
    }
}
