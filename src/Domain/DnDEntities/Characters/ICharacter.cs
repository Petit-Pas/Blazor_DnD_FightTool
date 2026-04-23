using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.ArmorClasses;
using DnDFightTool.Domain.DnDEntities.DamageAffinities;
using DnDFightTool.Domain.DnDEntities.Dices.Modifiers;
using DnDFightTool.Domain.DnDEntities.HitPoint;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using DnDFightTool.Domain.DnDEntities.Saves;
using DnDFightTool.Domain.DnDEntities.Skills;
using DnDFightTool.Domain.DnDEntities.Statuses;

namespace DnDFightTool.Domain.DnDEntities.Characters;

/// <summary>
///     Represent a single character in the game.
///     Could be a player, could be a monster.
/// </summary>
public interface ICharacter
{
    /// <summary>
    ///     The ability scores of the character
    /// </summary>
    AbilityScoresCollection AbilityScores { get; }

    /// <summary>
    ///     The armor class of the character
    /// </summary>
    ArmorClass ArmorClass { get; }

    /// <summary>
    ///     The damage affinities of the character
    /// </summary>
    DamageAffinitiesCollection DamageAffinities { get; }

    /// <summary>
    ///     The default DC to use for spells and effects used by this character
    /// </summary>
    CharacterDifficultyClassTemplate Dc { get; }

    /// <summary>
    ///     The hit points of the character
    /// </summary>
    HitPoints HitPoints { get; }

    /// <summary>
    ///     A unique, non meaningful identifier for this character
    /// </summary>
    Guid Id { get; }

    /// <summary>
    ///     The martial attacks that the character can use
    ///     A martial attack is anything that isn't a spell/effect.
    ///     It is usually done with a weapon or a bodily feature (claws, tail, bite, etc.)
    /// </summary>
    MartialAttackTemplateCollection MartialAttacks { get; }

    /// <summary>
    ///     A meaningful name for this character
    /// </summary>
    string Name { get; set; }

    /// <summary>
    ///     The skills of the character
    /// </summary>
    SkillCollection Skills { get; }

    /// <summary>
    ///     The type of the character, can be either a player or a monster
    /// </summary>
    CharacterType Type { get; }

    /// Helper methods


    /// <summary>
    ///     Basically fetches the dexterity modifier
    /// </summary>
    ScoreModifier GetInitiativeModifier()
    {
        return AbilityScores.GetModifier(AbilityEnum.Dexterity);
    }

    /// <summary>
    ///     This method is made to fetch a possibly applied status by it GUID.
    ///     It could come from anywhere from attacks and spells.
    ///     This is required for the status commands to be generic.
    /// </summary>
    /// <param name="statusId"></param>
    /// <returns></returns>
    StatusTemplate? GetPossiblyAppliedStatus(Guid statusId);
}