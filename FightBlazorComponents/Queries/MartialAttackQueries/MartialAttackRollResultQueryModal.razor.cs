using Fight;
using Fight.MartialAttacks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FightBlazorComponents.Queries.MartialAttackQueries;

public partial class MartialAttackRollResultQueryModal
{
    [Inject]
    public IFightContext FightContext { get; set; }

    [Parameter]
    public MartialAttackRollResult MartialAttackRollResult { get; set; }

    private EditContext _editContext;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (MartialAttackRollResult != null && _editContext == null)
        {
            _editContext = new EditContext(MartialAttackRollResult);
            StateHasChanged();
        }
    }
}
