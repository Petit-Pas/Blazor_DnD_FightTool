using MudBlazor;
using MudIcons = MudBlazor.Icons;

namespace DnDFightTool.UI.SharedComponents.Buttons.AtomicButtonsPreset;

public class UndoButton : ButtonBase
{
    public UndoButton() : base(Color.Default, MudIcons.Material.Filled.Undo, false)
    {
    }
}
