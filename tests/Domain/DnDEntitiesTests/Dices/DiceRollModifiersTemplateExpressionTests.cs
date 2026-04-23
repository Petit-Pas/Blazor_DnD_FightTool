using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Dices;
using FluentAssertions;
using NUnit.Framework;

namespace DnDEntitiesTests.Dices;

[TestFixture]
public class DiceRollModifiersTemplateExpressionTests
{
    [TestFixture]
    private class ParsingTests : DiceRollModifiersTemplateExpressionTests
    {

        [Test]
        public void Should_Parse_Wildcards()
        {
            // Arrange
            var modifier = new DiceRollModifiersTemplate("STR+INT+DEX+3+MAS");

            // Act
            // Assert
            modifier._wildcards.Should().Contain(x => x.Token == "STR");
            modifier._wildcards.Should().Contain(x => x.Token == "INT");
            modifier._wildcards.Should().Contain(x => x.Token == "DEX");
            modifier._wildcards.Should().Contain(x => x.Token == "MAS");
        }

        [Test]
        public void Should_Parse_Modifier()
        {
            // Arrange
            var modifier = new DiceRollModifiersTemplate("STR+2+INT+DEX+3+MAS-1");

            // Act
            // Assert
            modifier._staticModifier.Should().Be(4);
        }
    }

    [TestFixture]
    private class BuildExpressionTests : DiceRollModifiersTemplateExpressionTests
    {
        [Test]
        public void Should_Merge_Modifiers()
        {
            // Arrange
            var modifier = new DiceRollModifiersTemplate("2+8");

            // Act
            // Assert
            modifier.Expression.Should().Be("10");
        }

        [Test]
        public void Should_Order_Elements_Properly()
        {
            // Arrange
            var modifier = new DiceRollModifiersTemplate("8+MAS");

            // Act
            // Assert
            modifier.Expression.Should().Be("MAS+8");
        }

        [Test]
        public void Should_Not_Keep_Modifier_When_Empty()
        {
            // Arrange
            var modifier = new DiceRollModifiersTemplate("MAS+1-1");

            // Act
            // Assert
            modifier.Expression.Should().Be("MAS");
        }
    }

    [TestFixture]
    private class GetScoreModifierTests : DiceRollModifiersTemplateExpressionTests
    {
        [Test]
        public void Should_Include_Static_Modifiers()
        {
            // Arrange
            var modifier = new DiceRollModifiersTemplate("2");

            // Act
            // Assert
            modifier.GetScoreModifier(null!).Modifier.Should().Be(2);
        }

        [Test]
        public void Should_Resolve_Wildcards()
        {
            // Arrange
            var modifier = new DiceRollModifiersTemplate("MAS");
            var character = new Character(true);
            character.AbilityScores.MasteryBonus = 7;

            // Act
            // Assert
            modifier.GetScoreModifier(character).Modifier.Should().Be(7);
        }
    }
}
