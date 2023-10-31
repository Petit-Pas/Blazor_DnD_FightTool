using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using Microsoft.AspNetCore.Components;

namespace DnDEntitiesBlazorComponents.DnDEntities.Dices.DiceThrows.Components;

public partial class DiceThrowTemplateEditable
{
    [Parameter]
    public DiceThrowTemplate Template { get; set; }

    [Parameter]
    public EventCallback<DiceThrowTemplate> TemplateChanged {get;set;}

    private async Task OnTemplateChanged(DiceThrowTemplate template)
    {
        if (Template != template)
        {
            await TemplateChanged.InvokeAsync(template);
        }
    }
}
