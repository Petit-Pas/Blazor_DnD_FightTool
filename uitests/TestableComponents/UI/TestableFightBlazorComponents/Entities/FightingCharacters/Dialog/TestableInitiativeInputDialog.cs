using DnDFightTool.UiTests.Framework.Dialogs;
using Microsoft.Playwright;

namespace DnDFightTool.UiTests.TestableComponents.UI.TestableFightBlazorComponents.Entities.FightingCharacters.Dialog;

/// <summary>
///     Seam for the initiative-roll modal the dashboard opens when a fighter joins the fight without an initiative. Story 5
///     side-steps it by seeding a non-zero initiative, so no method here drives it; the surface exists so story 6 can add
///     the interaction that pins the rolled initiative through the UI.
/// </summary>
public sealed class TestableInitiativeInputDialog : DialogSeam
{
    /// <summary>
    ///     Initializes the seam with the browser page the initiative modal renders on.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    public TestableInitiativeInputDialog(IPage page) : base(page)
    {
    }
}
