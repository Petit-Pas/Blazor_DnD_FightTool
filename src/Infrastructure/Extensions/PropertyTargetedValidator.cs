using FluentValidation;

namespace Extensions;

public class PropertyTargetedValidator<T> : AbstractValidator<T>
{
    /// <summary>
    ///     Allows to easilt validate a single property instead of the whole object.
    /// </summary>
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<T>.CreateWithOptions((T)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
        {
            return [];
        }

        return result.Errors.Select(e => e.ErrorMessage);
    };
}
