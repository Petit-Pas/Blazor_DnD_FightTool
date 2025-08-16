using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;

namespace DnDEntitiesBlazorComponents;

/// <summary>
///     When registered as a singleton, it allows views to pass context easily when navigating from page to page.
/// </summary>
public interface IGlobalEditContext : ICharacterEditContext, IAttackEditContext
{
}

public interface ICharacterEditContext
{
    /// <summary>
    ///     The character that is currently being edited.
    ///     That one cannot come from the repository, it needs to be a copy to allow for canceling.
    /// </summary>
    Character? Character { get; }
    /// <summary>
    ///     Navigates to the character edit page
    /// </summary>
    /// <param name="character"></param>
    void EditCharacter(Character character);
    /// <summary>
    ///     Saves the edited character, then navigates back
    /// </summary>
    void SaveEditedCharacter();
    /// <summary>
    ///     Cancels the character edition, then navigates back
    /// </summary>
    void CancelCharacterEdittion();
}

public interface IAttackEditContext
{
    /// <summary>
    ///     The attack that is currently being edited.
    /// </summary>
    MartialAttackTemplate? Attack { get; }
    /// <summary>
    ///     Navigates to the attack edit page 
    /// </summary>
    /// <param name="attackTemplate"></param>
    void EditAttack(MartialAttackTemplate attackTemplate);
    /// <summary>
    ///     Saves the edited attack in the currently edited character, then navigates back
    /// </summary>
    void SaveEditedAttack();
    /// <summary>
    ///     Cancels the attack edition, then navigates back
    /// </summary>
    void CancelAttackEdition();
}

