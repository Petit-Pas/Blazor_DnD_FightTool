using System.Collections.Generic;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.IoC;
using DnDFightTool.Infrastructure.Mapping;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DnDEntitiesTests.Characters.Mapping;

[TestFixture]
public class MappingConfigurationsTests
{
    private Mapper _mapper = null!;

    [OneTimeSetUp]
    public void RegisterMappings()
    {
        new ServiceCollectionStub().RegisterCharacterSheetMappingConfigurations();
    }

    [SetUp]
    public void SetUp()
    {
        _mapper = new Mapper();
    }

    [TestFixture]
    public class CloneTests : MappingConfigurationsTests
    {
        [Test]
        public void Should_PreserveNestedMasteryBonus_When_Cloned()
        {
            // Arrange
            var character = BuildCharacter();

            // Act
            var cloned = _mapper.Clone(character);

            // Assert
            cloned.AbilityScores.MasteryBonus.Should().Be(4);
        }
    }

    [TestFixture]
    public class CopyTests : MappingConfigurationsTests
    {
        [Test]
        public void Should_PreserveNestedMasteryBonus_When_Copied()
        {
            // Arrange
            var character = BuildCharacter();

            // Act
            var copied = _mapper.Copy(character);

            // Assert
            copied.AbilityScores.MasteryBonus.Should().Be(4);
        }
    }

    private static Character BuildCharacter()
    {
        var character = new Character(withDefaults: true) { Name = "Fighter" };
        character.AbilityScores.MasteryBonus = 4;
        return character;
    }

    private sealed class ServiceCollectionStub : List<ServiceDescriptor>, IServiceCollection
    {
    }
}
