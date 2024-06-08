using Microsoft.AspNetCore.Components;

namespace SharedComponents.Inputs.Buttons.Actions;

public class CardActionCollectionElement : ComponentBase
{
    private RenderFragment? _childContent;
    private CardActionCollection? _parent;

    [Parameter]
    public RenderFragment? ChildContent { get => _childContent; set { _childContent = value; SendToParent(); } }

    [CascadingParameter]
    public CardActionCollection? Parent { get => _parent; set { _parent = value; SendToParent(); } }

    private void SendToParent()
    {
        if (Parent != null && ChildContent != null)
        {
            Parent.AddAction(this);
        }
    }
}
