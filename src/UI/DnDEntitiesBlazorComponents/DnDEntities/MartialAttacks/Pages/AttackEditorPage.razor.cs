using Microsoft.AspNetCore.Components;

namespace DnDEntitiesBlazorComponents.DnDEntities.MartialAttacks.Pages;

public partial class AttackEditorPage
{
    [Inject]
    public required IAttackEditContext AttackEditContext { get; set; }

    public void Save()
    {
        AttackEditContext.SaveEditedAttack();
    }
}
