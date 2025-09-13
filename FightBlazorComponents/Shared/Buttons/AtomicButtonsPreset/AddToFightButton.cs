using MudBlazor;
using SharedComponents.Buttons.AtomicButtonsPreset;
using SharedComponents.Icons;

namespace FightBlazorComponents.Shared.Buttons.AtomicButtonsPreset;

public class AddToFightButton : ButtonBase
{
    public AddToFightButton() : base(Color.Secondary, CustomIcons.SvgRepo.Swords, false)
    {
    }
}
