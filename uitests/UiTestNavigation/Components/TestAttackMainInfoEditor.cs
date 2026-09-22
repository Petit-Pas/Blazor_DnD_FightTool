using Microsoft.Playwright;

namespace DnDFightTool.UiTests.UiTestNavigation.Components;

/// <summary>
///     Typed object over the martial-attack "Main Informations" editor form. This story wires only name-level editing —
///     the to-hit modifiers and damage rolls are the deferred field-coverage story, which extends this same object.
/// </summary>
public sealed class TestAttackMainInfoEditor : ComponentObject
{
    /// <summary>
    ///     Initializes the editor scoped to the form that hosts the attack's name field.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="root">The locator scoping the main-info form.</param>
    public TestAttackMainInfoEditor(IPage page, ILocator root) : base(page, root)
    {
    }

    private ILocator NameField
    {
        get
        {
            return Root.GetByLabel("Name", new LocatorGetByLabelOptions { Exact = true });
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
}
