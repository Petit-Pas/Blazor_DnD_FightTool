using DnDFightTool.Components.DndUi.Shared.Components;
using DnDFightTool.Components.DndUi.Web.Components;

namespace DnDFightTool.Components.DndUi.Web.Hosting;

/// <summary>
///     Composes the request pipeline of the <c>DndUi.Web</c> host.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    ///     Wires the middleware chain and maps the Blazor Server root component.
    /// </summary>
    /// <param name="app">The application to configure.</param>
    /// <returns>The same <paramref name="app"/> instance, for chaining.</returns>
    public static WebApplication ConfigureWebAppPipeline(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.MapStaticAssets();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddAdditionalAssemblies(typeof(Routes).Assembly);

        return app;
    }
}
