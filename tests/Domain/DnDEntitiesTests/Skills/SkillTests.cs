using DnDFightTool.Domain.CharacterSheet.Skills;
using FluentAssertions;
using NUnit.Framework;

namespace DnDEntitiesTests.Skills;

[TestFixture]
public class SkillTests
{
    private Skill _skill = null!;

    [SetUp]
    public void SetUp()
    {
        _skill = new Skill(SkillEnum.Athletics);
    }

    [TestFixture]
    public class IncreaseMasteryTests : SkillTests
    {
        [Test]
        [TestCase(SkillMasteryEnum.Normal, SkillMasteryEnum.Mastery)]
        [TestCase(SkillMasteryEnum.Mastery, SkillMasteryEnum.Expertise)]
        [TestCase(SkillMasteryEnum.Expertise, SkillMasteryEnum.Expertise)]
        public void Should_StepUpOnce_And_CapAtExpertise(SkillMasteryEnum initial, SkillMasteryEnum expected)
        {
            // Arrange
            _skill.Mastery = initial;

            // Act
            _skill.IncreaseMastery();

            // Assert
            _skill.Mastery.Should().Be(expected);
        }
    }

    [TestFixture]
    public class DecreaseMasteryTests : SkillTests
    {
        [Test]
        [TestCase(SkillMasteryEnum.Expertise, SkillMasteryEnum.Mastery)]
        [TestCase(SkillMasteryEnum.Mastery, SkillMasteryEnum.Normal)]
        [TestCase(SkillMasteryEnum.Normal, SkillMasteryEnum.Normal)]
        public void Should_StepDownOnce_And_FloorAtNormal(SkillMasteryEnum initial, SkillMasteryEnum expected)
        {
            // Arrange
            _skill.Mastery = initial;

            // Act
            _skill.DecreaseMastery();

            // Assert
            _skill.Mastery.Should().Be(expected);
        }
    }
}
