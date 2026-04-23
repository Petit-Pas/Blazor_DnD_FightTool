using FluentValidation;

namespace DnDFightTool.Domain.CharacterSheet.Statuses.Validation;

public class StatusTemplateValidator : AbstractValidator<StatusTemplate>
{
    public StatusTemplateValidator()
    {
        RuleFor(template => template.Name)
            .NotEmpty();
    }
}
