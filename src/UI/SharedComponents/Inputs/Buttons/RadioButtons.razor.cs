using Microsoft.AspNetCore.Components;

namespace SharedComponents.Inputs.Buttons;

public partial class RadioButtons<T> 
{
    [Parameter]
    public required Dictionary<string, T> KeyValues { get; set; }

    [Parameter]
    public required T Value { get; set; }

    [Parameter]
    public EventCallback<T> OnValueChanged { get;set; }

    private void UpdateValue(T newValue)
    {
        if (newValue != null && !newValue.Equals(Value))
        {
            Value = newValue;
            OnValueChanged.InvokeAsync(Value);
        }
    }
}
