using DnDFightTool.Domain.CharacterSheet.AbilityScores;
using DnDFightTool.Domain.CharacterSheet.ArmorClasses;
using DnDFightTool.Domain.CharacterSheet.HitPoint;
using Extensions;
using FluentValidation;

namespace DnDFightTool.Domain.CharacterSheet.Characters.Validation;

/// <summary>
///     Validator for <see cref="Character"/>
/// </summary>
public class CharacterValidator : PropertyTargetedValidator<ICharacter>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public CharacterValidator(
        IValidator<HitPoints> hitPointsValidator,
        IValidator<ArmorClass> armorClassValidator,
        IValidator<AbilityScoresCollection> abilityScoresValidator)
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.HitPoints)
            .SetValidator(hitPointsValidator);

        RuleFor(x => x.ArmorClass)
            .SetValidator(armorClassValidator);

        RuleFor(x => x.AbilityScores)
            .SetValidator(abilityScoresValidator);
    }
}