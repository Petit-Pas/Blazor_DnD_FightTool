using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.Saves.Validation;

public class SaveRollResultValidator : AbstractValidator<SaveRollResult>
{
    public SaveRollResultValidator()
    {
        RuleFor(x => x.RolledResult)
            .InclusiveBetween(1, 20);
    }
}
