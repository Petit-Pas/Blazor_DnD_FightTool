using Microsoft.AspNetCore.Components;

namespace SharedComponents.Inputs.Buttons;

public partial class RadioButtons<T> 
{
    [Parameter]
    public Dictionary<string, T> KeyValues { get; set; }

    [Parameter]
    public T Value { get; set; }

    [Parameter]
    public EventCallback<T> OnValueChanged { get;set; }

    private void UpdateValue(T newValue)
    {
        if (!newValue.Equals(Value))
        {
            Value = newValue;
            OnValueChanged.InvokeAsync(Value);
        }
    }
}
