using DnDFightTool.Domain.Logs;
using DnDFightTool.Infrastructure.Extensions;
using DnDFightTool.UI.SharedComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DnDFightTool.UI.FightBlazorComponents.Log;

/// <summary>
///     Main log panel component. Renders all visible log blocks in a scrollable container.
///     Subscribes to <see cref="IDnDLogService.OnChanged"/> for live updates.
/// </summary>
public partial class CombatLogPanel : StylableComponentBase, IDisposable
{
    [Inject]
    private IDnDLogService LogService { get; set; } = null!;

    [Inject]
    private IJSRuntime JS { get; set; } = null!;

    [CascadingParameter(Name = "IsDarkMode")]
    private bool IsDarkMode { get; set; }

    private ElementReference _scrollSentinel;

    /// <summary>
    ///     Set to <c>true</c> after the first render pass, once <see cref="_scrollSentinel"/> is assigned
    ///     by Blazor and the JS runtime is available for DOM calls. Guards <see cref="HandleLogChanged"/>
    ///     against JS interop calls before the component is mounted.
    /// </summary>
    private bool _hasRendered;

    /// <summary>
    ///     Set to <c>true</c> in <see cref="Dispose"/> so that any queued <see cref="HandleLogChanged"/>
    ///     continuation that executes after disposal does not attempt to call into a disposed component
    ///     or a torn-down <see cref="IJSRuntime"/>.
    /// </summary>
    private bool _disposed;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        LogService.OnChanged += HandleLogChanged;
    }

    /// <inheritdoc />
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // _scrollSentinel is now bound to the DOM and the JS runtime is ready for interop.
            _hasRendered = true;
        }

        return base.OnAfterRenderAsync(firstRender);
    }

    private void HandleLogChanged()
    {
        if (_disposed)
        {
            return;
        }

        InvokeAsync(async () =>
        {
            if (_disposed)
            {
                return;
            }

            StateHasChanged();

            if (_hasRendered)
            {
                await Task.Yield();
                await JS.InvokeVoidAsync("dndJsInterop.scrollIntoView", _scrollSentinel);
            }
        });
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _disposed = true;
        GC.SuppressFinalize(this);
        LogService.OnChanged -= HandleLogChanged;
    }
}
