using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using DnDFightTool.Domain.DnDEntities.Statuses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DnDEntitiesBlazorComponents.DnDEntities.MartialAttacks;

public partial class MartialAttackTemplatesEditable
{
    [Parameter]
    public required Character Character { get; set; }

    private EditContext? _editContext { get; set; }

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

    private static void RemoveStatusFromAttack(MartialAttackTemplate attack, StatusTemplate status)
    {
        attack.Statuses.Remove(status.Id);
    }

    private static void AddNewStatusInAttack(MartialAttackTemplate attack)
    {
        attack.Statuses.Add(new StatusTemplate());
    }
}
