using DnDFightTool.Domain.CharacterSheet.Dices.Validation;
using Extensions;
using Microsoft.AspNetCore.Components;

namespace SharedComponents.Inputs;

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
    private bool _isCurrentlyValid = true;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (Value is not null && _currentInput == "")
        {
            _currentInput = Value.Expression;
        }
    }

    public bool Validate()
    {
        return _isCurrentlyValid;
    }

    private string? GetRegexError(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Cannot be empty";
        }
        return DiceRollTemplateValidator.MatchRegex(value)
            ? null
            : ErrorMessage;
    }

    private void Validate(string value)
    {
        var regexError = GetRegexError(value);
        if (regexError != null)
        {
            _isCurrentlyValid = false;
            _validationError = regexError;
        }
        else
        {
            _isCurrentlyValid = true;
            _validationError = null;
        }
    }

    private async Task OnInputChangedAsync(string value)
    {
        _currentInput = value;
        Validate(value);

        // Only update model when valid
        if (_isCurrentlyValid)
        {
            Value!.Expression = _currentInput;
            await ValueChanged.InvokeAsync(Value);
        }
    }
}
