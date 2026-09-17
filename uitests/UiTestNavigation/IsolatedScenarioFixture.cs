using NUnit.Framework;

namespace DnDFightTool.UiTests.UiTestNavigation;

/// <summary>
///     Base fixture for scenarios that must be fully independent of one another. Every scenario starts from a clean slate
///     and the application state is reset after each one, so the suite passes in any order. Derive from this for a normal
///     UI scenario.
/// </summary>
public abstract class IsolatedScenarioFixture : ApplicationFixture
{
    /// <summary>
    ///     Empties this scenario's screenshot folder and resets the step index before the scenario runs.
    /// </summary>
    [SetUp]
    public void PrepareScenario()
    {
        PrepareScenarioArtifacts();
    }

    /// <summary>
    ///     Captures the final image, then resets the application to a clean slate — undoing every command and deleting
    ///     leftover characters — so the next scenario starts empty regardless of what this one did.
    /// </summary>
    [TearDown]
    public async Task CleanUpScenarioAsync()
    {
        await CaptureFinalStateAsync();
        await ResetApplicationStateAsync();
    }
}
