using DnDFightTool.UiTests.Framework.Components;
using Microsoft.Playwright;

namespace DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.AbilityScores;

/// <summary>
///     Typed object over a single ability row inside the ability-scores editor: the mastery chip, the ability score and
///     the save bonus.
/// </summary>
public sealed class TestableAbilityScoreEditorComponent : ComponentObject
{
    /// <summary>
    ///     Initializes the editor scoped to one ability row.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="root">The locator scoping the ability row.</param>
    public TestableAbilityScoreEditorComponent(IPage page, ILocator root) : base(page, root)
    {
    }

    private ILocator MasteryChip
    {
        get
        {
            return Root.Locator(".ability-score-chip");
        }
    }

    private ILocator ScoreField
    {
        get
        {
            return Root.GetByLabel("Ability score", new LocatorGetByLabelOptions { Exact = true });
        }
    }

    private ILocator SaveBonusField
    {
        get
        {
            return Root.GetByLabel("Save bonus", new LocatorGetByLabelOptions { Exact = true });
        }
    }

    /// <summary>
    ///     Toggles the ability's save-mastery state by clicking its chip.
    /// </summary>
    public async Task ToggleSaveMastery()
    {
        await MasteryChip.ClickAsync();
    }

    /// <summary>
    ///     Reads whether the ability currently has save mastery, from the chip's filled/outlined variant.
    /// </summary>
    /// <returns><see langword="true"/> if the chip is shown filled (save mastery granted).</returns>
    public async Task<bool> HasSaveMastery()
    {
        var classes = await MasteryChip.GetAttributeAsync("class") ?? string.Empty;
        return classes.Contains("mud-chip-filled");
    }

    /// <summary>
    ///     Sets the ability's raw score.
    /// </summary>
    /// <param name="score">The value to type into the Ability score field.</param>
    public async Task SetScore(int score)
    {
        await ScoreField.FillAsync(score.ToString());
    }

    /// <summary>
    ///     Reads the ability's raw score.
    /// </summary>
    /// <returns>The value currently in the Ability score field.</returns>
    public async Task<int> GetScore()
    {
        return int.Parse(await ScoreField.InputValueAsync());
    }

    /// <summary>
    ///     Sets the ability's arbitrary save-check modifier.
    /// </summary>
    /// <param name="saveBonus">The value to type into the Save bonus field.</param>
    public async Task SetSaveBonus(int saveBonus)
    {
        await SaveBonusField.FillAsync(saveBonus.ToString());
    }

    /// <summary>
    ///     Reads the ability's arbitrary save-check modifier.
    /// </summary>
    /// <returns>The value currently in the Save bonus field.</returns>
    public async Task<int> GetSaveBonus()
    {
        return int.Parse(await SaveBonusField.InputValueAsync());
    }
}
