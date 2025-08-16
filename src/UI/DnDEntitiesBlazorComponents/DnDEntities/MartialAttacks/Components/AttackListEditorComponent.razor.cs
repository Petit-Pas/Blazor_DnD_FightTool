using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using Microsoft.AspNetCore.Components;
using Mapping;

namespace DnDEntitiesBlazorComponents.DnDEntities.MartialAttacks.Components;

public partial class AttackListEditorComponent
{
    [Parameter]
    public Character? Character { get; set; }

    [Inject]
    public required IAttackEditContext AttackEditContext { get; set; } = default!;

    [Inject]
    public required IMapper Mapper { get; set; }

    public void Edit(MartialAttackTemplate attack)
    {
        AttackEditContext.EditAttack(Mapper.Copy(attack));
    }

    public void Duplicate(MartialAttackTemplate attack)
    {
        AttackEditContext.EditAttack(Mapper.Clone(attack));
    }

    public void Delete(MartialAttackTemplate attack)
    {
        Character!.MartialAttacks.Remove(attack.Id);
    }

    public void AddNew()
    {
        AttackEditContext.EditAttack(new MartialAttackTemplate());
    }
}
