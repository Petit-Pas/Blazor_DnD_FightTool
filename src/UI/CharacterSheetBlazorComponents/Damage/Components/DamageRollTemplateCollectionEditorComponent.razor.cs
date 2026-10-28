using DnDFightTool.Domain.CharacterSheet.Damage;
using Microsoft.AspNetCore.Components;

namespace DnDFightTool.UI.CharacterSheetBlazorComponents.Damage.Components;

public partial class DamageRollTemplateCollectionEditorComponent
{
    [Parameter]
    public DamageRollTemplateCollection? DamageRollTemplates { get; set; }

    /// <summary>
    ///     Row components by template, so <see cref="Validate"/> can ask each row: invalid dice text never reaches the model.
    /// </summary>
    public Dictionary<DamageRollTemplate, DamageRollTemplateEditorComponent> DamageTemplateComponentReferences { get; set; } = [];

    private DamageRollTemplateCollection? _boundTemplates;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!ReferenceEquals(DamageRollTemplates, _boundTemplates))
        {
            _boundTemplates = DamageRollTemplates;
            DamageTemplateComponentReferences.Clear();
        }
    }

    public bool Validate()
    {
        // Only rows still in the collection count; a ref can outlive its row if the list was changed elsewhere.
        return DamageRollTemplates is null
            || DamageRollTemplates.All(t => !DamageTemplateComponentReferences.TryGetValue(t, out var row) || row.Validate());
    }

    public void AddNew()
    {
        DamageRollTemplates?.Add(new DamageRollTemplate());
    }

    public void Remove(DamageRollTemplate damageTemplate)
    {
        DamageRollTemplates?.Remove(damageTemplate);
        DamageTemplateComponentReferences.Remove(damageTemplate);
    }
}
