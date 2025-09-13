using MudBlazor;
using MudIcons = MudBlazor.Icons;

namespace SharedComponents.Buttons.AtomicButtonsPreset;

public class AddButton : ButtonBase
{
    public AddButton() : base(Color.Primary, MudIcons.Material.Filled.AddCircle, true)
    {
    }
}
