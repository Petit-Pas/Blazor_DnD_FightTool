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
    
    /// <summary>
    ///     When we want to scroll to the edited element, we need to wait for the page to first refresh.
    ///     Since otherwise the element doesn't have its definitive size yet, which is an issue when it's at the bottom of the parent div.
    /// </summary>
    private bool WaitingForOpeningOfEdit { get; set; } = false;

    /// <summary>
    ///     When we are editing a new (or duplicated) element, we should know it so that we can get rid of it when the modification is cancelled.
    /// </summary>
    private bool EditedElementIsANewOne { get; set; } = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (WaitingForOpeningOfEdit)
        {
            WaitingForOpeningOfEdit = false;
            await ScrollTo(EditedTemplate);
        }
    }

    private async Task CreateNewAsync()
    {
        var newTemplate = new MartialAttackTemplate();
        Character.MartialAttacks.Add(newTemplate);
        EditedElementIsANewOne = true;
        await Edit(newTemplate);
    }

    private void Delete(MartialAttackTemplate martialAttackTemplate)
    {
        Character.MartialAttacks.Remove(martialAttackTemplate.Id);
    }

    private async Task Edit(MartialAttackTemplate martialAttackTemplate)
    {
        EditedTemplate = Mapper.Copy(martialAttackTemplate);
        WaitingForOpeningOfEdit = true;
        StateHasChanged();
    }

    private async Task ScrollTo(MartialAttackTemplate martialAttackTemplate)
    {
        await JsRuntime.ScrollToTopAsync("attack-template-collection", "attack-template-card-" + Character.MartialAttacks.ToList().FindIndex(x => x.Key == martialAttackTemplate.Id));
        await JsRuntime.LockScroll("attack-template-collection");
    }

    private async Task Duplicate(MartialAttackTemplate martialAttackTemplate)
    {
        var newTemplate = Mapper.Clone(martialAttackTemplate);
        Character.MartialAttacks.Add(newTemplate);
        EditedElementIsANewOne = true;
        await Edit(newTemplate);
    }

    private async Task CancelEdit()
    {
        await JsRuntime.UnlockScroll("attack-template-collection");
        if (EditedElementIsANewOne)
        {
            Delete(EditedTemplate);
            EditedElementIsANewOne = false;
        }
        EditedTemplate = null;
    }

    private async Task SaveEdit()
    {
        await JsRuntime.UnlockScroll("attack-template-collection");
        EditedTemplate = null;
        EditedElementIsANewOne = false;
    }

    // TODO recheck every css class and avoid using magic strings when sending to the js interop
}
