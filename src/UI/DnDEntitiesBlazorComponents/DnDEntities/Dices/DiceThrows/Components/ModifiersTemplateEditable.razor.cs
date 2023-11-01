using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using Microsoft.AspNetCore.Components;

namespace DnDEntitiesBlazorComponents.DnDEntities.Dices.DiceThrows.Components;

public partial class ModifiersTemplateEditable
{
    [Parameter, EditorRequired]
    public ModifiersTemplate? Template { get; set; }

    [Parameter]
    public EventCallback<ModifiersTemplate> TemplateChanged { get; set; }

    private async Task OnTemplateChanged(ModifiersTemplate template)
    {
        if (Template != template)
        {
            await TemplateChanged.InvokeAsync(template);
        }
    }
}
