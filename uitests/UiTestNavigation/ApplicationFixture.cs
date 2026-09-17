using DnDFightTool.Components.DndUi.Web.Hosting;
using DnDFightTool.Components.DndUi.Web.IoC;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.TurnTracking;
using DnDFightTool.Domain.Logs;
using DnDFightTool.UiTests.UiTestNavigation.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using NUnit.Framework;
using UndoableMediator.Mediators;

namespace DnDFightTool.UiTests.UiTestNavigation;

/// <summary>
///     Shared plumbing for UI scenarios: a real web host, one Chromium page, screenshot capture and application-state
///     reset helpers. <b>Abstract on purpose — never derive a scenario from this directly.</b> Use
///     <see cref="IsolatedScenarioFixture"/> for independent scenarios, or <see cref="SequentialScenarioFixture"/> for an
///     ordered chain of steps that share state. This type declares no <c>[SetUp]</c>/<c>[TearDown]</c>; the derived
///     fixtures own the lifecycle and decide when to call the helpers here.
/// </summary>
public abstract class ApplicationFixture
{
    // These resources are shared across the whole assembly and are disposed in AssemblyFixture's [OneTimeTearDown]
    // (via StopApplicationAsync), not in the derived fixtures' per-scenario teardown — disposing them per scenario would
    // kill the host after the first test. Suppress NUnit1032, which cannot see the OneTimeTearDown ownership.
#pragma warning disable NUnit1032
    private static WebApplication? _application;
    private static IPlaywright? _playwright;
    private static IBrowser? _browser;
    private static IPage? _page;
#pragma warning restore NUnit1032
    private static string? _dataFolder;

    // Per-scenario screenshot step index. Reset by the derived fixture's [SetUp] via PrepareScenarioArtifacts, so each
    // scenario (or ordered step) starts a fresh zero-based sequence even though the fixture instance may be reused.
    private int _captureIndex;

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
    ///     Gets the running host's <see cref="IServiceProvider"/> so scenarios and teardown can reach the host singletons
    ///     (<see cref="IUndoableMediator"/>, <see cref="IFightContext"/>, <see cref="ICharacterRepository"/>, and siblings).
    /// </summary>
    protected IServiceProvider Services
    {
        get
        {
            return _application?.Services
                ?? throw new InvalidOperationException("The UI test application fixture has not started.");
        }
    }

    /// <summary>
    ///     Clears the running scenario's artifacts folder and resets the screenshot step index, so a re-run starts from an
    ///     empty folder and numbering restarts at zero. Derived fixtures call this from their <c>[SetUp]</c>.
    /// </summary>
    protected void PrepareScenarioArtifacts()
    {
        var folder = GetScenarioFolder();
        if (Directory.Exists(folder))
        {
            Directory.Delete(folder, recursive: true);
        }

        Directory.CreateDirectory(folder);
        _captureIndex = 0;
    }

    /// <summary>
    ///     Captures the scenario's end-of-run state as the <c>final</c> image, swallowing any failure. It is best-effort by
    ///     design: a screenshot problem must never mask the scenario's own result nor block the state reset that keeps
    ///     scenarios independent. Derived fixtures call this first in their teardown, before any state is undone.
    /// </summary>
    protected async Task CaptureFinalStateAsync()
    {
        try
        {
            await CaptureAsync("final");
        }
        catch (Exception)
        {
        }
    }

    /// <summary>
    ///     Resets the application to a clean slate: undoes every command while the history is non-empty, clears the
    ///     (hide-based) log, and deletes leftover characters through the repository business API. Asserts the command-driven
    ///     state and the repository are genuinely empty afterwards, turning cleanup into continuous proof that undo restores
    ///     everything.
    /// </summary>
    protected async Task ResetApplicationStateAsync()
    {
        var mediator = Services.GetRequiredService<IUndoableMediator>();
        // Stop if an undo reports failure rather than looping forever on a command whose undo cannot decrement history.
        while (mediator.HistoryLength > 0 && await mediator.UndoLastCommandAsync())
        {
        }

        AssertCommandDrivenStateIsEmpty();

        // Log undo hides entries rather than removing them, so undo alone cannot empty Blocks. Clear it now so the next
        // scenario starts from a genuinely empty log. (The redo stack still holds the undone commands; the next command
        // clears it — "clean state" and "empty redo stack" are mutually exclusive through the mediator API.)
        var logService = Services.GetRequiredService<IDnDLogService>();
        logService.Clear();
        logService.Blocks.Should().BeEmpty();

        var characterRepository = Services.GetRequiredService<ICharacterRepository>();
        foreach (var character in characterRepository.GetAllCharacters().ToArray())
        {
            characterRepository.Delete(character);
        }

        characterRepository.Count.Should().Be(0);
    }

