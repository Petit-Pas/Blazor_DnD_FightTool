using DnDFightTool.Domain.CharacterSheet.AbilityScores;
using DnDFightTool.Domain.CharacterSheet.ArmorClasses;
using DnDFightTool.Domain.CharacterSheet.DamageAffinities;
using DnDFightTool.Domain.CharacterSheet.HitPoint;
using DnDFightTool.Domain.CharacterSheet.MartialAttacks;
using DnDFightTool.Domain.CharacterSheet.Saves;
using DnDFightTool.Domain.CharacterSheet.Skills;
using DnDFightTool.Domain.CharacterSheet.Statuses;

namespace DnDFightTool.Domain.CharacterSheet.Characters;

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