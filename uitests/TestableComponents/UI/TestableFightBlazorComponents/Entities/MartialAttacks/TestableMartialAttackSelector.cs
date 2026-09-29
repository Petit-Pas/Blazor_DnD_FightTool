using DnDFightTool.UiTests.Framework.Components;
using DnDFightTool.UiTests.TestableComponents.UI.TestableDnDQueryPrompter.MartialAttackQueries;
using Microsoft.Playwright;

namespace DnDFightTool.UiTests.TestableComponents.UI.TestableFightBlazorComponents.Entities.MartialAttacks;

/// <summary>
///     Typed object over the dashboard's martial-attack selector: pick one of the selected fighter's attacks, then trigger
///     it. Triggering opens the roll modals, so <see cref="Attack"/> returns the modal seam — story 6 gives that seam its
///     interaction surface and the scenarios that drive it. No story-5 scenario invokes <see cref="Attack"/>.
/// </summary>
public sealed class TestableMartialAttackSelector : ComponentObject
{
    /// <summary>
    ///     Initializes the selector scoped to the attacks tab's element.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="root">The locator scoping the attack selector.</param>
    public TestableMartialAttackSelector(IPage page, ILocator root) : base(page, root)
    {
    }

    /// <summary>
    ///     Selects the attack whose table row matches <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The attack name shown in the selector.</param>
    /// <returns>This selector, so the trigger can be chained.</returns>
    public async Task<TestableMartialAttackSelector> SelectAttack(string name)
    {
        await Root.Locator("tr").Filter(new LocatorFilterOptions { HasText = name }).ClickAsync();
        return this;
    }

    /// <summary>
    ///     Triggers the selected attack, opening the martial-attack roll modal.
    /// </summary>
    /// <returns>The roll-modal seam (its interaction surface arrives in story 6).</returns>
    public async Task<TestableMartialAttackRollResultQueryHandlerModal> Attack()
    {
        await Root.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Attack" }).ClickAsync();
        return new TestableMartialAttackRollResultQueryHandlerModal(Page);
    }
}
