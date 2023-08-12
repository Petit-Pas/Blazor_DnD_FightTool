using DnDFightTool.Domain.DnDEntities.Characters;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DnDEntitiesBlazorComponents.DnDEntities.MartialAttacks;

public partial class MartialAttackTemplatesEditable
{
    [Parameter]
    public Character Character { get; set; }

    private EditContext _editContext { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (Character != null)
        {
            _editContext = new EditContext(Character);

            //_editContext.OnFieldChanged += (sender, args) =>
            //{
            //    StateHasChanged();
            //};
        }
    }

}
