using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.MartialAttacks;
using Microsoft.AspNetCore.Components;
using DnDFightTool.Infrastructure.Mapping;

namespace DnDFightTool.UI.CharacterSheetBlazorComponents.MartialAttacks.Components;

public partial class AttackListEditorComponent
{
    [Parameter]
    public ICharacter? Character { get; set; }

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
