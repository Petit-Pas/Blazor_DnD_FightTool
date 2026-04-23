using Microsoft.AspNetCore.Components;

namespace DnDFightTool.UI.SharedComponents;

public class StylableComponentBase: ComponentBase
{
    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public string Style { get; set; } = "";
}
