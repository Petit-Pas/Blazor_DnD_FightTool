using DnDFightTool.Infrastructure.Extensions;
using FluentValidation;

namespace DnDFightTool.Domain.CharacterSheet.Statuses.Validation;

public class StatusTemplateValidator : PropertyTargetedValidator<StatusTemplate>
{
    public StatusTemplateValidator()
    {
        RuleFor(template => template.Name)
            .NotEmpty();
    }
}
