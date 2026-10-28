using DnDFightTool.Domain.CharacterSheet.Dices.Validation;
using FluentAssertions;
using NUnit.Framework;

namespace DnDEntitiesTests.Dices.Validation;

[TestFixture]
public class DiceRollTemplateValidatorTests
{
    [TestFixture]
    public class MatchRegexTests : DiceRollTemplateValidatorTests
    {
        [Test]
        public void Should_Accept_A_Dice_Expression()
        {
            // Arrange
            const string expression = "2d8+STR";

            // Act
            var isMatch = DiceRollTemplateValidator.MatchRegex(expression);

            // Assert
            isMatch.Should().BeTrue();
        }

        [Test]
        public void Should_Accept_A_Modifiers_Expression()
        {
            // Arrange
            const string expression = "WIS+8";

            // Act
            var isMatch = DiceRollTemplateValidator.MatchRegex(expression);

            // Assert
            isMatch.Should().BeTrue();
        }

        [Test]
        public void Should_Reject_An_Invalid_Expression()
        {
            // Arrange
            const string expression = "abc";

            // Act
            var isMatch = DiceRollTemplateValidator.MatchRegex(expression);

            // Assert
            isMatch.Should().BeFalse();
        }
    }
}
