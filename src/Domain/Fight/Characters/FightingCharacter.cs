
using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.AttackRolls.ArmorClasses;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.DamageAffinities;
using DnDFightTool.Domain.DnDEntities.HitPoint;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using DnDFightTool.Domain.DnDEntities.Saves;
using DnDFightTool.Domain.DnDEntities.Skills;
using DnDFightTool.Domain.DnDEntities.Statuses;

namespace DnDFightTool.Domain.Fight.Characters;

/// <summary>
///     Wraps a character with additional information required for a fight
/// </summary>
public class FightingCharacter : ICharacter
{
    private readonly Character _character;

    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="character"></param>
	public FightingCharacter(Character character)
	{
        _character = character ?? throw new ArgumentNullException(nameof(character));
	}

    public Guid Id => _character.Id;

    public AbilityScoresCollection AbilityScores => _character.AbilityScores;

    public ArmorClass ArmorClass => _character.ArmorClass;

    public DamageAffinitiesCollection DamageAffinities => _character.DamageAffinities;

    public CharacterDifficultyClassTemplate Dc => _character.Dc;

    public HitPoints HitPoints => _character.HitPoints;

    public MartialAttackTemplateCollection MartialAttacks => _character.MartialAttacks;

    public string Name => _character.Name;

    public SkillCollection Skills => _character.Skills;

    public CharacterType Type => _character.Type;

    // TODO this should maybe only be exposed on the fighter.
    // But that would required to switch every existing command to use fighters instead of the character
    // So it's a task by itself.
    public StatusTemplate? GetPossiblyAppliedStatus(Guid statusId)
    {
        return _character.GetPossiblyAppliedStatus(statusId);
    }
}