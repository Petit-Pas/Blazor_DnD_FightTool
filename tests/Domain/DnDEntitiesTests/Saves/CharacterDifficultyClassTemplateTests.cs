using System;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Saves;
using FluentAssertions;
using NUnit.Framework;

namespace DnDEntitiesTests.Saves;

[TestFixture]
internal class CharacterDifficultyClassTemplateTests
{
    [TestFixture]
    private class ParsingTests : CharacterDifficultyClassTemplateTests
    {
        [Test]
        public void Should_Parse_Wildcards()
        {
            // Arrange
            var dcTemplate = new CharacterDifficultyClassTemplate("STR+INT+DEX+3+MAS");

            // Act
            // Assert
            dcTemplate._wildcards.Should().Contain(x => x.Token == "STR");
            dcTemplate._wildcards.Should().Contain(x => x.Token == "INT");
            dcTemplate._wildcards.Should().Contain(x => x.Token == "DEX");
            dcTemplate._wildcards.Should().Contain(x => x.Token == "MAS");
        }

        [Test]
        public void Should_Parse_Modifier()
        {
            // Arrange
            var dcTemplate = new CharacterDifficultyClassTemplate("STR+2+INT+DEX+3+MAS-1");

            // Act
            // Assert
            dcTemplate._staticModifier.Should().Be(4);
        }

        [Test]
        public void Should_Work_With_Empty()
        {
            // Arrange
            var dcTemplate = new CharacterDifficultyClassTemplate(string.Empty);

            // Act
            // Assert
            dcTemplate._wildcards.Should().BeEmpty();
            dcTemplate._staticModifier.Should().Be(0);
        }

        [Test]
        public void Should_Not_Allow_For_Dc_Wildcard()
        {
            // Arrange
            var buildingWrongTemplate = () => new CharacterDifficultyClassTemplate("DC");

            // Act & Assert
            buildingWrongTemplate.Should().Throw<InvalidOperationException>()
                .WithMessage("The expression DC is not a valid expression to describe a character DC.");
        }

        [Test]
        public void Should_Not_Allow_For_Dices()
        {
            // Arrange
            var buildingWrongTemplate = () => new CharacterDifficultyClassTemplate("2d6");
            // Act & Assert
            buildingWrongTemplate.Should().Throw<InvalidOperationException>()
                .WithMessage("The expression 2d6 is not a valid expression to describe a character DC.");
        }
    }

    [TestFixture]
    private class BuildExpressionTests : CharacterDifficultyClassTemplateTests
    {
        [Test]
        public void Should_Merge_Modifiers()
        {
            // Arrange
            var dcTemplate = new CharacterDifficultyClassTemplate("2+8");

            // Act
            // Assert
            dcTemplate.Expression.Should().Be("10");
        }

        [Test]
        public void Should_Order_Elements_Properly()
        {
            // Arrange
            var dcTemplate = new CharacterDifficultyClassTemplate("MAS+8");

            // Act
            // Assert
            dcTemplate.Expression.Should().Be("8+MAS");
        }

        [Test]
        public void Should_Not_Keep_Modifier_When_Empty()
        {
            // Arrange
            var dcTemplate = new CharacterDifficultyClassTemplate("MAS+1-1");

            // Act
            // Assert
            dcTemplate.Expression.Should().Be("MAS");
        }

        [Test]
        public void Should_Work_With_Empty()
        {
            // Arrange 
            var dcTemplate = new CharacterDifficultyClassTemplate(string.Empty);

            // Act
            // Assert
            dcTemplate.Expression.Should().Be(string.Empty);
        }
    }

    [TestFixture]
    private class GetDcModifierTests : CharacterDifficultyClassTemplateTests
    {
        [Test]
        public void Should_Include_Static_Modifiers()
        {
            // Arrange
            var dcTemplate = new CharacterDifficultyClassTemplate("2");

            // Act
            var dc = dcTemplate.GetDc(null!);

            // Assert
            dc.Should().Be(2);
        }

        [Test]
        public void Should_Resolve_Wildcards()
        {
            // Arrange
            var character = new Character(true);
            character.AbilityScores.MasteryBonus = 7;
            var dcTemplate = new CharacterDifficultyClassTemplate("MAS");

            // Act
            var dc = dcTemplate.GetDc(character);

            // Assert
            dc.Should().Be(7);
        }

        [Test]
        public void Should_Work_With_Empty()
        {
            // Arrange
            var diceThrow = new CharacterDifficultyClassTemplate(string.Empty);

            // Act
            var dc = diceThrow.GetDc(null!);

            // Assert
            dc.Should().Be(0);
        }
    }
}
