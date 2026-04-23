using Extensions;
using FluentValidation;

namespace DnDFightTool.Domain.CharacterSheet.ArmorClasses.Validation;

/// <summary>
///     Validator for <see cref="ArmorClass"/>
/// </summary>
public class ArmorClassValidator : PropertyTargetedValidator<ArmorClass>
{
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
