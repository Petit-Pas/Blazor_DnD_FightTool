using DnDFightTool.Domain.Fight;
using Microsoft.AspNetCore.Components;

namespace DndUi.Components.Pages;

public partial class FightPage
{
    [Inject]
    public required IFightContext FightContext { get; set; }
}
