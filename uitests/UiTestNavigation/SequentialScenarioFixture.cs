using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace DnDFightTool.UiTests.UiTestNavigation;

/// <summary>
///     Base fixture for a chain of ordered steps (<c>[Order]</c>) that build on one another and share application state.
///     The heavy reset (undo everything, delete characters) runs only at the fixture boundary — <see cref="OneTimeSetUp"/>
///     and <see cref="OneTimeTearDown"/> — so state persists between steps and each step can be a small asserted increment
///     with its own screenshots. Once a step fails, the remaining steps are skipped rather than run against a broken state.
/// </summary>
public abstract class SequentialScenarioFixture : ApplicationFixture
{
    // Persists across the fixture's steps (one fixture instance is reused), so a failed step can short-circuit the rest.
    private bool _anEarlierStepFailed;

    /// <summary>
    ///     Cleans any leftover application state before the chain begins, so the first step starts from a clean slate
    ///     regardless of what ran before this fixture.
    /// </summary>
    [OneTimeSetUp]
    public async Task ResetSharedStateBeforeChainAsync()
    {
        await ResetApplicationStateAsync();
    }

    /// <summary>
    ///     Skips the step if an earlier one failed (the chain relies on each step's success); otherwise empties this step's
    ///     screenshot folder and resets the step index.
    /// </summary>
    [SetUp]
    public void PrepareStep()
    {
        if (_anEarlierStepFailed)
        {
            Assert.Ignore("Skipped because an earlier step in this sequence failed.");
        }

        PrepareScenarioArtifacts();
    }

    /// <summary>
    ///     Captures this step's final image and records whether it failed, so the next step can short-circuit. The shared
    ///     application state is deliberately left intact for the next step.
    /// </summary>
    [TearDown]
    public async Task CaptureStepAsync()
    {
        await CaptureFinalStateAsync();

        if (TestContext.CurrentContext.Result.Outcome.Status is TestStatus.Failed)
        {
            _anEarlierStepFailed = true;
        }
    }

    /// <summary>
    ///     Resets the application to a proven-clean slate once the whole chain has run, so the next fixture starts empty.
    /// </summary>
    [OneTimeTearDown]
    public async Task ResetSharedStateAfterChainAsync()
    {
        await ResetApplicationStateAsync();
    }
}
