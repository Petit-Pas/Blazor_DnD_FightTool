using DnDFightTool.Components.DndUi.Web.IoC;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DomainTestsUtilities.Factories.Characters;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace DndUiWebTests.IoC;

public class RegisterWebAppServicesTests
{
    private string _dataFolder = null!;

    [SetUp]
    public void SetUp()
    {
        _dataFolder = Path.Combine(Path.GetTempPath(), $"DnDFightTool.Tests.{Guid.NewGuid():N}");
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_dataFolder))
        {
            Directory.Delete(_dataFolder, recursive: true);
        }
    }

    private WebApplicationBuilder BuildHost()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.RegisterWebAppServices(_dataFolder);
        builder.Host.UseDefaultServiceProvider(options =>
        {
            options.ValidateOnBuild = true;
            options.ValidateScopes = true;
        });

        return builder;
    }

    [Test]
    public void Should_Build_A_Container_With_No_Missing_Registration_Or_Captured_Scope()
    {
        // Arrange
        var builder = BuildHost();

        // Act
        var act = () => builder.Build();

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void Should_Persist_Characters_To_The_Provided_DataFolder()
    {
        // Arrange
        var app = BuildHost().Build();
        var repository = app.Services.GetRequiredService<ICharacterRepository>();

        // Act
        repository.Save(CharacterFactory.BuildMonster());

        // Assert
        Directory.GetFiles(_dataFolder).Should().HaveCount(1);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Should_Throw_When_DataFolder_Is_Missing(string? dataFolder)
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.RegisterWebAppServices(dataFolder!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
