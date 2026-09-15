using FluentAssertions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace DnDFightTool.UiTests.UiTestNavigation.Meta;

/// <summary>
///     Verifies that the real web host renders the home page.
/// </summary>
public sealed class HomePageSmokeTests : ApplicationFixture
{
    /// <summary>
    ///     Loads the home page and verifies its character tabs are rendered.
    /// </summary>
    [Test]
    public async Task Should_Render_The_Home_Page()
    {
        // Arrange
        var response = await Page.GotoAsync(BaseUrl);

        // Act
        var playersTab = Page.GetByRole(AriaRole.Tab, new PageGetByRoleOptions { Name = "Players", Exact = true });

        // Assert
        response.Should().NotBeNull();
        response!.Ok.Should().BeTrue();
        await Assertions.Expect(playersTab).ToBeVisibleAsync();
    }
}
