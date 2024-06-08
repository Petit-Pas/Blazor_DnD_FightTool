using Microsoft.JSInterop;

namespace JsInterop;

public static class ScrollExtensions
{
    public static async Task ScrollToTopAsync(this IJSRuntime jsRuntime, string scrollableAreaElementId, string htmlElementId)
    {
        await jsRuntime.InvokeVoidAsync("dndJsInterop.scrollToTop", scrollableAreaElementId, htmlElementId);
    }

    public static async Task LockScroll(this IJSRuntime jsRuntime, string scrollableAreaElementId)
    {
        await jsRuntime.InvokeVoidAsync("dndJsInterop.lockScroll", scrollableAreaElementId);
    }

    public static async Task UnlockScroll(this IJSRuntime jsRuntime, string scrollableAreaElementId)
    {
        await jsRuntime.InvokeVoidAsync("dndJsInterop.unlockScroll", scrollableAreaElementId);
    }
}
