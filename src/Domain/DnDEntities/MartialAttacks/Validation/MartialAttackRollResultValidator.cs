using DnDFightTool.Domain.DnDEntities.Damage.Validation;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows.Validation;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using FluentValidation;

namespace DnDEntities.MartialAttacks.Validation;

public class MartialAttackRollResultValidator : AbstractValidator<MartialAttackRollResult>
{
    public MartialAttackRollResultValidator()
    {
        RuleFor(x => x.TargetId)
            .NotEmpty();

        RuleFor(x => x.HitRoll)
            .SetValidator(new HitRollResultValidator());

        RuleForEach(x => x.DamageRolls)
            .SetValidator(new DamageRollResultValidator());
    }
}
