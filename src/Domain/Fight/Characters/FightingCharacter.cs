
using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.AttackRolls.ArmorClasses;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.DamageAffinities;
using DnDFightTool.Domain.DnDEntities.HitPoint;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using DnDFightTool.Domain.DnDEntities.Saves;
using DnDFightTool.Domain.DnDEntities.Skills;
using DnDFightTool.Domain.DnDEntities.Statuses;
using Mapping;

namespace DnDFightTool.Domain.Fight.Characters;

/// <summary>
///     Wraps a character with additional information required for a fight
/// </summary>
public class FightingCharacter : ICharacter
{
    private readonly Character _character;

    #region character mirroring

    public Guid Id => _character.Id;

    public AbilityScoresCollection AbilityScores => _character.AbilityScores;

    public ArmorClass ArmorClass => _character.ArmorClass;

    public DamageAffinitiesCollection DamageAffinities => _character.DamageAffinities;

    public CharacterDifficultyClassTemplate Dc => _character.Dc;

    public HitPoints HitPoints => _character.HitPoints;

    public MartialAttackTemplateCollection MartialAttacks => _character.MartialAttacks;

    public string Name { get => _character.Name; set => _character.Name = value; }

    public SkillCollection Skills => _character.Skills;

    public CharacterType Type => _character.Type;

    #endregion character mirroring

    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="character"></param>
	public FightingCharacter(Character character)
	{
        _character = character ?? throw new ArgumentNullException(nameof(character));
	}

    /// <summary>
    ///     Creates a deep copy of this FightingCharacter, including the underlying character.
    ///     For this entity, I could not use FastDeepCloner as wwon't be able to do the private character properly.
    /// </summary>
    /// <param name="mapper"></param>
    /// <returns></returns>
    public FightingCharacter Copy(IMapper mapper)
    {
        return new FightingCharacter(mapper.Copy(_character));
    }

    /// <summary>
    ///     Holds the initiative roll
    /// </summary>
    public int InitiativeRoll { get; set; } = 0;

    /// <summary>
    ///     Gets the actual initiave, including the dexterity modifier
    /// </summary>
    /// <returns></returns>
    public int GetInitiativeTotal()
    {
        return InitiativeRoll + ((ICharacter)this).GetInitiativeModifier();
    }

    public StatusTemplate? GetPossiblyAppliedStatus(Guid statusId)
    {
        return _character.GetPossiblyAppliedStatus(statusId);
    }

    public static Func<FightingCharacter, (int, int)> InitiativeSortKey =>
        fc => (-fc.GetInitiativeTotal(), -((ICharacter)fc).GetInitiativeModifier());
}