using MudBlazor;
using SharedComponents.Icons;

namespace SharedComponents.Buttons.AtomicButtonsPreset;

public class FightButton : ButtonBase
{
    public FightButton() : base(Color.Secondary, CustomIcons.SvgRepo.Swords, false)
    {
    }
}
