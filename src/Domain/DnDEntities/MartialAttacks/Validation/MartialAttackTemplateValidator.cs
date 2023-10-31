using DnDFightTool.Domain.DnDEntities.Statuses;
using DnDFightTool.Domain.DnDEntities.Statuses.Validation;
using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.MartialAttacks.Validation;

public class MartialAttackTemplateValidator : AbstractValidator<MartialAttackTemplate>
{
    public MartialAttackTemplateValidator(AbstractValidator<StatusTemplate> statusTemplateValidator)
    {
        RuleFor(template => template.Name)
            .NotEmpty();

        RuleForEach(template => template.Statuses)
            .SetValidator(statusTemplateValidator);
    }
}
