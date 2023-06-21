using DnDEntities.AbilityScores;
using DnDEntities.Characters;
using Fight.Savings;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;

namespace FightTests.Savings;

[TestFixture]
public class SavingRollResultTests
{
    [TestFixture]
    public class IsSuccesfullTests : SavingRollResultTests
    {
        Character _caster;
        Character _target;

        SaveRollResult _saveRoll;

        [SetUp]
        public void SetUp()
        {
            _caster = new Character();
            _target = new Character(true);

            _target.AbilityScores.First(x => x.Ability == AbilityEnum.Strength).Score = 14;


            _saveRoll = new SaveRollResult()
            {
                RolledResult = 10,
                Target = 10,
                Ability = AbilityEnum.Intelligence
            };
        }

        [Test]
        [TestCase(9, false)]
        [TestCase(10, true)]
        [TestCase(11, true)]
        public void Should_Be_Succesfull_When_Target_Is_Lower_Than_RolledResult(int rolledResult, bool expectedResult)
        {
            // Arrange
            _saveRoll.RolledResult = rolledResult;

            // Act
            var result = _saveRoll.IsSuccesfull(_target, _caster);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Test]
        [TestCase(14, false)]
        [TestCase(15, true)]
        [TestCase(16, true)]
        // TODO At the time of writing this test, the characters have no saving throw. So this test should be updated when the feature is implemented
        // TODO at the moment, the mocked value is 15
        public void Should_Use_Caster_Saving_Throw_When_Target_Is_Zero(int rolledResult, bool expectedResult)
        {
            // Arrange
            _saveRoll.RolledResult = rolledResult;
            _saveRoll.Target = 0;

            // Act
            var result = _saveRoll.IsSuccesfull(_target, _caster);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Test]
        [TestCase(AbilityEnum.Strength, true)]
        [TestCase(AbilityEnum.Intelligence, false)]
        public void Should_Use_Modifier_Of_The_Proper_Ability(AbilityEnum ability, bool expectedResult)
        {
            // Arrange
            _saveRoll.RolledResult = 8;
            _saveRoll.Ability = ability;

            // Act
            var result = _saveRoll.IsSuccesfull(_target, _caster);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Test]
        [TestCase(8, 12, false, false)]
        [TestCase(9, 12, false, true)]
        [TestCase(10, 12, false, true)]
        [TestCase(6, 12, true, false)]
        [TestCase(7, 12, true, true)]
        [TestCase(8, 12, true, true)]
        public void Should_Use_Modifier_With_Mastery_As_Bonus_To_The_RolledResult(int rolledResult, int abilityScore, bool mastery, bool expectedResult)
        {
            // Arrange
            var ability = _target.AbilityScores.First(x => x.Ability == AbilityEnum.Intelligence);
            ability.HasMastery = mastery;
            ability.Score = abilityScore;
            _saveRoll.RolledResult = rolledResult;

            // Act
            var result = _saveRoll.IsSuccesfull(_target, _caster);

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
