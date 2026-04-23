using DnDFightTool.Domain.CharacterSheet.Damage;
using Microsoft.AspNetCore.Components;

namespace CharacterSheetBlazorComponents.Damage.Components;

public partial class DamageRollTemplateCollectionEditorComponent
{
    [Parameter]
    public DamageRollTemplateCollection? DamageRollTemplates { get; set; }

    public Dictionary<DamageRollTemplate, DamageRollTemplateEditorComponent> DamageTemplateComponentReferences { get; set; } = [];

    public bool Validate()
    {
        return DamageTemplateComponentReferences.Values.All(x => x.Validate());
    }

    public void AddNew()
    {
        DamageRollTemplates?.Add(new DamageRollTemplate());
    }

    public void Remove(DamageRollTemplate damageTemplate)
    {
        DamageRollTemplates?.Remove(damageTemplate);
    }
}
