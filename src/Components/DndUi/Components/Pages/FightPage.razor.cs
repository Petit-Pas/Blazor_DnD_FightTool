using AspNetCoreExtensions.Navigations;
using Microsoft.AspNetCore.Components;

namespace DndUi.Components.Pages;

public partial class FightPage
{
    [Inject]
    public required IStateFullNavigation Navigation { get; set; }
    
    private void GoBack()
    {
        Navigation.NavigateBack();
    }
}
