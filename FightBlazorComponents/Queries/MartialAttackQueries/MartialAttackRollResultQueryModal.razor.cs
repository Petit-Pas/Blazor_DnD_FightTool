using Blazored.Modal.Services;
using Blazored.Modal;
using DnDEntities.Characters;
using Fight;
using Fight.MartialAttacks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FightBlazorComponents.Queries.MartialAttackQueries;

public partial class MartialAttackRollResultQueryModal : ComponentBase
{
    [Inject]
    public IFightContext FightContext { get; set; }

    [Parameter]
    public MartialAttackRollResult MartialAttackRollResult { get; set; }

    [CascadingParameter] 
    BlazoredModalInstance BlazoredModal { get; set; } = default!;

    [Parameter]
    public Character Caster { get; set; }

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

    private async Task Validate()
    {
        var validationSuccessful = _editContext.Validate();
        if (!validationSuccessful)
        {
            return;
        }
        await BlazoredModal.CloseAsync();
    }

    private async Task Cancel()
    {
        await BlazoredModal.CancelAsync();
    }
}
