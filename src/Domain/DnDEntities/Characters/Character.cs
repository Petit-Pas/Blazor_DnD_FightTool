using System.ComponentModel.DataAnnotations;
using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.AttackRolls.ArmorClasses;
using DnDFightTool.Domain.DnDEntities.DamageAffinities;
using DnDFightTool.Domain.DnDEntities.HitPoint;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using DnDFightTool.Domain.DnDEntities.Saves;
using DnDFightTool.Domain.DnDEntities.Skills;
using DnDFightTool.Domain.DnDEntities.Statuses;
using FastDeepCloner;

namespace DnDFightTool.Domain.DnDEntities.Characters;

/// <summary>
///     Represent a single character in the game.
///     Could be a player, could be a monster.
/// </summary>
public class Character
{
    /// <summary>
    ///     empty ctor.
    ///     Should only be used by serialization
    ///     This was necessary, because if we established the default values in this ctor, then the default values would always be added upon each construction
    /// </summary>
    public Character() : this(false)
    {
    }

    /// <summary>
    ///     Ctor that should be used in the code.
    ///     Allows to set the default values for a standard character (abilities, skills, affinities, etc.)
    /// </summary>
    /// <param name="withDefaults"></param>
    public Character(bool withDefaults = false)
    {
        AbilityScores = new AbilityScoresCollection(withDefaults);
        Skills = new SkillCollection(withDefaults);
        DamageAffinities = new DamageAffinitiesCollection(withDefaults);
        HitPoints = new HitPoints();
        MartialAttacks = new MartialAttackTemplateCollection(withDefaults);
        // TODO a "DC" token in here will make a stack overflow
        // DificultyClass should only be used fot he saving directly, but not for the configuration in itself
        // DCTemplate? like for DiceThrowTemplate? 
        Dc = new DifficultyClass("10");
    }

    /// <summary>
    ///     A unique, non meaningful identifier for this character
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    ///     A meaningful name for this character
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    ///     The type of the character, can be either a player or a monster
    /// </summary>
    public CharacterType Type { get; set; } = CharacterType.Unknown;

    /// <summary>
    ///     The armor class of the character
    /// </summary>
    public ArmorClass ArmorClass { get; set; } = new ();

    /// <summary>
    ///     The ability scores of the character
    /// </summary>
    public AbilityScoresCollection AbilityScores { get; set; }

    /// <summary>
    ///     The skills of the character
    /// </summary>
    public SkillCollection Skills { get; set; }

    /// <summary>
    ///     The damage affinities of the character
    /// </summary>
    public DamageAffinitiesCollection DamageAffinities { get; set; }

    /// <summary>
    ///     The martial attacks that the character can use
    ///     A martial attack is anything that isn't a spell/effect.
    ///     It is usually done with a weapon or a bodily feature (claws, tail, bite, etc.)
    /// </summary>
    public MartialAttackTemplateCollection MartialAttacks { get; set; }

    /// <summary>
    ///     The hit points of the character
    /// </summary>
    public HitPoints HitPoints { get; set; }

    /// <summary>
    ///     The default DC to use for spells and effects used by this character
    /// </summary>
    public DifficultyClass Dc { get; set; }

    /// <summary>
    ///     Will do a deep copy of this character, then give it a new ID
    /// </summary>
    /// <returns></returns>
    public Character Duplicate()
    {
        var copy = this.Clone();
        copy.Id = Guid.NewGuid();
        return copy;
    }

    /// <summary>
    ///     This method is made to fetch a possibly applied status by it GUID.
    ///     It could come from anywhere from attacks and spells.
    ///     This is required for the status commands to be generic.
    /// </summary>
    /// <param name="statusId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public StatusTemplate? GetPossiblyAppliedStatus(Guid statusId)
    {
        return MartialAttacks.Values.SelectMany(x => x.Statuses).FirstOrDefault(x => x.Key == statusId).Value;
    }
}