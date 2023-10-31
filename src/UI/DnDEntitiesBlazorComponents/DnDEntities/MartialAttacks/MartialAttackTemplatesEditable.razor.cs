using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Damage;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using DnDFightTool.Domain.DnDEntities.Statuses;
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


    private void RemoveDamageFromAttack(MartialAttackTemplate attack, DamageRollTemplate damage)
    {
        attack.Damages.Remove(damage);
    }

    private void AddNewDamageInAttack(MartialAttackTemplate attack)
    {
        attack.Damages.Add(new DamageRollTemplate());
    }

    private void RemoveStatusFromAttack(MartialAttackTemplate attack, StatusTemplate status)
    {
        attack.Statuses.Remove(status);
    }

    private void AddNewStatusInAttack(MartialAttackTemplate attack)
    {
        attack.Statuses.Add(new StatusTemplate());
    }

}
