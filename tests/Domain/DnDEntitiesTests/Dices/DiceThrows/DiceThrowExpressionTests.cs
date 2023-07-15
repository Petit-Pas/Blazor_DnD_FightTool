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
            var diceThrow = new DiceThrowTemplate();

            // Act
            diceThrow.Expression = "3d12+1d8+3+MAS+2d4";

            // Assert
            diceThrow.GetDicesToRoll().SingleOrDefault(x => x.Value == 12)!.Amount.Should().Be(3);
            diceThrow.GetDicesToRoll().SingleOrDefault(x => x.Value == 8)!.Amount.Should().Be(1);
            diceThrow.GetDicesToRoll().SingleOrDefault(x => x.Value == 4)!.Amount.Should().Be(2);
        }

        [Test]
        public void Should_Parse_Wildcards()
        {
            // Arrange
            var diceThrow = new DiceThrowTemplate();

            // Act
            diceThrow.Expression = "STR+INT+DEX+1d6+3+MAS";

            // Assert
            diceThrow.Wildcards.Should().Contain(x => x.Token == "STR");
            diceThrow.Wildcards.Should().Contain(x => x.Token == "INT");
            diceThrow.Wildcards.Should().Contain(x => x.Token == "DEX");
            diceThrow.Wildcards.Should().Contain(x => x.Token == "MAS");
        }

        [Test]
        public void Should_Parse_Modifier()
        {
            // Arrange
            var diceThrow = new DiceThrowTemplate();

            // Act
            diceThrow.Expression = "STR+2+INT+DEX+1d6+3+MAS-1";

            // Assert
            diceThrow.StaticModifier.Should().Be(4);
        }
    }

    [TestFixture]
    private class BuildExpressionTests : DiceThrowExpressionTests
    {
        [Test]
        public void Should_Merge_Dices_With_Same_Value()
        {
            // Arrange
            var diceThrow = new DiceThrowTemplate();

            // Act
            diceThrow.Expression = "1d12+2d12-1d12";

            // Assert
            diceThrow.Expression.Should().Be("2d12");
        }

        [Test]
        public void Should_Order_Dices_By_Growing_Value()
        {
            // Arrange
            var diceThrow = new DiceThrowTemplate();

            // Act
            diceThrow.Expression = "1d12+1d4-1d8";

            // Assert
            diceThrow.Expression.Should().Be("1d12-1d8+1d4");
        }

        [Test]
        public void Should_Merge_Modifiers()
        {
            // Arrange
            var diceThrow = new DiceThrowTemplate();

            // Act
            diceThrow.Expression = "2+8";

            // Assert
            diceThrow.Expression.Should().Be("10");
        }

        [Test]
        public void Should_Order_Elements_Properly()
        {
            // Arrange
            var diceThrow = new DiceThrowTemplate();

            // Act
            diceThrow.Expression = "8+MAS+1d8";

            // Assert
            diceThrow.Expression.Should().Be("1d8+MAS+8");
        }

        [Test]
        public void Should_Not_Keep_Modifier_When_Empty()
        {
            // Arrange
            var diceThrow = new DiceThrowTemplate();

            // Act
            diceThrow.Expression = "1d8+1-1";

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
            var diceThrow = new DiceThrowTemplate();

            // Act
            diceThrow.Expression = "1d8+2";

            // Assert
            diceThrow.GetScoreModifier(null).Modifier.Should().Be(2);
        }

        [Test]
        public void Should_Resolve_Wildcards()
        {
            // Arrange
            var diceThrow = new DiceThrowTemplate();
            var character = new Character(true);
            character.AbilityScores.MasteryBonus = 7;

            // Act
            diceThrow.Expression = "1d8+MAS";

            // Assert
            diceThrow.GetScoreModifier(character).Modifier.Should().Be(7);
        }
    }
}
