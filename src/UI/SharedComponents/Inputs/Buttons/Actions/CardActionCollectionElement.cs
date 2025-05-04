using Microsoft.AspNetCore.Components;

namespace SharedComponents.Inputs.Buttons.Actions;

public class CardActionCollectionElement : ComponentBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [CascadingParameter]
    public CardActionCollection? Parent { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        SendToParent();
    }

    private void SendToParent()
    {
        if (Parent != null && ChildContent != null)
        {
            Parent.AddAction(this);
        }
    }
}
