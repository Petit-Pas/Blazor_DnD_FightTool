using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.AttackRolls.ArmorClasses;
using DnDFightTool.Domain.DnDEntities.DamageAffinities;
using DnDFightTool.Domain.DnDEntities.Skills;
using DnDFightTool.Domain.DnDEntities.HitPoint;
using FastDeepCloner;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;

namespace DnDFightTool.Domain.DnDEntities.Characters;

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
        HitPoints = new HitPoints(withDefaults);
        MartialAttacks = new MartialAttackTemplateCollection(withDefaults);
    }

    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = "";

    public CharacterType Type { get; set; } = CharacterType.Unknown;

    public ArmorClass ArmorClass { get; set; } = new ();

    public AbilityScoresCollection AbilityScores { get; set; }

    public SkillCollection Skills { get; set; }

    public DamageAffinitiesCollection DamageAffinities { get; set; }

    public MartialAttackTemplateCollection MartialAttacks { get; set; }

    public HitPoints HitPoints { get; set; }

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
}