    /// <summary>
    ///     Captures a full-page screenshot of the current <see cref="Page"/> into the running scenario's artifacts
    ///     subfolder and returns the written file path. Files are named <c>NN-{name}.png</c>, where <c>NN</c> is a
    ///     zero-padded per-scenario step index that increments on every capture (named or automatic). The subfolder is
    ///     emptied by the fixture's <c>[SetUp]</c> (<see cref="PrepareScenarioArtifacts"/>), so a re-run never mixes stale
    ///     step files with new ones.
    /// </summary>
    /// <param name="name">The step name embedded in the file name; sanitized to filesystem-safe characters.</param>
    /// <returns>The absolute path of the PNG that was written.</returns>
    protected virtual async Task<string> CaptureAsync(string name)
    {
        var folder = GetScenarioFolder();
        Directory.CreateDirectory(folder);

        var path = Path.Combine(folder, $"{_captureIndex:D2}-{name.ToFileSafeName()}.png");
        _captureIndex++;

        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = path,
            FullPage = true
        });

        return path;
    }

    /// <summary>
    ///     Resolves the artifacts subfolder for the currently running scenario, mirroring the namespace tree below this
    ///     fixture's namespace then the test name (e.g. <c>{ArtifactsRoot}/Meta/ScreenshotEvidenceTests/{TestName}</c>). This
    ///     matches how the test explorer groups scenarios. Exposed to scenarios so a test can plant or inspect its own files.
    /// </summary>
    /// <returns>The absolute path of the running scenario's artifacts subfolder.</returns>
    protected static string GetScenarioFolder()
    {
        var test = TestContext.CurrentContext.Test;
        var className = test.ClassName ?? "UnknownFixture";

        // Drop the project-level namespace prefix (redundant with the per-project artifacts root) so scenarios sit under
        // their sub-namespace (e.g. "Meta") exactly as the test tree shows them.
        var rootNamespace = typeof(ApplicationFixture).Namespace ?? string.Empty;
        var relativeClassName = className.StartsWith(rootNamespace + ".", StringComparison.Ordinal)
            ? className.Substring(rootNamespace.Length + 1)
            : className;

        var segments = relativeClassName.Split('.', StringSplitOptions.RemoveEmptyEntries)
            .Append(test.Name)
            .Select(segment => segment.ToFileSafeName())
            .ToArray();

        return Path.Combine([GetArtifactsRoot(), .. segments]);
    }

    private static string GetArtifactsRoot()
    {
        // Resolve per running test project so each project keeps its own evidence; never the process working directory.
        var projectDirectory = new DirectoryInfo(AppContext.BaseDirectory).FindAncestorContaining("*.csproj");
        return Path.Combine(projectDirectory.FullName, "artifacts");
    }

    /// <summary>
    ///     Asserts the command-driven state is genuinely empty after a full undo: no fighters, no visible log entries, no
    ///     combat in progress and no applied statuses. This assertion is what turns teardown into continuous proof that undo
    ///     restores everything — a command whose undo is incomplete leaves residue here and fails the scenario that caused it.
    /// </summary>
    protected void AssertCommandDrivenStateIsEmpty()
    {
        Services.GetRequiredService<IUndoableMediator>().HistoryLength.Should().Be(0);
        Services.GetRequiredService<IFightContext>().Fighters.Should().BeEmpty();

        var logService = Services.GetRequiredService<IDnDLogService>();
        logService.Blocks
            .SelectMany(block => block.Entries)
            .Where(entry => !logService.IsHidden(entry.Id))
            .Should().BeEmpty();

        var turnService = Services.GetRequiredService<ICombatTurnService>();
        turnService.IsStarted.Should().BeFalse();
        turnService.TurnOrder.Should().BeEmpty();

        Services.GetRequiredService<IAppliedStatusRepository>().AppliedStatuses.Should().BeEmpty();
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
            // ApplicationName and EnvironmentName must be set through WebApplicationOptions, not mutated after
            // CreateBuilder: the development static-web-assets loader reads them while the host is being built, and it
            // resolves DndUi.Web's asset manifest by ApplicationName. Setting them afterwards leaves the loader keyed to
            // the test runner's assembly, so MapStaticAssets matches the routes but serves empty (0-byte) CSS.
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                ApplicationName = typeof(ServiceCollectionExtensions).Assembly.GetName().Name,
                EnvironmentName = "Development"
            });
            builder.WebHost.UseUrls("https://127.0.0.1:0");
            builder.Services.RegisterWebAppServices(_dataFolder, commandHistoryMaxSize: 1000);

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
