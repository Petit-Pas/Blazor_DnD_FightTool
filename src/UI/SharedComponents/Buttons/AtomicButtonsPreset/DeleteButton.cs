using MudBlazor;
using MudIcons = MudBlazor.Icons;

namespace DnDFightTool.UI.SharedComponents.Buttons.AtomicButtonsPreset;

public class DeleteButton : ButtonBase
{
    public DeleteButton() : base(Color.Error, MudIcons.Material.Filled.Delete, false)
    {
    }
}
