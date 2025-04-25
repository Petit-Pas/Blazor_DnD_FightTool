using Blazored.Modal;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FightBlazorComponents.Queries.MartialAttackQueries;

public partial class MartialAttackRollResultQueryModal : ComponentBase
{
    [Parameter]
    public required MartialAttackRollResult MartialAttackRollResult { get; set; }

    [CascadingParameter] 
    private BlazoredModalInstance BlazoredModal { get; set; } = default!;

    [Parameter]
    public required Character Caster { get; set; }

    private EditContext? _editContext;

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
        if (_editContext == null)
        {
            return;
        }
        
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
