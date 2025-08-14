using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Characters.Validation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedComponents;

namespace DnDEntitiesBlazorComponents.DnDEntities.Characters.Components;

public partial class CharacterMainInfoEditorComponent : StylableComponentBase
{
    [Inject]
#pragma warning disable 8618
    public CharacterValidator CharacterValidator { private get; set; }
#pragma warning restore 8618
    
    [Parameter, EditorRequired]
    public Character? Character { get; set; }

    private MudForm? _form;

    /// <summary>
    ///     Validates the underlying form.
    /// </summary>
    public async Task<bool> ValidateAsync()
    {
        if (_form is not null)
        {
            await _form.Validate();
            return _form.IsValid;
        }
        
        return false;
    }
}
