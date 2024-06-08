using Microsoft.AspNetCore.Components;
using NeoBlazorphic.StyleParameters;

namespace SharedComponents.Inputs.Buttons.Actions;

public partial class CircularActionButton
{
    public CircularActionButton(string buttonIcon, ThemeColor themeColor)
    {
        _buttonIcon = buttonIcon;
        _themeColor = themeColor;
    }

    [Parameter]
    public virtual EventCallback OnClick { get; set; }

    [Parameter]
    public virtual int SizeInPx { get; set; } = 28;

    private string _buttonIcon;
    private ThemeColor _themeColor;

    private async Task NotifyClick()
    {
        await OnClick.InvokeAsync();
    }
}
