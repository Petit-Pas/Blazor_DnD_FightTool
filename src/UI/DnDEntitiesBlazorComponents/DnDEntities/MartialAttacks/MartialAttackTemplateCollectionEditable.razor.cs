using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using JavascriptInteropExtensions;
using Mapping;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using NeoBlazorphic.StyleParameters;

namespace DnDEntitiesBlazorComponents.DnDEntities.MartialAttacks;

public partial class MartialAttackTemplateCollectionEditable
{
    [Inject]
    public required IMapper Mapper { get; set; }

    [Inject]
    public required IJSRuntime JsRuntime { get; set; }

    [Parameter]
    public required Character Character { get; set; }

    private readonly static BorderRadius _templateBorderRadius = new(2, "em");

    private EditContext? EditContext { get; set; } = null;
    
    /// <summary>
    ///     When we want to scroll to the edited element, we need to wait for the page to first refresh.
    ///     Since otherwise the element doesn't have its definitive size yet, which is an issue when it's at the bottom of the parent div.
    /// </summary>
    private bool WaitingForOpeningOfEdit { get; set; } = false;

    /// <summary>
    ///     When we are editing a new (or duplicated) element, we should know it so that we can get rid of it when the modification is cancelled.
    /// </summary>
    private bool EditedElementIsANewOne { get; set; } = false;

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (WaitingForOpeningOfEdit)
        {
            WaitingForOpeningOfEdit = false;
            await ScrollTo((MartialAttackTemplate)EditContext!.Model);
        }
    }

    private void CreateNew()
    {
        var newTemplate = new MartialAttackTemplate();
        Character.MartialAttacks.Add(newTemplate);
        EditedElementIsANewOne = true;
        Edit(newTemplate);
    }

    private void Delete(MartialAttackTemplate martialAttackTemplate)
    {
        Character.MartialAttacks.Remove(martialAttackTemplate.Id);
    }

    private void Edit(MartialAttackTemplate martialAttackTemplate)
    {
        var editedTemplate = Mapper.Copy(martialAttackTemplate);
        EditContext = new EditContext(editedTemplate);
        WaitingForOpeningOfEdit = true;

        EditContext.OnValidationStateChanged += ValidationChanged;

        StateHasChanged();
    }

    private void ValidationChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        StateHasChanged();
    }

    private async Task ScrollTo(MartialAttackTemplate martialAttackTemplate)
    {
        await JsRuntime.ScrollToTopAsync("attack-template-collection", "attack-template-card-" + Character.MartialAttacks.ToList().FindIndex(x => x.Key == martialAttackTemplate.Id));
        await JsRuntime.LockScroll("attack-template-collection");
    }

    private void Duplicate(MartialAttackTemplate martialAttackTemplate)
    {
        var newTemplate = Mapper.Clone(martialAttackTemplate);
        Character.MartialAttacks.Add(newTemplate);
        EditedElementIsANewOne = true;
        Edit(newTemplate);
    }

    private async Task CancelEdit()
    {
        if (EditContext == null)
        {
            return;
        }
        
        await JsRuntime.UnlockScroll("attack-template-collection");
        if (EditedElementIsANewOne)
        {
            Delete((MartialAttackTemplate)EditContext.Model);
            EditedElementIsANewOne = false;
        }
        EditContext.OnValidationStateChanged -= ValidationChanged;
        EditContext = null;
    }

    private async Task SaveEdit()
    {
        if (EditContext == null)
        {
            return;
        }
        if (!EditContext.Validate())
        {
            return;
        }

        await JsRuntime.UnlockScroll("attack-template-collection");

        var editedTemplate = (MartialAttackTemplate)EditContext.Model;
        Character.MartialAttacks[editedTemplate.Id] = editedTemplate;

        EditContext.OnValidationStateChanged -= ValidationChanged;
        EditContext = null;
        EditedElementIsANewOne = false;
    }

    // TODO recheck every css class and avoid using magic strings when sending to the js interop
}
