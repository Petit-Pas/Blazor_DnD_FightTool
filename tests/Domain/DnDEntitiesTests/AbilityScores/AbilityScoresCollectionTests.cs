using DnDFightTool.Domain.DnDEntities.AbilityScores;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;

namespace DnDEntitiesTests.AbilityScores;

[TestFixture]
public class AbilityScoresCollectionTests
{
    private readonly AbilityScoresCollection _abilityCollection = new(true);

    [SetUp]
    public void SetUp()
    {
        var str = _abilityCollection.First(x => x.Ability == AbilityEnum.Strength);
        str.HasMastery = true;
        _abilityCollection.First(x => x.Ability == AbilityEnum.Dexterity).Score = 16;
    }

    [Test]
    [TestCase(10, 0)]
    [TestCase(14, 2)]
    public void Should_Return_Modifier_Without_Mastery(int score, int expectedModifier)
    {
        // Arrange
        _abilityCollection.First(x => x.Ability == AbilityEnum.Strength).Score = score;

        // Act
        var scoreModifier = _abilityCollection.GetModifier(AbilityEnum.Strength);

        // Assert
        scoreModifier.Modifier.Should().Be(expectedModifier);
    }

    [Test]
    [TestCase(3, 3)]
    [TestCase(1, 1)]
    public void Should_Return_Modifier_With_Mastery_When_Asked_For_Saving(int masteryBonus, int expectedModifier)
    {
        // Arrange
        _abilityCollection.MasteryBonus = masteryBonus;

        // Act
        var scoreModifier = _abilityCollection.GetSavingModifier(AbilityEnum.Strength);

        // Assert
        scoreModifier.Modifier.Should().Be(expectedModifier);
    }

    [Test]
    [TestCase(AbilityEnum.Strength, 0)]
    [TestCase(AbilityEnum.Dexterity, 3)]
    public void Should_Return_Modifier_Corresponding_To_The_Ability_Requested(AbilityEnum ability, int expectedModifier)
    {
        // Act
        var scoreModifier = _abilityCollection.GetModifier(ability);

        // Assert
        scoreModifier.Modifier.Should().Be(expectedModifier);
    }

}
