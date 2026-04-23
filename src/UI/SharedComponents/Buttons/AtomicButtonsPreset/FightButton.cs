using MudBlazor;
using DnDFightTool.UI.SharedComponents.Icons;

namespace DnDFightTool.UI.SharedComponents.Buttons.AtomicButtonsPreset;

public class FightButton : ButtonBase
{
    public FightButton() : base(Color.Secondary, CustomIcons.SvgRepo.Swords, false)
    {
    }
}
