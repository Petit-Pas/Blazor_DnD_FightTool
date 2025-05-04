using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using DnDFightTool.Domain.DnDEntities.Statuses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DnDEntitiesBlazorComponents.DnDEntities.MartialAttacks;

public partial class MartialAttackTemplateEditable
{
    [Parameter]
    public EditContext? EditContext { get; set; }

    private MartialAttackTemplate? Template => EditContext is null ? null : EditContext.Model as MartialAttackTemplate ?? throw new ArgumentNullException("EditContext", $"The EditContext for MartialAttackTemplateEditable should have a model of type {typeof(MartialAttackTemplate)}");
}
