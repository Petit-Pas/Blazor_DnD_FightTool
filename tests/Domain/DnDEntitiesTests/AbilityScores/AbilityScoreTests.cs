using DnDEntities.AbilityScores;
using FluentAssertions;
using NUnit.Framework;

namespace DnDEntitiesTests.AbilityScores;

[TestFixture]
public class AbilityScoreTests
{
    [Test]
    [TestCase(1, -5)]
    [TestCase(2, -4)]
    [TestCase(3, -4)]
    [TestCase(4, -3)]
    [TestCase(5, -3)]
    [TestCase(6, -2)]
    [TestCase(7, -2)]
    [TestCase(8, -1)]
    [TestCase(9, -1)]
    [TestCase(10, 0)]
    [TestCase(11, 0)]
    [TestCase(12, 1)]
    [TestCase(13, 1)]
    [TestCase(14, 2)]
    [TestCase(15, 2)]
    [TestCase(16, 3)]
    [TestCase(17, 3)]
    [TestCase(18, 4)]
    [TestCase(19, 4)]
    [TestCase(20, 5)]
    [TestCase(21, 5)]
    [TestCase(22, 6)]
    public void GetModifierTests(int score, int expectedModifier)
    {
        // Arrange
        var abilityScore = new AbilityScore(AbilityEnum.Strength, score);
         
        // Arrange
        var modifier = abilityScore.GetModifier();

        // Assert
        modifier.Modifier.Should().Be(expectedModifier);
    }

    // TODO lacks unit tests with mastery bonus in GetModifier(here)
}
