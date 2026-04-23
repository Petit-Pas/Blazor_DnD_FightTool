using MudBlazor;
using DnDFightTool.UI.SharedComponents.Icons;

namespace DnDFightTool.UI.SharedComponents.Buttons.AtomicButtonsPreset;

public class ConfirmButton : ButtonBase
{
    public ConfirmButton() : base(Color.Success, CustomIcons.FontAwesome.CircleCheck, false)
    {
    }
}
