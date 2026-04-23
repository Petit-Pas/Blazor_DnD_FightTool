using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Characters.Validation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using DnDFightTool.UI.SharedComponents;

namespace DnDFightTool.UI.CharacterSheetBlazorComponents.Characters.Components;

public partial class CharacterMainInfoEditorComponent : StylableComponentBase
{
    [Inject]
    private CharacterValidator _characterValidator { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ICharacter? Character { get; set; }

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
