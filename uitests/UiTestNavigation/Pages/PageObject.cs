using Microsoft.Playwright;

namespace DnDFightTool.UiTests.UiTestNavigation.Pages;

/// <summary>
///     Base type for a typed object over a routable page. A page object owns the browser <see cref="Page"/> and the host
///     <see cref="BaseUrl"/>, exposes one method per user intent the page supports, and hides every Playwright locator
///     behind those methods so a scenario body never writes one. Pages reachable by URL navigate through
///     <see cref="GotoAsync"/>; pages that render only from a scoped edit context are returned from a parent page's action
///     instead of being URL-navigable.
/// </summary>
public abstract class PageObject
{
    private readonly string _baseUrl;

    /// <summary>
    ///     Initializes the page object with the browser page it drives and the host base URL its routes resolve against.
    /// </summary>
    /// <param name="page">The Playwright page the scenario shares.</param>
    /// <param name="baseUrl">The running host's base address (e.g. <c>http://127.0.0.1:5000</c>).</param>
    protected PageObject(IPage page, string baseUrl)
    {
        Page = page ?? throw new ArgumentNullException(nameof(page));
        _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
    }

    /// <summary>
    ///     Gets the browser page this object drives. Exposed to derived page objects only; scenarios never touch it.
    /// </summary>
    protected IPage Page { get; }

    /// <summary>
    ///     Gets the host base URL, so a derived page object can hand it to a child page object it returns.
    /// </summary>
    protected string BaseUrl
    {
        get
        {
            return _baseUrl;
        }
    }

    /// <summary>
    ///     Navigates the browser to <paramref name="route"/> resolved against the host base URL and waits for the DOM to be
    ///     ready. Used by the URL-navigable pages; the edit pages do not call this because their route renders empty unless
    ///     a per-circuit edit context is set.
    /// </summary>
    /// <param name="route">The app-relative route, e.g. <c>/</c>, <c>/fighters</c>, <c>/fight-dashboard</c>.</param>
    protected async Task GotoAsync(string route)
    {
        var target = new Uri(new Uri(_baseUrl), route).ToString();
        await Page.GotoAsync(target, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
    }
}
