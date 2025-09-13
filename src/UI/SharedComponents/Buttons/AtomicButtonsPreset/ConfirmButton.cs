using MudBlazor;
using SharedComponents.Icons;

namespace SharedComponents.Buttons.AtomicButtonsPreset;

public class ConfirmButton : ButtonBase
{
    public ConfirmButton() : base(Color.Success, CustomIcons.FontAwesome.CircleCheck, false)
    {
    }
}
