using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows.Validation;
using DnDFightTool.Domain.DnDEntities.Statuses;
using Extensions;
using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.MartialAttacks.Validation;

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
        IValidator<DiceThrowModifiersTemplate> diceThrowModifierTemplate)
    {
        RuleFor(template => template.Name)
            .NotEmpty();

        RuleForEach(template => template.Statuses.Values)
            .SetValidator(statusTemplateValidator);

        RuleFor(template => template.ToHitModifiers)
            .SetValidator(diceThrowModifierTemplate);
    }
}
