namespace DnDFightTool.Domain.DnDEntities.Characters;

// TODO evaluate the possibility to have an NPC category.
// Not a player, but not a template for a monster either, aka keeps hps between fights for instance.

/// <summary>
///     The type of a character, could be a player or a monster
/// </summary>
public enum CharacterType
{
    /// <summary>
    ///     When a character is a player, in addition to being what it means, it also means that it when added to fight, its the actual character that's added.
    ///     At the end of the fight, the character is saved back to the repository.
    /// </summary>
    Player,
    /// <summary>
    ///     When a character is a monster, in addition to being what it means, it also means that it's a simple template that, when added to a fight, is duplicated.
    ///     At the end of the fight, the data of the monsters are lost.
    /// </summary>
    Monster,
    Unknown
}