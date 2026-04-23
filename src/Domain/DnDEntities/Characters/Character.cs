using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.ArmorClasses;
using DnDFightTool.Domain.DnDEntities.DamageAffinities;
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
public class Character : ICharacter
{
    /// <inheritdoc />
    public Character() : this(false)
    {
    }

    /// <inheritdoc />
    public Character(bool withDefaults = false)
    {
        if (withDefaults)
        {
            Name = "Name";
        }
        AbilityScores = new AbilityScoresCollection(withDefaults);
        Skills = new SkillCollection(withDefaults);
        DamageAffinities = new DamageAffinitiesCollection(withDefaults);
        HitPoints = new HitPoints();
        MartialAttacks = new MartialAttackTemplateCollection(withDefaults);
        Dc = new CharacterDifficultyClassTemplate("10");
    }

    /// <inheritdoc />
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <inheritdoc />
    public string Name { get; set; } = "";

    /// <inheritdoc />
    public CharacterType Type { get; set; } = CharacterType.Unknown;

    /// <inheritdoc />
    public ArmorClass ArmorClass { get; set; } = new();

    /// <inheritdoc />
    public AbilityScoresCollection AbilityScores { get; set; }

    /// <inheritdoc />
    public SkillCollection Skills { get; set; }

    /// <inheritdoc />
    public DamageAffinitiesCollection DamageAffinities { get; set; }

    /// <inheritdoc />
    public MartialAttackTemplateCollection MartialAttacks { get; set; }

    /// <inheritdoc />
    public HitPoints HitPoints { get; set; }

    /// <inheritdoc />
    public CharacterDifficultyClassTemplate Dc { get; set; }

    /// <inheritdoc />
    public StatusTemplate? GetPossiblyAppliedStatus(Guid statusId)
    {
        return MartialAttacks.Values.SelectMany(x => x.Statuses).FirstOrDefault(x => x.Key == statusId).Value;
    }
}