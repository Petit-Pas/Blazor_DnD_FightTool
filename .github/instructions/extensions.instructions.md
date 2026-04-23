---
applyTo: "src/Infrastructure/Extensions/**/*.cs"
---

# Infrastructure Extensions Conventions

- **Namespace**: `Extensions` (short, no prefix). Imported implicitly across the solution.
- **Enum helpers**: `EnumExtensions.GetAttribute<TAttribute>(this Enum value)` — retrieves a single custom attribute from an enum field via reflection. Logs warnings (not exceptions) for missing attributes.
- **Array helpers**: `ArrayExtensions` — includes `.IsEmpty()` and similar utility methods.
- **String helpers**: `StringExtensions` — includes `.When(bool condition)` for conditional CSS class building.
- **Regex validation**: `IRegexValidated` interface with `Regex` property and `Expression` setter. `IRegexValidatedExtensions.IsValid(...)` for validation.
- **Validators**: `PropertyTargetedValidator<T> : AbstractValidator<T>` — base class for all FluentValidation validators in this solution. Provides `ValidateValue` delegate for per-property Blazor form validation.
- **Style**: Utility/extension classes are kept small and focused. One concept per file.
