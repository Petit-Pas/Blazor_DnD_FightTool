using DnDFightTool.Domain.CharacterSheet.Damage;
using DnDFightTool.Domain.CharacterSheet.DamageAffinities;
using FluentAssertions;
using NUnit.Framework;

namespace DnDEntitiesTests.DamageAffinities;

[TestFixture]
public class DamageAffinityTests
{
    [TestFixture]
    public class IncreaseAffinityTests : DamageAffinityTests
    {
        [Test]
        [TestCase(DamageAffinityEnum.Weak, DamageAffinityEnum.Normal)]
        [TestCase(DamageAffinityEnum.Normal, DamageAffinityEnum.Resistant)]
        [TestCase(DamageAffinityEnum.Resistant, DamageAffinityEnum.Immune)]
        [TestCase(DamageAffinityEnum.Immune, DamageAffinityEnum.Heal)]
        [TestCase(DamageAffinityEnum.Heal, DamageAffinityEnum.Heal)]
        public void Should_StepUpOnce_And_CapAtHeal(DamageAffinityEnum initial, DamageAffinityEnum expected)
        {
            // Arrange
            var affinity = new DamageAffinity(DamageTypeEnum.Fire, initial);

            // Act
            affinity.IncreaseAffinity();

            // Assert
            affinity.Affinity.Should().Be(expected);
        }
    }

    [TestFixture]
    public class DecreaseAffinityTests : DamageAffinityTests
    {
        [Test]
        [TestCase(DamageAffinityEnum.Heal, DamageAffinityEnum.Immune)]
        [TestCase(DamageAffinityEnum.Immune, DamageAffinityEnum.Resistant)]
        [TestCase(DamageAffinityEnum.Resistant, DamageAffinityEnum.Normal)]
        [TestCase(DamageAffinityEnum.Normal, DamageAffinityEnum.Weak)]
        [TestCase(DamageAffinityEnum.Weak, DamageAffinityEnum.Weak)]
        public void Should_StepDownOnce_And_FloorAtWeak(DamageAffinityEnum initial, DamageAffinityEnum expected)
        {
            // Arrange
            var affinity = new DamageAffinity(DamageTypeEnum.Fire, initial);

            // Act
            affinity.DecreaseAffinity();

            // Assert
            affinity.Affinity.Should().Be(expected);
        }
    }
}
