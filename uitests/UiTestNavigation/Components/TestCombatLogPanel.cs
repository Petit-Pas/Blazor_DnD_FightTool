using Microsoft.Playwright;

namespace DnDFightTool.UiTests.UiTestNavigation.Components;

/// <summary>
///     Typed object over the dashboard's combat-log panel. Reads the visible log-entry text so a scenario can assert on
///     what combat wrote.
/// </summary>
public sealed class TestCombatLogPanel : ComponentObject
{
    /// <summary>
    ///     Initializes the panel scoped to the log element.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="root">The locator scoping the log panel.</param>
    public TestCombatLogPanel(IPage page, ILocator root) : base(page, root)
    {
    }

    /// <summary>
    ///     Reads the visible log entries in order.
    /// </summary>
    /// <returns>The text of each visible log entry.</returns>
    public async Task<IReadOnlyList<string>> Entries()
    {
        var texts = await Root.Locator(".log-entry").AllInnerTextsAsync();
        return [.. texts.Select(text => text.Trim()).Where(text => text.Length > 0)];
    }
}
