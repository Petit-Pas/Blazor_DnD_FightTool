using System.Collections.Generic;
using System.Linq;
using DnDFightTool.Domain.CharacterSheet.IoC;
using DnDFightTool.Domain.CharacterSheet.Statuses;
using DnDFightTool.Infrastructure.Mapping;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DnDEntitiesTests.Statuses;

[TestFixture]
public class StatusTemplateCollectionMappingTests
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
    public class CloneTests : StatusTemplateCollectionMappingTests
    {
        [Test]
        public void Should_HaveConsistentKeyValueIds_When_Cloned()
        {
            // Arrange
            var collection = BuildCollectionWithTwoStatuses();

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
            var collection = BuildCollectionWithTwoStatuses();
            var originalIds = collection.Keys.ToHashSet();

            // Act
            var cloned = _mapper.Clone(collection);

            // Assert
            cloned.Keys.Should().NotIntersectWith(originalIds);
        }
    }

    [TestFixture]
    public class CopyTests : StatusTemplateCollectionMappingTests
    {
        [Test]
        public void Should_HaveConsistentKeyValueIds_When_Copied()
        {
            // Arrange
            var collection = BuildCollectionWithTwoStatuses();

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
            var collection = BuildCollectionWithTwoStatuses();
            var originalIds = collection.Keys.ToHashSet();

            // Act
            var copied = _mapper.Copy(collection);

            // Assert
            copied.Keys.Should().BeEquivalentTo(originalIds);
        }
    }

    private static StatusTemplateCollection BuildCollectionWithTwoStatuses()
    {
        var collection = new StatusTemplateCollection();
        collection.Add(new StatusTemplate("Poisoned"));
        collection.Add(new StatusTemplate("Stunned"));
        return collection;
    }

    private sealed class ServiceCollectionStub : List<ServiceDescriptor>, IServiceCollection
    {
    }
}
