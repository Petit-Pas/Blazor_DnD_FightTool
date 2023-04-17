using Fight;
using Microsoft.AspNetCore.Components;

namespace FightBlazorComponents.Pages;

public partial class FightPage
{
    [Inject]
    public IFightContext FightContext { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }
}
