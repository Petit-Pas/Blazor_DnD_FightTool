using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.AttackRolls.ArmorClasses.Validation;

/// <summary>
///     Validator for <see cref="ArmorClass"/>
/// </summary>
public class ArmorClassValidator : AbstractValidator<ArmorClass>
{
    /// <summary>
    ///     Since the usual comportment of a validator is to be stateless, we can use a singleton
    ///     This avoids for the need of injection or newing up when setting this validator as a sub validator
    /// </summary>
    public static readonly ArmorClassValidator Instance = new();

    /// <summary>
    ///     Ctor
    /// </summary>
    public ArmorClassValidator()
    {
        RuleFor(x => x.BaseArmorClass)
            .InclusiveBetween(1, 30);

        RuleFor(x => x.ShieldArmorClass)
            .GreaterThan(0);
    }
}