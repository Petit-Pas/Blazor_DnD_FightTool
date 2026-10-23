using Microsoft.Playwright;

namespace DnDFightTool.UiTests.Framework.Components;

/// <summary>
///     Base type for a typed object over a reusable UI fragment (a tile, a panel, an editor sub-form). Unlike a
///     <see cref="Pages.PageObject"/>, a component object is scoped to a root <see cref="Root"/> locator so its actions
///     resolve inside that one element even when the same fragment renders more than once on a page (two Save/Cancel rows
///     across tabs, two search boxes, many fighter tiles). This scoping is the seam that lets the typed surface grow
///     without locator collisions.
/// </summary>
public abstract class ComponentObject
{
    /// <summary>
    ///     Initializes the component object with the browser page and the root locator its own locators resolve within.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="root">The locator scoping this fragment's element; all inner locators derive from it.</param>
    protected ComponentObject(IPage page, ILocator root)
    {
        Page = page ?? throw new ArgumentNullException(nameof(page));
        Root = root ?? throw new ArgumentNullException(nameof(root));
    }

    /// <summary>
    ///     Gets the browser page this object drives. Exposed to derived component objects only; scenarios never touch it.
    /// </summary>
    protected IPage Page { get; }

    /// <summary>
    ///     Gets the locator scoping this fragment, so a derived component object resolves its inner elements inside the one
    ///     instance it represents rather than across every copy on the page.
    /// </summary>
    protected ILocator Root { get; }

    /// <summary>
    ///     Reads a locator's <c>aria-label</c> and returns the state carried after its <c>": "</c> separator, as used by
    ///     accessible names of the shape <c>"{subject}: {state}"</c> (e.g. mastery and affinity buttons).
    /// </summary>
    /// <param name="locator">The locator whose <c>aria-label</c> carries the state.</param>
    /// <returns>The state portion of the accessible name.</returns>
    protected static async Task<string> GetAccessibleNameState(ILocator locator)
    {
        const string separator = ": ";
        var label = await locator.GetAttributeAsync("aria-label");
        if (label is null)
        {
            throw new InvalidOperationException("Expected an aria-label on the locator but found none.");
        }

        var separatorIndex = label.IndexOf(separator, StringComparison.Ordinal);
        if (separatorIndex < 0)
        {
            throw new InvalidOperationException($"Expected an aria-label of the shape '{{subject}}: {{state}}' but found '{label}'.");
        }

        return label[(separatorIndex + separator.Length)..];
    }
}
