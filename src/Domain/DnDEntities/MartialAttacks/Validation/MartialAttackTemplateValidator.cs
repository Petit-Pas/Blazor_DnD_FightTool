using DnDFightTool.Domain.DnDEntities.Statuses;
using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.MartialAttacks.Validation;

/// <summary>
///     Validator for <see cref="MartialAttackTemplate" />
/// </summary>
public class MartialAttackTemplateValidator : AbstractValidator<MartialAttackTemplate>
{
    // TODO I should apply this IoC rather than the singleton approach everywhere.

    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="statusTemplateValidator"></param>
    public MartialAttackTemplateValidator(AbstractValidator<StatusTemplate> statusTemplateValidator)
    {
        RuleFor(template => template.Name)
            .NotEmpty();

        RuleForEach(template => template.Statuses.Values)
            .SetValidator(statusTemplateValidator);
    }
}
