using DnDFightTool.Domain.DnDEntities.Characters;

namespace DnDEntitiesBlazorComponents;

/// <summary>
///     When registered as a singleton, it allows views to pass context easily when navigating from page to page.
/// </summary>
public interface IGlobalEditContext
{
    Character? Character { get; }

    void EditCharacter(Character character);
}
