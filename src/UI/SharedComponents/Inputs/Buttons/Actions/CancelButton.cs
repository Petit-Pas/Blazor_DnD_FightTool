using NeoBlazorphic.StyleParameters;

namespace SharedComponents.Inputs.Buttons.Actions;

public class CancelButton : CircularActionButton
{
    public CancelButton() : base("fa-solid fa-xmark", ThemeColor.Danger)
    {
    }
}
