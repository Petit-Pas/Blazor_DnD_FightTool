using System.Linq;
using DnDFightTool.Domain.CharacterSheet.AbilityScores;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Dices;
using DnDFightTool.Domain.CharacterSheet.Dices.Modifiers;
using FluentAssertions;
using NUnit.Framework;

namespace DnDEntitiesTests.Dices;

[TestFixture]
public class WildcardTests
{
    private class ResolveTests : WildcardTests
    {
        [Test]
        [TestCase(AbilityEnum.Strength)]
        [TestCase(AbilityEnum.Dexterity)]
        [TestCase(AbilityEnum.Constitution)]
        [TestCase(AbilityEnum.Intelligence)]
        [TestCase(AbilityEnum.Wisdom)]
        [TestCase(AbilityEnum.Charisma)]
        public void Should_Resolve_Ability_Tokens(AbilityEnum ability)
        {
            // Arrange
            var wildcard = new Wildcard(ability.ShortName());
            var character = new Character(true);
            character.AbilityScores.Single(x => x.Ability == ability).Score = 24;

            // Act
            var modifier = wildcard.Resolve(character);

            // Assert
            modifier.Modifier.Should().Be(7);
        }

        [Test]
        public void Should_Resolve_Mastery_Token()
        {
            // Arrange
            var wildcard = new Wildcard("MAS");
            var character = new Character(true);
            character.AbilityScores.MasteryBonus = 7;

            // Act
            var modifier = wildcard.Resolve(character);

            // Assert
            modifier.Modifier.Should().Be(7);
        }
    }
}
