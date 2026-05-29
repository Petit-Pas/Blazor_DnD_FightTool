using MudBlazor;
using MudIcons = MudBlazor.Icons;

namespace DnDFightTool.UI.SharedComponents.Buttons.AtomicButtonsPreset;

public class SubtractButton : ButtonBase
{
    public SubtractButton() : base(Color.Primary, MudIcons.Material.Filled.RemoveCircle, false)
    {
    }
}
