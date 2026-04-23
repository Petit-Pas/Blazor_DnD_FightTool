using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Dices;

namespace DnDFightTool.Domain.DnDEntities.Saves;

// TODO at some point it should allow for temporary modifiers

/// <summary>
///    Result of a save roll. Inherits <see cref="RawD20RollResult"/> so it carries
///    the d20 value (1–20) through <see cref="IDiceRollResult.Result"/>.
/// </summary>
public class SaveRollResult : RawD20RollResult
{
    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="target"></param>
    /// <param name="ability"></param>
    public SaveRollResult(DifficultyClassTemplate target, AbilityEnum ability)
    {
        Target = target;
        Ability = ability;
    }

    /// <summary>
    ///     DC of the save, Needs to be evaluated with the caster's ability score & all
    /// </summary>
    public DifficultyClassTemplate Target { get; set; }

    /// <summary>
    ///     The ability targeted by the save
    /// </summary>
    public AbilityEnum Ability { get; set; }

    // TODO this virtual modifier should be removed and the test should be made differently. Using a proper factory to say you want a successful save or not rather than those faked entities.
    /// <summary>
    ///    Checks whether the save is successful or not
    /// </summary>
    /// <param name="casterCharacter"></param>
    /// <param name="targetCharacter"></param>
    /// <returns></returns>
    public virtual bool IsSuccessful(ICharacter casterCharacter, ICharacter targetCharacter)
    {
        var target = Target.GetValue(casterCharacter);

        var modifier = targetCharacter.AbilityScores.GetSavingModifier(Ability);

        return Result + modifier.Modifier >= target;
    }
}
