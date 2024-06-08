using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using JsInterop;
using Mapping;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NeoBlazorphic.StyleParameters;

namespace DnDEntitiesBlazorComponents.DnDEntities.MartialAttacks;

public partial class MartialAttackTemplateCollectionEditable
{
    [Inject]
    public IMapper Mapper { get; set; }

    [Inject]
    public IJSRuntime JsRuntime { get; set; }

    [Parameter]
    public Character Character { get; set; }

    private static BorderRadius TemplateBorderRadius = new BorderRadius(2, "em");

    private MartialAttackTemplate? EditedTemplate { get; set; } = null;

    private void CreateNew()
    {
        Character.MartialAttacks.Add(new MartialAttackTemplate());
    }

    private void Delete(MartialAttackTemplate martialAttackTemplate)
    {
        Character.MartialAttacks.Remove(martialAttackTemplate.Id);
    }

    private async Task Edit(MartialAttackTemplate martialAttackTemplate)
    {
        EditedTemplate = Mapper.Copy(martialAttackTemplate);
        await ScrollTo(martialAttackTemplate);
    }

    private async Task ScrollTo(MartialAttackTemplate martialAttackTemplate)
    {
        await JsRuntime.ScrollToTopAsync("attack-template-collection", "attack-template-card-" + Character.MartialAttacks.ToList().FindIndex(x => x.Key == martialAttackTemplate.Id));
        await JsRuntime.LockScroll("attack-template-collection");
    }

    private async Task Duplicate(MartialAttackTemplate martialAttackTemplate)
    {
        Character.MartialAttacks.Add(Mapper.Clone(martialAttackTemplate));
    }

    private async Task CancelEdit()
    {
        await JsRuntime.UnlockScroll("attack-template-collection");
        EditedTemplate = null;
    }

    private async Task SaveEdit()
    {
        await JsRuntime.UnlockScroll("attack-template-collection");
        EditedTemplate = null;
    }

    // TODO recheck every css class and avoid using magic strings when sending to the js interop
}
