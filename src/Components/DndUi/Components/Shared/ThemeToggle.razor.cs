using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DndUi.Components.Shared
{
    public partial class ThemeToggle : ComponentBase
    {
        [Parameter]
        public bool IsDarkMode { get; set; } = true;

        [Parameter]
        public MudThemeProvider? MudThemeProvider { get; set; }

        private async void ToggleTheme()
        {
            if (MudThemeProvider is not null)
            {
                IsDarkMode = !IsDarkMode;
                await MudThemeProvider.IsDarkModeChanged.InvokeAsync(IsDarkMode);
            }
        }
    }
}
