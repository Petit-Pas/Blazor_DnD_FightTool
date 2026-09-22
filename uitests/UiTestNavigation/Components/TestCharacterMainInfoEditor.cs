using Microsoft.Playwright;

namespace DnDFightTool.UiTests.UiTestNavigation.Components;

/// <summary>
///     Typed object over the character "Basic infos" editor form. This story wires only name-level editing — the depth
///     fields (HP, AC, shield) are the deferred field-coverage story, which extends this same component object.
/// </summary>
public sealed class TestCharacterMainInfoEditor : ComponentObject
{
    /// <summary>
    ///     Initializes the editor scoped to the form that hosts the character's name field.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="root">The locator scoping the main-info form.</param>
    public TestCharacterMainInfoEditor(IPage page, ILocator root) : base(page, root)
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
    ///     Sets the character's name.
    /// </summary>
    /// <param name="name">The name to type into the Name field.</param>
    public async Task SetName(string name)
    {
        await NameField.FillAsync(name);
    }

    /// <summary>
    ///     Reads the character's current name from the Name field.
    /// </summary>
    /// <returns>The value currently in the Name field.</returns>
    public async Task<string> GetName()
    {
        return await NameField.InputValueAsync();
    }
}
