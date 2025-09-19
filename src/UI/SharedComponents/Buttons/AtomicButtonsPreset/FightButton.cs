using MudBlazor;
using SharedComponents.Buttons.AtomicButtonsPreset;
using SharedComponents.Icons;

namespace FightBlazorComponents.Shared.Buttons.AtomicButtonsPreset;

public class FightButton : ButtonBase
{
    public FightButton() : base(Color.Secondary, CustomIcons.SvgRepo.Swords, false)
    {
    }
}
