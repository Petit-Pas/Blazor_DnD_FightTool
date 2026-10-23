using System.Text.RegularExpressions;
using DnDFightTool.UiTests.Framework.Components;
using Microsoft.Playwright;

namespace DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.Skills.Components;

/// <summary>
///     Typed object over the skills editor hosted on the character editor's "Abilities &amp; Skills" tab: mastery
///     increase/decrease per skill and the skill's governing-ability picker.
/// </summary>
public sealed class TestableSkillsEditorComponent : ComponentObject
{
    /// <summary>
    ///     Initializes the editor scoped to the skills panel.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="root">The locator scoping the skills panel.</param>
    public TestableSkillsEditorComponent(IPage page, ILocator root) : base(page, root)
    {
    }

    /// <summary>
    ///     Increases the given skill's mastery by one step (left-click), capped at Expertise.
    /// </summary>
    /// <param name="skill">The skill's readable name, e.g. <c>Acrobatics</c>.</param>
    public async Task IncreaseMastery(string skill)
    {
        await MasteryButton(skill).ClickAsync();
    }

    /// <summary>
    ///     Decreases the given skill's mastery by one step (right-click), floored at Normal.
    /// </summary>
    /// <param name="skill">The skill's readable name, e.g. <c>Acrobatics</c>.</param>
    public async Task DecreaseMastery(string skill)
    {
        await MasteryButton(skill).ClickAsync(new LocatorClickOptions { Button = MouseButton.Right });
    }

    /// <summary>
    ///     Reads the given skill's current mastery from the mastery button's accessible name.
    /// </summary>
    /// <param name="skill">The skill's readable name, e.g. <c>Acrobatics</c>.</param>
    /// <returns>The skill's mastery, e.g. <c>Normal</c>, <c>Mastery</c> or <c>Expertise</c>.</returns>
    public async Task<string> GetMastery(string skill)
    {
        return await GetAccessibleNameState(MasteryButton(skill));
    }

    /// <summary>
    ///     Sets the ability used by the given skill's check, via its governing-ability picker.
    /// </summary>
    /// <param name="skill">The skill's readable name, e.g. <c>Acrobatics</c>.</param>
    /// <param name="abilityShortName">The ability's short name, e.g. <c>STR</c> or <c>DEX</c>.</param>
    public async Task SetAbility(string skill, string abilityShortName)
    {
        await AbilityChip(skill).ClickAsync();
        // The menu's items portal outside the table into a popover appended at document level, so they are resolved
        // against the page rather than the row's locator. MudBlazor renders each item as a plain, role-less
        // ".mud-menu-item" div (not an ARIA "menuitem"), so it must be matched by class and text rather than by role.
        // The default ability's item appends " (default)" to its text.
        var escaped = Regex.Escape(abilityShortName);
        await Page.Locator(".mud-popover-open .mud-menu-item")
            .Filter(new LocatorFilterOptions { HasTextRegex = new Regex($"^{escaped}( \\(default\\))?$") })
            .ClickAsync();
    }

    /// <summary>
    ///     Reads the ability currently used by the given skill's check.
    /// </summary>
    /// <param name="skill">The skill's readable name, e.g. <c>Acrobatics</c>.</param>
    /// <returns>The ability's short name currently shown on the skill's chip.</returns>
    public async Task<string> GetAbility(string skill)
    {
        return (await AbilityChip(skill).InnerTextAsync()).Trim();
    }

    private ILocator MasteryButton(string skill)
    {
        var escaped = Regex.Escape(skill);
        return Root.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { NameRegex = new Regex($"^{escaped} mastery: ") });
    }

    private ILocator SkillRow(string skill)
    {
        return MasteryButton(skill).Locator("xpath=ancestor::tr");
    }

    private ILocator AbilityChip(string skill)
    {
        return SkillRow(skill).Locator(".ability-score-chip");
    }
}
