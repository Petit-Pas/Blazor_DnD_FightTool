using Blazored.Modal;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Saves;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FightBlazorComponents.Queries.SaveQueries;

public partial class SaveRollResultQueryModal
{
    [CascadingParameter]
    BlazoredModalInstance BlazoredModal { get; set; } = default!;

    [Parameter]
    public Character? Caster { get; set; }

    [Parameter]
    public Character? Target { get; set; }

    [Parameter]
    public SaveRollResult? SaveRollResult { get; set; }

    private EditContext? _editContext;

    private bool _allParametersAreValid => Caster != null && Target != null && SaveRollResult != null;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (_allParametersAreValid) 
        {
            _editContext = new EditContext(SaveRollResult!);
            StateHasChanged();
        }
    }

    private bool IsModelValid()
    {
        if (_editContext != null)
        {
            return _editContext.Validate();
        }
        return false;
    }
}
