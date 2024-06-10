using Microsoft.AspNetCore.Components;

namespace SharedComponents.Inputs.Buttons.Actions;


/// <summary>
///     Container to hold actions for a card, they will appear only when the user hovers over the parent card.
///     
///     Warning: A parent of this component should have a position: relative for the positioning of this element to work correctly.
/// </summary>
public partial class CardActionCollection
{
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    private List<CardActionCollectionElement> Actions { get; set; } = new List<CardActionCollectionElement>(5);

    public void AddAction(CardActionCollectionElement actionButton)
    {
        if (!Actions.Contains(actionButton))
        {
            Actions.Add(actionButton);
            StateHasChanged();
        }
    }
}
