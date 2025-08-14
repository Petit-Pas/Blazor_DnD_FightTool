using AspNetCoreExtensions.Navigations;
using DnDFightTool.Domain.DnDEntities.Characters;

namespace DnDEntitiesBlazorComponents;

internal class GlobalEditContext : IGlobalEditContext
{
    private readonly IStateFullNavigation _stateFullNavigation;

    public GlobalEditContext(IStateFullNavigation stateFullNavigation)
    {
        _stateFullNavigation = stateFullNavigation ?? throw new ArgumentNullException(nameof(stateFullNavigation));
    }

    public Character? Character { get; private set; }

    public void EditCharacter(Character character)
    {
        Character = character;
        _stateFullNavigation.NavigateTo("Characters/Edit");
    }
}
