using Microsoft.JSInterop;

namespace JavascriptInteropExtensions;

public static class ScrollExtensions
{
    public async static Task ScrollToTopAsync(this IJSRuntime jsRuntime, string scrollableAreaElementId, string htmlElementId)
    {
        await jsRuntime.InvokeVoidAsync("dndJsInterop.scrollToTop", scrollableAreaElementId, htmlElementId);
    }

    public async static Task LockScroll(this IJSRuntime jsRuntime, string scrollableAreaElementId)
    {
        await jsRuntime.InvokeVoidAsync("dndJsInterop.lockScroll", scrollableAreaElementId);
    }

    public async static Task UnlockScroll(this IJSRuntime jsRuntime, string scrollableAreaElementId)
    {
        await jsRuntime.InvokeVoidAsync("dndJsInterop.unlockScroll", scrollableAreaElementId);
    }
}
