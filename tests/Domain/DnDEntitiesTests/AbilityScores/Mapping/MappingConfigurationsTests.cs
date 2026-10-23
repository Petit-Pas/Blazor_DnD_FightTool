using System.Collections.Generic;
using System.Linq;
using DnDFightTool.Domain.CharacterSheet.AbilityScores;
using DnDFightTool.Domain.CharacterSheet.IoC;
using DnDFightTool.Infrastructure.Mapping;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DnDEntitiesTests.AbilityScores.Mapping;

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
        public void Should_PreserveMasteryBonus_When_Cloned()
        {
            // Arrange
            var collection = BuildCollection();

            // Act
            var cloned = _mapper.Clone(collection);

            // Assert
            cloned.MasteryBonus.Should().Be(4);
        }

        [Test]
        public void Should_PreserveAbilityScores_When_Cloned()
        {
            // Arrange
            var collection = BuildCollection();

            // Act
            var cloned = _mapper.Clone(collection);

            // Assert
            cloned.Should().BeEquivalentTo(collection);
        }
    }

    [TestFixture]
    public class CopyTests : MappingConfigurationsTests
    {
        [Test]
        public void Should_PreserveMasteryBonus_When_Copied()
        {
            // Arrange
            var collection = BuildCollection();

            // Act
            var copied = _mapper.Copy(collection);

            // Assert
            copied.MasteryBonus.Should().Be(4);
        }

        [Test]
        public void Should_PreserveAbilityScores_When_Copied()
        {
            // Arrange
            var collection = BuildCollection();

            // Act
            var copied = _mapper.Copy(collection);

            // Assert
            copied.Should().BeEquivalentTo(collection);
        }
    }

    private static AbilityScoresCollection BuildCollection()
    {
        var collection = new AbilityScoresCollection(withDefaults: true) { MasteryBonus = 4 };
        var strength = collection.First(x => x.Ability == AbilityEnum.Strength);
        strength.Score = 18;
        strength.HasMastery = true;
        return collection;
    }

    private sealed class ServiceCollectionStub : List<ServiceDescriptor>, IServiceCollection
    {
    }
}
