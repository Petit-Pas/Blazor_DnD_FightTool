using MudBlazor;
using DnDFightTool.UI.SharedComponents.Icons;

namespace DnDFightTool.UI.SharedComponents.Buttons.AtomicButtonsPreset;

public class RollButton : ButtonBase
{
    public RollButton() : base(Color.Secondary, CustomIcons.FontAwesome.RollingDice, false)
    {
    }
}
