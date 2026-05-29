
using DnDFightTool.Domain.CharacterSheet.AbilityScores;
using DnDFightTool.Domain.CharacterSheet.ArmorClasses;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.DamageAffinities;
using DnDFightTool.Domain.CharacterSheet.HitPoint;
using DnDFightTool.Domain.CharacterSheet.MartialAttacks;
using DnDFightTool.Domain.CharacterSheet.Saves;
using DnDFightTool.Domain.CharacterSheet.Skills;
using DnDFightTool.Domain.CharacterSheet.Statuses;
using DnDFightTool.Infrastructure.Mapping;

namespace DnDFightTool.Domain.Fight.Characters;

/// <summary>
///     Wraps a character with additional information required for a fight
/// </summary>
public class FightingCharacter : IFightingCharacter
{
    private readonly ICharacter _character;

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
    ///     The id of the originating <see cref="Character"/> template.
    ///     For players, this equals the wrapped character's <see cref="Character.Id"/>.
    ///     For monsters, this equals the source template id (NOT the regenerated id of the cloned copy).
    /// </summary>
    public Guid OriginalCharacterId { get; }

    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="character"></param>
    /// <param name="originalCharacterId">The id of the originating template.</param>
	public FightingCharacter(ICharacter character, Guid originalCharacterId)
	{
        _character = character ?? throw new ArgumentNullException(nameof(character));
        OriginalCharacterId = originalCharacterId;
	}

    /// <summary>
    ///     Creates a deep copy of this FightingCharacter, including the underlying character.
    /// </summary>
    /// <param name="mapper"></param>
    /// <returns></returns>
    public IFightingCharacter Copy(IMapper mapper)
    {
        return new FightingCharacter(mapper.Copy(_character), OriginalCharacterId);
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

}