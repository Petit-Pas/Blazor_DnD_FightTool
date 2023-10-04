using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.Characters;

namespace DnDFightTool.Domain.DnDEntities.Saves;

// TODO at some point it should allow for temporary modifiers

public class SaveRollResult
{
    public SaveRollResult(DifficultyClass target, AbilityEnum ability)
    {
        Target = target;
        Ability = ability;
    }


    /// <summary>
    ///     Rolled result, does not contain bonuses
    /// </summary>
    public int RolledResult { get; set; }

    /// <summary>
    ///     DC of the save
    /// </summary>
    public DifficultyClass Target { get; set; }

    /// <summary>
    ///     The ability targeted by the save
    /// </summary>
    public AbilityEnum Ability { get; set; }

    public virtual bool IsSuccesfull(Character targetCharacter, Character casterCharacter)
    {
        var target = Target.GetValue(casterCharacter);

        var modifier = targetCharacter.AbilityScores.GetModifierWithMastery(Ability);

        return RolledResult + modifier.Modifier >= target;
    }
}
