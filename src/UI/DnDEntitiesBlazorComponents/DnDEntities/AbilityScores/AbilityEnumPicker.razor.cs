using DnDFightTool.Domain.DnDEntities.AbilityScores;
using Microsoft.AspNetCore.Components;

namespace DnDEntitiesBlazorComponents.DnDEntities.AbilityScores;

public partial class AbilityEnumPicker
{
    /// <summary>
    ///     Property to bind to for the ability
    /// </summary>
    [Parameter, EditorRequired]
    public AbilityEnum Ability { get; set; }

    /// <summary>
    ///     Event used for the Ability propertyBinding
    /// </summary>
    [Parameter]
    public virtual EventCallback<AbilityEnum> AbilityChanged { get; set; }

    private async Task OnValueChanged(AbilityEnum ability)
    {
        if (ability != Ability)
        {
            await AbilityChanged.InvokeAsync(ability);
        }
    }
}
