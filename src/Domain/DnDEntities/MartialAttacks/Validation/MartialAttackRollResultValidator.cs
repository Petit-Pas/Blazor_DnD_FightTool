using DnDEntities.Damage.Validation;
using DnDEntities.Dices.DiceThrows.Validation;
using Fight.MartialAttacks;
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
