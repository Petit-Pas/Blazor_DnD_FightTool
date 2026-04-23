using MudBlazor;
using MudIcons = MudBlazor.Icons;

namespace DnDFightTool.UI.SharedComponents.Buttons.AtomicButtonsPreset;

public class DuplicateButton : ButtonBase
{
    public DuplicateButton() : base(Color.Primary, MudIcons.Material.Filled.ContentCopy, false)
    {
    }
}
