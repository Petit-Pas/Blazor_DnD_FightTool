using MudBlazor;
using MudIcons = MudBlazor.Icons;

namespace SharedComponents.Buttons.AtomicButtonsPreset;

public class SaveButton : ButtonBase
{
    public SaveButton() : base(Color.Success, MudIcons.Material.Filled.Save, false)
    {
    }
}
