using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace AspNetCoreExtensions.Navigations;

/// <summary>
///     Wrapper for <see cref="NavigationManager"/> to be able to track the navigation history 
///         and navigate back easily.
/// </summary>
internal class StateFullNavigation : IStateFullNavigation
{
    private readonly NavigationManager _navigationManager;

    private List<string> _pages = new(_maxSize) { "https://0.0.0.1/characters" }; // A very shitty way to fix the fact that we can't catch the original navigation.
    private const int _maxSize = 100;

    public StateFullNavigation(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));

        _navigationManager.LocationChanged += NavigationManager_LocationChanged;
    }

    /// <summary>
    ///     Handler for the LocationChanged event of the NavigationManager.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NavigationManager_LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (_pages.Count == 0)
        {
            AddToLocalHistory(e.Location);
        }
        else if (_pages.Last() == e.Location)
        {
            return;
        }
        else if (_pages.Count >= 2 && _pages[^2] == e.Location)
        {
            RemoveLastPageFromLocalHistory();
        }
        else
        {
            AddToLocalHistory(e.Location);
        }
    }

    private void AddToLocalHistory(string page)
    {
        if (_pages.Count == _maxSize)
        {
            _pages = [.. _pages.GetRange(1, _maxSize - 1), page];
        }
        else
        {
            _pages.Add(page);
        }
    }

    private void RemoveLastPageFromLocalHistory()
    {
        _pages.Remove(_pages.Last());
    }

    public void NavigateTo(string page)
    {
        _navigationManager.NavigateTo(page);
    }

    public void NavigateBack()
    {
        if (_pages.Count > 1)
        {
            _navigationManager.NavigateTo(_pages[^2]);
        }
        else
        {
            _navigationManager.NavigateTo("/"); // Navigate to home or default page if no history
        }
    }
}
