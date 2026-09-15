using DnDFightTool.Components.DndUi.Web.Hosting;
using DnDFightTool.Components.DndUi.Web.IoC;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.TurnTracking;
using DnDFightTool.Domain.Logs;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using NUnit.Framework;
using UndoableMediator.Mediators;

namespace DnDFightTool.UiTests.UiTestNavigation;

/// <summary>
///     Provides a real web host and browser page to UI scenarios.
/// </summary>
public abstract class ApplicationFixture
{
    // These resources are shared across the whole assembly and are disposed in AssemblyFixture's [OneTimeTearDown]
    // (via StopApplicationAsync), not in the per-scenario [TearDown] below — disposing them per scenario would kill the
    // host after the first test. Suppress NUnit1032, which cannot see the OneTimeTearDown ownership.
#pragma warning disable NUnit1032
    private static WebApplication? _application;
    private static IPlaywright? _playwright;
    private static IBrowser? _browser;
    private static IPage? _page;
#pragma warning restore NUnit1032
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
    ///     Gets a value indicating whether the per-scenario teardown undoes every command and asserts the command-driven
    ///     state is empty. A scenario whose subject is undo/redo overrides this to <c>false</c> and takes responsibility for
    ///     leaving the history clean. Character deletion is not gated by this flag.
    /// </summary>
    protected virtual bool UndoAllCommandsOnTeardown
    {
        get
        {
            return true;
        }
    }

    /// <summary>
    ///     Runs after every scenario. Undoes every command while the history is non-empty, asserts the command-driven state
    ///     is genuinely empty, then deletes any leftover characters through the repository business API and asserts the
    ///     repository is empty. A scenario that opts out of the undo loop (via <see cref="UndoAllCommandsOnTeardown"/>) still
    ///     has its characters cleaned up here, because character persistence is not command-driven.
    /// </summary>
    [TearDown]
    public async Task CleanUpScenarioAsync()
    {
        if (UndoAllCommandsOnTeardown)
        {
            var mediator = Services.GetRequiredService<IUndoableMediator>();
            // Stop if an undo reports failure rather than looping forever on a command whose undo cannot decrement history.
            while (mediator.HistoryLength > 0 && await mediator.UndoLastCommandAsync())
            {
            }

            AssertCommandDrivenStateIsEmpty();

            // The undo above left the redo stack holding those commands. Nothing in IUndoableMediator can clear it, and
            // "clean state" and "empty redo stack" are mutually exclusive through the API; the next scenario's first command
            // clears it. Log undo hides entries rather than removing them, so undo alone cannot empty Blocks. The assertion
            // above already proved nothing is visible; clear it now so the next scenario starts from a genuinely empty log.
            var logService = Services.GetRequiredService<IDnDLogService>();
            logService.Clear();
            logService.Blocks.Should().BeEmpty();
        }

        var characterRepository = Services.GetRequiredService<ICharacterRepository>();
        foreach (var character in characterRepository.GetAllCharacters().ToArray())
        {
            characterRepository.Delete(character);
        }

        characterRepository.Count.Should().Be(0);
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
            var builder = WebApplication.CreateBuilder();
            builder.Environment.EnvironmentName = "Development";
            builder.Environment.ApplicationName = typeof(ServiceCollectionExtensions).Assembly.GetName().Name!;
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
