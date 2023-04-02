using DnDEntities.AbilityScores;
using DnDEntities.AttackRolls.ArmorClasses;
using DnDEntities.DamageAffinities;
using DnDEntities.Skills;

namespace DnDEntities.Characters;

public class Character
{
    /// <summary>
    ///     Should only be used by serialization
    /// </summary>
    public Character() : this(false)
    {
    }

    public Character(bool withDefaults = false)
    {
        AbilityScores = new AbilityScoresCollection(withDefaults);
        Skills = new SkillCollection(withDefaults);
        DamageAffinities = new DamageAffinitiesCollection(withDefaults);

    }

    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = "";

    public CharacterType Type { get; set; } = CharacterType.Unknown;

    public ArmorClass ArmorClass { get; set; } = new ();

    public AbilityScoresCollection AbilityScores { get; set; }

    public SkillCollection Skills { get; set; }

    public DamageAffinitiesCollection DamageAffinities { get; set; }

}