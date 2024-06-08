using NeoBlazorphic.StyleParameters;

namespace SharedComponents.Inputs.Buttons.Actions;

public class DeleteButton : CircularActionButton
{
    public DeleteButton() : base("fa-regular fa-trash-can", ThemeColor.Danger)
    {
    }
}
