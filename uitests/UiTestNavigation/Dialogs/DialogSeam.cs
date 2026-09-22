using Microsoft.Playwright;

namespace DnDFightTool.UiTests.UiTestNavigation.Dialogs;

/// <summary>
///     Minimal typed placeholder for a modal surface. A page or component method that opens a modal returns one of these
///     seams so the fluent surface is complete and type-checked <b>now</b>, while the modal's interaction methods and the
///     scenarios that drive them are delivered in story 6. It carries only the browser page; it exposes no interaction
///     surface on purpose — driving the modal is out of scope here.
/// </summary>
public abstract class DialogSeam
{
    /// <summary>
    ///     Initializes the seam with the browser page the future interaction methods will drive.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    protected DialogSeam(IPage page)
    {
        Page = page ?? throw new ArgumentNullException(nameof(page));
    }

    /// <summary>
    ///     Gets the browser page the modal renders on. Reserved for the interaction methods story 6 adds.
    /// </summary>
    protected IPage Page { get; }
}
