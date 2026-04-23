using DnDFightTool.Domain.CharacterSheet.Dices;
using DnDFightTool.Domain.CharacterSheet.Statuses;
using Extensions;
using FluentValidation;

namespace DnDFightTool.Domain.CharacterSheet.MartialAttacks.Validation;

/// <summary>
///     Validator for <see cref="MartialAttackTemplate" />
/// </summary>
public class MartialAttackTemplateValidator : PropertyTargetedValidator<MartialAttackTemplate>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="statusTemplateValidator"></param>
    public MartialAttackTemplateValidator(
        IValidator<StatusTemplate> statusTemplateValidator,
        IValidator<DiceRollModifiersTemplate> diceThrowModifierTemplate)
    {
        RuleFor(template => template.Name)
            .NotEmpty();

        RuleForEach(template => template.Statuses.Values)
            .SetValidator(statusTemplateValidator);

        RuleFor(template => template.ToHitModifiers)
            .SetValidator(diceThrowModifierTemplate);
    }
}
