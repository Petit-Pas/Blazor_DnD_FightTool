using DnDFightTool.Components.DndUi.Web.Hosting;
using DnDFightTool.Components.DndUi.Web.IoC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Playwright;

namespace DnDFightTool.UiTests.UiTestNavigation;

/// <summary>
///     Provides a real web host and browser page to UI scenarios.
/// </summary>
public abstract class ApplicationFixture
{
    private static WebApplication? _application;
    private static IPlaywright? _playwright;
    private static IBrowser? _browser;
    private static IPage? _page;
    private static string? _dataFolder;

    /// <summary>
    ///     Gets the browser page used by the scenario.
    /// </summary>
    protected IPage Page
    {
        get
        {
            return _page ?? throw new InvalidOperationException("The UI test application fixture has not started.");
        }
    }

    /// <summary>
    ///     Gets the dynamically assigned address of the web host.
    /// </summary>
    protected string BaseUrl
    {
        get
        {
            return _application?.Urls.Single()
                ?? throw new InvalidOperationException("The UI test application fixture has not started.");
        }
    }

    /// <summary>
    ///     Starts the real web host and one Chromium browser for the test fixture.
    /// </summary>
    public static async Task StartApplicationAsync()
    {
        if (_application is not null)
        {
            throw new InvalidOperationException("The UI test application fixture has already started.");
        }

        _dataFolder = Path.Combine(Path.GetTempPath(), $"DnDFightTool.UiTests.{Guid.NewGuid():N}");

        try
        {
            var builder = WebApplication.CreateBuilder();
            builder.Environment.EnvironmentName = "Development";
            builder.Environment.ApplicationName = typeof(ServiceCollectionExtensions).Assembly.GetName().Name!;
            builder.WebHost.UseUrls("https://127.0.0.1:0");
            builder.Services.RegisterWebAppServices(_dataFolder);

            _application = builder.Build();
            _application.ConfigureWebAppPipeline();
            await _application.StartAsync();

            _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });
            _page = await _browser.NewPageAsync(new BrowserNewPageOptions
            {
                IgnoreHTTPSErrors = true
            });
        }
        catch
        {
            await DisposeResourcesAsync();
            throw;
        }
    }

    /// <summary>
    ///     Stops the web host and disposes the browser resources after the fixture completes.
    /// </summary>
    public static async Task StopApplicationAsync()
    {
        await DisposeResourcesAsync();
    }

    private static async Task DisposeResourcesAsync()
    {
        try
        {
            if (_page is not null)
            {
                await _page.CloseAsync();
            }

            if (_browser is not null)
            {
                await _browser.CloseAsync();
            }

            _playwright?.Dispose();

            if (_application is not null)
            {
                await _application.StopAsync();
                await _application.DisposeAsync();
            }
        }
        finally
        {
            _page = null;
            _browser = null;
            _playwright = null;
            _application = null;

            if (_dataFolder is not null)
            {
                try
                {
                    if (Directory.Exists(_dataFolder))
                    {
                        Directory.Delete(_dataFolder, recursive: true);
                    }
                }
                catch (IOException)
                {
                }

                _dataFolder = null;
            }
        }
    }
}
