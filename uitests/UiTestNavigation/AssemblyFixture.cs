using NUnit.Framework;

namespace DnDFightTool.UiTests;

/// <summary>
///     Owns the single application and browser lifecycle for the test assembly. Declared in the shared
///     <c>DnDFightTool.UiTests</c> namespace so NUnit applies it to every sub-namespace, including the
///     linked copy compiled into the scratch project.
/// </summary>
[SetUpFixture]
public sealed class AssemblyFixture
{
    /// <summary>
    ///     Starts the shared application before scenarios run.
    /// </summary>
    [OneTimeSetUp]
    public async Task SetUpAsync()
    {
        await UiTestNavigation.ApplicationFixture.StartApplicationAsync();
    }

    /// <summary>
    ///     Stops the shared application after all scenarios complete.
    /// </summary>
    [OneTimeTearDown]
    public async Task TearDownAsync()
    {
        await UiTestNavigation.ApplicationFixture.StopApplicationAsync();
    }
}