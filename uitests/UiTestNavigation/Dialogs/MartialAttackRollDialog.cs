using Microsoft.Playwright;

namespace DnDFightTool.UiTests.UiTestNavigation.Dialogs;

/// <summary>
///     Seam for the martial-attack roll modal that combat opens when a fighter attacks (the hit-roll and damage-roll
///     query dialogs). Returned by <see cref="Components.TestMartialAttackSelector.Attack"/> so the trigger is typed and
///     complete now; the methods that complete the roll and read the resulting log entry arrive in story 6.
/// </summary>
public sealed class MartialAttackRollDialog : DialogSeam
{
    /// <summary>
    ///     Initializes the seam with the browser page the roll modal renders on.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    public MartialAttackRollDialog(IPage page) : base(page)
    {
    }
}
