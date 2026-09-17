using FluentAssertions;
using NUnit.Framework;

namespace DnDFightTool.UiTests.UiTestNavigation.Meta;

/// <summary>
///     Verifies the screenshot evidence mechanism: named captures land on disk with the expected sequential file names,
///     and the per-scenario teardown writes an automatic <c>final</c> image after each scenario body completes.
/// </summary>
public sealed class ScreenshotEvidenceTests : IsolatedScenarioFixture
{
    // Captured during the tests so the fixture-level teardown can assert the automatic `final` image, which base
    // CleanUpScenarioAsync writes only after each test body has finished — too late for an in-body assertion.
    private static string? _namedCaptureFolder;
    private static string? _stepSequenceFolder;

    /// <summary>
    ///     NAMED_CAPTURE: loads a page and captures a named shot, then asserts the expected <c>00-home.png</c> file exists.
    /// </summary>
    [Test]
    public async Task Should_Write_A_Named_Capture_To_Disk()
    {
        // Arrange
        await Page.GotoAsync(BaseUrl);

        // Act
        var path = await CaptureAsync("home");

        // Assert
        _namedCaptureFolder = Path.GetDirectoryName(path);
        Path.GetFileName(path).Should().Be("00-home.png");
        File.Exists(path).Should().BeTrue();
    }

    /// <summary>
    ///     STEP_SEQUENCE: captures three named shots in order and asserts the folder holds sequentially numbered files. The
    ///     automatic <c>03-final.png</c> is asserted in <see cref="AssertAutomaticFinalImagesWereCaptured"/> once teardown
    ///     has run.
    /// </summary>
    [Test]
    public async Task Should_Write_Sequentially_Numbered_Captures_In_Call_Order()
    {
        // Arrange
        await Page.GotoAsync(BaseUrl);

        // Act
        var first = await CaptureAsync("one");
        var second = await CaptureAsync("two");
        var third = await CaptureAsync("three");

        // Assert
        _stepSequenceFolder = Path.GetDirectoryName(third);
        Path.GetFileName(first).Should().Be("00-one.png");
        Path.GetFileName(second).Should().Be("01-two.png");
        Path.GetFileName(third).Should().Be("02-three.png");
        File.Exists(first).Should().BeTrue();
        File.Exists(second).Should().BeTrue();
        File.Exists(third).Should().BeTrue();
    }

    /// <summary>
    ///     UNSAFE_NAME: a capture name containing path characters is sanitized to a filesystem-safe file name.
    /// </summary>
    [Test]
    public async Task Should_Sanitize_Unsafe_Capture_Names()
    {
        // Arrange
        await Page.GotoAsync(BaseUrl);

        // Act
        var path = await CaptureAsync("bad/name:step");

        // Assert
        var fileName = Path.GetFileName(path);
        fileName.IndexOfAny(Path.GetInvalidFileNameChars()).Should().Be(-1);
        fileName.Should().StartWith("00-");
        File.Exists(path).Should().BeTrue();
    }

    /// <summary>
    ///     STALE_RERUN: leftover files from a prior run are erased before a scenario runs, so a re-run's folder holds only
    ///     the current run's images. Clearing happens in the fixture's <c>[SetUp]</c> (<c>PrepareScenarioArtifacts</c>); here
    ///     we plant a stale file and re-run that same preparation to prove it removes leftovers.
    /// </summary>
    [Test]
    public void Should_Erase_Leftover_Screenshots_Before_A_Scenario_Runs()
    {
        // Arrange — simulate a leftover artifact from a previous run.
        var folder = GetScenarioFolder();
        Directory.CreateDirectory(folder);
        var stalePath = Path.Combine(folder, "99-stale.png");
        File.WriteAllBytes(stalePath, []);

        // Act — the same preparation the fixture runs in [SetUp].
        PrepareScenarioArtifacts();

        // Assert
        File.Exists(stalePath).Should().BeFalse();
    }

    /// <summary>
    ///     Asserts the automatic <c>final</c> capture that base <c>CleanUpScenarioAsync</c> takes after each scenario body:
    ///     the named-capture scenario ends with <c>01-final.png</c> and the step-sequence scenario with <c>03-final.png</c>.
    /// </summary>
    [OneTimeTearDown]
    public static void AssertAutomaticFinalImagesWereCaptured()
    {
        if (_namedCaptureFolder is not null)
        {
            File.Exists(Path.Combine(_namedCaptureFolder, "01-final.png")).Should().BeTrue();
        }

        if (_stepSequenceFolder is not null)
        {
            File.Exists(Path.Combine(_stepSequenceFolder, "03-final.png")).Should().BeTrue();
        }
    }
}
