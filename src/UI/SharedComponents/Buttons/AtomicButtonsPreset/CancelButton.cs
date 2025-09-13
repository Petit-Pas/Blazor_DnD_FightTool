using MudBlazor;
using MudIcons = MudBlazor.Icons;

namespace SharedComponents.Buttons.AtomicButtonsPreset;

public class CancelButton : ButtonBase
{
    public CancelButton() : base(Color.Default, MudIcons.Material.Filled.Cancel, true)
    {
    }
}
