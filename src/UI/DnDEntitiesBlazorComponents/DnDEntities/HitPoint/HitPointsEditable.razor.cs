using DnDEntities.Characters;
using DnDEntities.HitPoint;
using Microsoft.AspNetCore.Components;

namespace DnDEntitiesBlazorComponents.DnDEntities.HitPoint;

public partial class HitPointsEditable : ComponentBase
{
    [Parameter, EditorRequired]
    public HitPoints? HitPoints { get; set; }
}