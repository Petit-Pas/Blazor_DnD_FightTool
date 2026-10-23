using DnDFightTool.UiTests.Framework.Components;
using Microsoft.Playwright;

namespace DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.AbilityScores;

/// <summary>
///     Typed object over the ability-scores editor hosted on the character editor's "Abilities &amp; Skills" tab: the
///     per-ability rows and the shared mastery bonus.
/// </summary>
public sealed class TestableAbilityScoresEditorComponent : ComponentObject
{
    /// <summary>
    ///     Initializes the editor scoped to the panel that hosts the ability rows and the mastery bonus.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="root">The locator scoping the ability-scores panel.</param>
    public TestableAbilityScoresEditorComponent(IPage page, ILocator root) : base(page, root)
    {
    }

    private ILocator MasteryBonusField
    {
        get
        {
            return Root.GetByLabel("Mastery bonus", new LocatorGetByLabelOptions { Exact = true });
        }
    }

    /// <summary>
    ///     Gets the typed object over the ability row matching <paramref name="shortName"/>.
    /// </summary>
    /// <param name="shortName">The ability's short name, e.g. <c>STR</c> or <c>DEX</c>.</param>
    /// <returns>The ability row editor.</returns>
    public TestableAbilityScoreEditorComponent Ability(string shortName)
    {
        var row = Root.GetByRole(AriaRole.Group, new LocatorGetByRoleOptions { Name = $"{shortName} ability", Exact = true });
        return new TestableAbilityScoreEditorComponent(Page, row);
    }

    /// <summary>
    ///     Sets the shared mastery bonus applied to every ability with save mastery.
    /// </summary>
    /// <param name="masteryBonus">The value to type into the Mastery bonus field.</param>
    public async Task SetMasteryBonus(int masteryBonus)
    {
        await MasteryBonusField.FillAsync(masteryBonus.ToString());
    }

    /// <summary>
    ///     Reads the shared mastery bonus applied to every ability with save mastery.
    /// </summary>
    /// <returns>The value currently in the Mastery bonus field.</returns>
    public async Task<int> GetMasteryBonus()
    {
        return int.Parse(await MasteryBonusField.InputValueAsync());
    }
}
