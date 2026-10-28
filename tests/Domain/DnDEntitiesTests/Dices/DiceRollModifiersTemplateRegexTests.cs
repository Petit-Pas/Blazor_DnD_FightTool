using DnDFightTool.Domain.CharacterSheet.Dices;
using DnDFightTool.Infrastructure.Extensions;
using FluentAssertions;
using NUnit.Framework;

namespace DnDEntitiesTests.Dices;

[TestFixture]
public class DiceRollModifiersTemplateRegexTests
{
    [TestFixture]
    private class IsValidTests : DiceRollModifiersTemplateRegexTests
    {
        [Test]
        public void Should_Accept_A_Modifiers_Expression()
        {
            // Arrange
            var template = new DiceRollModifiersTemplate();

            // Act
            var isValid = template.IsValid("WIS+8");

            // Assert
            isValid.Should().BeTrue();
        }

        [Test]
        public void Should_Accept_A_Negative_Static_Modifier()
        {
            // Arrange
            var template = new DiceRollModifiersTemplate();

            // Act
            var isValid = template.IsValid("-1");

            // Assert
            isValid.Should().BeTrue();
        }

        [Test]
        public void Should_Reject_A_Dice_Term()
        {
            // Arrange
            var template = new DiceRollModifiersTemplate();

            // Act
            var isValid = template.IsValid("1d4");

            // Assert
            isValid.Should().BeFalse();
        }
    }
}
