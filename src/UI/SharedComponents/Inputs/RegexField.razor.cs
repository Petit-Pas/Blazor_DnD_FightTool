using DnDFightTool.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components;

namespace DnDFightTool.UI.SharedComponents.Inputs;

public partial class RegexField<T>
    where T : IRegexValidated
{
    [Parameter]
    public string Label { get; set; } = string.Empty;

    [Parameter]
    public T? Value { get; set; }

    [Parameter]
    public EventCallback<T> ValueChanged { get; set; }

    protected virtual string ErrorMessage => "Invalid Expression";

    private string _currentInput = "";
    private string? _validationError;
    private T? _syncedValue;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (Value is null)
        {
            _syncedValue = default;
            _currentInput = "";
            _validationError = null;
            return;
        }
        // Resync only when bound to another object; otherwise a re-render would overwrite the user's invalid input.
        if (!ReferenceEquals(Value, _syncedValue))
        {
            _syncedValue = Value;
            _currentInput = Value.Expression;
            Validate(_currentInput);
        }
    }

    public bool Validate()
    {
        return _validationError is null;
    }

    private string? GetValidationError(string? value)
    {
        return Value is null
            ? null
            : GetRegexError(value ?? "");
    }

    private string? GetRegexError(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Cannot be empty";
        }
        return Value!.IsValid(value)
            ? null
            : ErrorMessage;
    }

    private void Validate(string value)
    {
        _validationError = GetRegexError(value);
    }

    private async Task OnInputChangedAsync(string value)
    {
        _currentInput = value;
        Validate(value);

        // Only update model when valid
        if (_validationError is null)
        {
            Value!.Expression = _currentInput;
            await ValueChanged.InvokeAsync(Value);
        }
    }
}
