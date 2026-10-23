using DnDFightTool.UiTests.Framework.Components;
using Microsoft.Playwright;

namespace DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.Characters.Components;

/// <summary>
///     Typed object over the character "Basic infos" editor form: name, hit points, armor class and shield.
/// </summary>
public sealed class TestableCharacterMainInfoEditor : ComponentObject
{
    /// <summary>
    ///     Initializes the editor scoped to the form that hosts the character's main info fields.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="root">The locator scoping the main-info form.</param>
    public TestableCharacterMainInfoEditor(IPage page, ILocator root) : base(page, root)
    {
    }

    private ILocator NameField
    {
        get
        {
            return Root.GetByLabel("Name", new LocatorGetByLabelOptions { Exact = true });
        }
    }

    private ILocator MaxHpsField
    {
        get
        {
            return Root.GetByLabel("Max HPs", new LocatorGetByLabelOptions { Exact = true });
        }
    }

    private ILocator CurrentHpsField
    {
        get
        {
            return Root.GetByLabel("HPs", new LocatorGetByLabelOptions { Exact = true });
        }
    }

    private ILocator ArmorClassField
    {
        get
        {
            return Root.GetByLabel("Armor Class", new LocatorGetByLabelOptions { Exact = true });
        }
    }

    private ILocator ShieldValueField
    {
        get
        {
            return Root.GetByLabel("Shield value", new LocatorGetByLabelOptions { Exact = true });
        }
    }

    private ILocator EffectiveAcField
    {
        get
        {
            return Root.GetByLabel("Effective AC", new LocatorGetByLabelOptions { Exact = true });
        }
    }

    private ILocator ShieldEquippedCheckbox
    {
        get
        {
            return Root.GetByRole(AriaRole.Checkbox, new LocatorGetByRoleOptions { Name = "Shield equipped" });
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

    /// <summary>
    ///     Sets the character's maximum hit points.
    /// </summary>
    /// <param name="maxHps">The value to type into the Max HPs field.</param>
    public async Task SetMaxHps(int maxHps)
    {
        await MaxHpsField.FillAsync(maxHps.ToString());
    }

    /// <summary>
    ///     Reads the character's maximum hit points from the Max HPs field.
    /// </summary>
    /// <returns>The value currently in the Max HPs field.</returns>
    public async Task<int> GetMaxHps()
    {
        return int.Parse(await MaxHpsField.InputValueAsync());
    }

    /// <summary>
    ///     Sets the character's current hit points.
    /// </summary>
    /// <param name="currentHps">The value to type into the HPs field.</param>
    public async Task SetCurrentHps(int currentHps)
    {
        await CurrentHpsField.FillAsync(currentHps.ToString());
    }

    /// <summary>
    ///     Reads the character's current hit points from the HPs field.
    /// </summary>
    /// <returns>The value currently in the HPs field.</returns>
    public async Task<int> GetCurrentHps()
    {
        return int.Parse(await CurrentHpsField.InputValueAsync());
    }

    /// <summary>
    ///     Sets the character's base armor class.
    /// </summary>
    /// <param name="armorClass">The value to type into the Armor Class field.</param>
    public async Task SetArmorClass(int armorClass)
    {
        await ArmorClassField.FillAsync(armorClass.ToString());
    }

    /// <summary>
    ///     Reads the character's base armor class from the Armor Class field.
    /// </summary>
    /// <returns>The value currently in the Armor Class field.</returns>
    public async Task<int> GetArmorClass()
    {
        return int.Parse(await ArmorClassField.InputValueAsync());
    }

    /// <summary>
    ///     Sets the armor class granted by the character's shield.
    /// </summary>
    /// <param name="shieldValue">The value to type into the Shield value field.</param>
    public async Task SetShieldValue(int shieldValue)
    {
        await ShieldValueField.FillAsync(shieldValue.ToString());
    }

    /// <summary>
    ///     Reads the armor class granted by the character's shield from the Shield value field.
    /// </summary>
    /// <returns>The value currently in the Shield value field.</returns>
    public async Task<int> GetShieldValue()
    {
        return int.Parse(await ShieldValueField.InputValueAsync());
    }

    /// <summary>
    ///     Sets whether the character's shield is equipped.
    /// </summary>
    /// <param name="equipped"><see langword="true"/> to equip the shield; <see langword="false"/> to unequip it.</param>
    public async Task SetShieldEquipped(bool equipped)
    {
        await ShieldEquippedCheckbox.SetCheckedAsync(equipped);
    }

    /// <summary>
    ///     Reads whether the character's shield is currently equipped.
    /// </summary>
    /// <returns><see langword="true"/> if the shield checkbox is checked.</returns>
    public async Task<bool> IsShieldEquipped()
    {
        return await ShieldEquippedCheckbox.IsCheckedAsync();
    }

    /// <summary>
    ///     Reads the character's effective armor class (base plus shield, when equipped) from the read-only Effective AC
    ///     field.
    /// </summary>
    /// <returns>The value currently shown in the Effective AC field.</returns>
    public async Task<int> GetEffectiveAc()
    {
        return int.Parse(await EffectiveAcField.InputValueAsync());
    }
}
