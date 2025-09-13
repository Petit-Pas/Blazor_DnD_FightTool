using MudBlazor;
using SharedComponents.Icons;

namespace SharedComponents.Buttons.AtomicButtonsPreset;

public class RollButton : ButtonBase
{
    public RollButton() : base(Color.Secondary, CustomIcons.FontAwesome.RollingDice, false)
    {
    }
}
