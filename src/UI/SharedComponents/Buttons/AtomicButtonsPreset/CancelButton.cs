using MudBlazor;
using MudIcons = MudBlazor.Icons;

namespace DnDFightTool.UI.SharedComponents.Buttons.AtomicButtonsPreset;

public class CancelButton : ButtonBase
{
    public CancelButton() : base(Color.Default, MudIcons.Material.Filled.Cancel, true)
    {
    }
}
