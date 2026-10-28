using System.Collections.Generic;
using DnDFightTool.Domain.CharacterSheet.Damage;
using DnDFightTool.Domain.CharacterSheet.Dices;
using DnDFightTool.Domain.CharacterSheet.MartialAttacks;
using DnDFightTool.Domain.CharacterSheet.IoC;
using DnDFightTool.Infrastructure.Mapping;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DnDEntitiesTests.Dices.Mapping;

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
    public class CopyTests : MappingConfigurationsTests
    {
        [Test]
        public void Should_Not_Share_DiceRollTemplate_When_Copied()
        {
            // Arrange
            var original = new DiceRollTemplate("1d6");

            // Act
            var copied = _mapper.Copy(original);
            copied.Expression = "2d8";

            // Assert
            copied.Should().NotBeSameAs(original);
            original.Expression.Should().Be("1d6");
        }

        [Test]
        public void Should_Not_Share_DiceRollModifiersTemplate_When_Copied()
        {
            // Arrange
            var original = new DiceRollModifiersTemplate("WIS+8");

            // Act
            var copied = _mapper.Copy(original);
            copied.Expression = "STR+2";

            // Assert
            copied.Should().NotBeSameAs(original);
            original.Expression.Should().Be("WIS+8");
        }

        [Test]
        public void Should_Keep_Expression_When_Copied()
        {
            // Arrange
            var original = new DiceRollTemplate("2d8+STR");

            // Act
            var copied = _mapper.Copy(original);

            // Assert
            copied.Expression.Should().Be("2d8+STR");
        }

        [TestCase("1-1", "0")]
        [TestCase("2-3", "-1")]
        [TestCase("1d6-2d6", "-1d6")]
        public void Should_Copy_A_Template_Whose_Expression_Does_Not_Reparse(string input, string expected)
        {
            // Arrange
            var original = new DiceRollTemplate(input);

            // Act
            var copied = _mapper.Copy(original);

            // Assert
            copied.Expression.Should().Be(expected);
        }

        [Test]
        public void Should_Not_Share_Dices_When_Copied()
        {
            // Arrange
            var original = new DiceRollTemplate("1d6");

            // Act
            var copied = _mapper.Copy(original);
            copied.GetDicesToRoll()[0].Amount = 3;

            // Assert
            original.Expression.Should().Be("1d6");
        }

        [Test]
        public void Should_Not_Share_Dices_When_An_Attack_Is_Copied()
        {
            // Arrange
            var original = new MartialAttackTemplate
            {
                ToHitModifiers = new DiceRollModifiersTemplate("WIS+8"),
                Damages = [new DamageRollTemplate { Dices = new DiceRollTemplate("1d6"), Type = DamageTypeEnum.Fire }],
            };

            // Act
            var copied = _mapper.Copy(original);
            copied.ToHitModifiers.Expression = "STR+2";
            copied.Damages[0].Dices.Expression = "2d8";

            // Assert
            original.ToHitModifiers.Expression.Should().Be("WIS+8");
            original.Damages[0].Dices.Expression.Should().Be("1d6");
        }
    }

    [TestFixture]
    public class CloneTests : MappingConfigurationsTests
    {
        [Test]
        public void Should_Not_Share_Dices_When_An_Attack_Is_Cloned()
        {
            // Arrange
            var original = new MartialAttackTemplate
            {
                ToHitModifiers = new DiceRollModifiersTemplate("WIS+8"),
                Damages = [new DamageRollTemplate { Dices = new DiceRollTemplate("1d6"), Type = DamageTypeEnum.Fire }],
            };

            // Act
            var cloned = _mapper.Clone(original);
            cloned.ToHitModifiers.Expression = "STR+2";
            cloned.Damages[0].Dices.Expression = "2d8";

            // Assert
            original.ToHitModifiers.Expression.Should().Be("WIS+8");
            original.Damages[0].Dices.Expression.Should().Be("1d6");
        }
    }

    private sealed class ServiceCollectionStub : List<ServiceDescriptor>, IServiceCollection
    {
    }
}
