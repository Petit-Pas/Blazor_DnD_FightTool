---
applyTo: "**/*Validator*.cs"
---

# FluentValidation Conventions

- **Base class**: Inherit from `PropertyTargetedValidator<T>` (from `Extensions` project), not `AbstractValidator<T>` directly. This base class provides `ValidateValue` for per-property validation in Blazor forms.
- **Location**: Validators live in a `Validation/` subfolder next to the entity they validate.
- **Naming**: `{EntityName}Validator`.
- **Constructor**: Rules are defined in the constructor. Inject child validators via `IValidator<T>` and compose with `.SetValidator(...)`.
- **Messages**: Use `.WithMessage(...)` with descriptive, user-facing strings. Reference entity context (e.g., ability short name) in the message lambda.
- **Registration**: Validators are registered via `AddValidatorsFromAssemblyContaining<T>()` or `RegisterPropertyTargetedValidators(assembly)` in `MauiProgram.cs`.
- **No async rules**: All current validation rules are synchronous.

## Example

```csharp
public class AbilityScoreValidator : PropertyTargetedValidator<AbilityScore>
{
    public AbilityScoreValidator()
    {
        RuleFor(x => x.Score)
            .GreaterThanOrEqualTo(1).WithMessage(x => $"{x.Ability.ShortName()} cannot be lower than 1.")
            .LessThanOrEqualTo(30).WithMessage(x => $"{x.Ability.ShortName()} cannot be above 30.");
    }
}
```
