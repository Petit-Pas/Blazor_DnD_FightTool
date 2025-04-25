using System.Linq;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using FluentAssertions;
using NUnit.Framework;

namespace DnDEntitiesTests.Dices.DiceThrows;

[TestFixture]
public class DiceThrowExpressionTests
{
    [TestFixture]
    private class ParsingTests : DiceThrowExpressionTests
    {
        [Test]
        public void Should_Parse_Dices_Properly()
        {
            // Arrange
            var diceThrow = new DiceThrowTemplate("3d12+1d8+3+MAS+2d4");
            
            // Act
            // Assert
            diceThrow.GetDicesToRoll().SingleOrDefault(x => x.Value == 12)!.Amount.Should().Be(3);
            diceThrow.GetDicesToRoll().SingleOrDefault(x => x.Value == 8)!.Amount.Should().Be(1);
            diceThrow.GetDicesToRoll().SingleOrDefault(x => x.Value == 4)!.Amount.Should().Be(2);
        }

        [Test]
        public void Should_Parse_Wildcards()
        {
            // Arrange
            var diceThrow = new DiceThrowTemplate("STR+INT+DEX+1d6+3+MAS");

            // Act
            // Assert
            diceThrow._wildcards.Should().Contain(x => x.Token == "STR");
            diceThrow._wildcards.Should().Contain(x => x.Token == "INT");
            diceThrow._wildcards.Should().Contain(x => x.Token == "DEX");
            diceThrow._wildcards.Should().Contain(x => x.Token == "MAS");
        }

        [Test]
        public void Should_Parse_Modifier()
        {
            // Arrange
            var diceThrow = new DiceThrowTemplate("STR+2+INT+DEX+1d6+3+MAS-1");

            // Act
            // Assert
            diceThrow._staticModifier.Should().Be(4);
        }
    }

    [TestFixture]
    private class BuildExpressionTests : DiceThrowExpressionTests
    {
        [Test]
        public void Should_Merge_Dices_With_Same_Value()
        {
            // Arrange
            var diceThrow = new DiceThrowTemplate("1d12+2d12-1d12");

            // Act
            // Assert
            diceThrow.Expression.Should().Be("2d12");
        }

        [Test]
        public void Should_Order_Dices_By_Growing_Value()
        {
            // Arrange
            var diceThrow = new DiceThrowTemplate("1d12+1d4-1d8");

            // Act
            // Assert
            diceThrow.Expression.Should().Be("1d12-1d8+1d4");
        }

        [Test]
        public void Should_Merge_Modifiers()
        {
            // Arrange
            var diceThrow = new DiceThrowTemplate("2+8");

            // Act
            // Assert
            diceThrow.Expression.Should().Be("10");
        }

        [Test]
        public void Should_Order_Elements_Properly()
        {
            // Arrange
            var diceThrow = new DiceThrowTemplate("8+MAS+1d8");

            // Act
            // Assert
            diceThrow.Expression.Should().Be("1d8+MAS+8");
        }

        [Test]
        public void Should_Not_Keep_Modifier_When_Empty()
        {
            // Arrange
            var diceThrow = new DiceThrowTemplate("1d8+1-1");

            // Assert
            diceThrow.Expression.Should().Be("1d8");
        }
    }

    [TestFixture]
    private class GetScoreModifierTests : DiceThrowExpressionTests
    {
        [Test]
        public void Should_Include_Static_Modifiers()
        {
            // Arrange
            var diceThrow = new DiceThrowTemplate("1d8+2");

            // Act
            var scoreModifier = diceThrow.GetScoreModifier(null!);

            // Assert
            ((int)scoreModifier).Should().Be(2);
        }

        [Test]
        public void Should_Resolve_Wildcards()
        {
            // Arrange
            var character = new Character(true);
            character.AbilityScores.MasteryBonus = 7;
            var diceThrow = new DiceThrowTemplate()
            {
                Expression = "1d8+MAS"
            };

            // Act
            var scoreModifier = diceThrow.GetScoreModifier(character);

            // Assert
            ((int)scoreModifier).Should().Be(7);
        }
    }
}
