using MudBlazor;
using MudIcons = MudBlazor.Icons;

namespace SharedComponents.Buttons.AtomicButtonsPreset;

public class EditButton : ButtonBase
{
    public EditButton() : base(Color.Primary, MudIcons.Material.Filled.Edit, false)
    {
    }
}
