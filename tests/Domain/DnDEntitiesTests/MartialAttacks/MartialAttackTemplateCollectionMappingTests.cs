using System.Collections.Generic;
using System.Linq;
using DnDFightTool.Domain.CharacterSheet.IoC;
using DnDFightTool.Domain.CharacterSheet.MartialAttacks;
using DnDFightTool.Infrastructure.Mapping;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DnDEntitiesTests.MartialAttacks;

[TestFixture]
public class MartialAttackTemplateCollectionMappingTests
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
    public class CloneTests : MartialAttackTemplateCollectionMappingTests
    {
        [Test]
        public void Should_HaveConsistentKeyValueIds_When_Cloned()
        {
            // Arrange
            var collection = BuildCollectionWithTwoAttacks();

            // Act
            var cloned = _mapper.Clone(collection);

            // Assert
            foreach (var (key, value) in cloned)
            {
                key.Should().Be(value.Id);
            }
        }

        [Test]
        public void Should_GenerateNewIds_When_Cloned()
        {
            // Arrange
            var collection = BuildCollectionWithTwoAttacks();
            var originalIds = collection.Keys.ToHashSet();

            // Act
            var cloned = _mapper.Clone(collection);

            // Assert
            cloned.Keys.Should().NotIntersectWith(originalIds);
        }
    }

    [TestFixture]
    public class CopyTests : MartialAttackTemplateCollectionMappingTests
    {
        [Test]
        public void Should_HaveConsistentKeyValueIds_When_Copied()
        {
            // Arrange
            var collection = BuildCollectionWithTwoAttacks();

            // Act
            var copied = _mapper.Copy(collection);

            // Assert
            foreach (var (key, value) in copied)
            {
                key.Should().Be(value.Id);
            }
        }

        [Test]
        public void Should_PreserveIds_When_Copied()
        {
            // Arrange
            var collection = BuildCollectionWithTwoAttacks();
            var originalIds = collection.Keys.ToHashSet();

            // Act
            var copied = _mapper.Copy(collection);

            // Assert
            copied.Keys.Should().BeEquivalentTo(originalIds);
        }
    }

    private static MartialAttackTemplateCollection BuildCollectionWithTwoAttacks()
    {
        var collection = new MartialAttackTemplateCollection(withDefault: false);
        collection.Add(new MartialAttackTemplate { Name = "Slash" });
        collection.Add(new MartialAttackTemplate { Name = "Stab" });
        return collection;
    }

    private sealed class ServiceCollectionStub : List<ServiceDescriptor>, IServiceCollection
    {
    }
}
