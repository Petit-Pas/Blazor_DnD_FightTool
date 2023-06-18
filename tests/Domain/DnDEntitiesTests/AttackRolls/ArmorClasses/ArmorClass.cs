using DnDEntities.AttackRolls.ArmorClasses;
using FluentAssertions;
using NUnit.Framework;

namespace DnDEntitiesTests.AttackRolls.ArmorClasses;

[TestFixture]
public class ArmorClassTests
{
    private ArmorClass _armorClass = null!;

    [SetUp]
    public void SetUp()
    {
        _armorClass = new ArmorClass()
        {
            BaseArmorClass = 10,
            ShieldArmorClass = 2,
            HasShieldEquipped = false,
        };
    }

    [Test]
    public void Shield_Armor_Class_Should_Not_Change_Effective_Ac_When_Not_Equiped()
    {
        // Assert
        _armorClass.EffectiveAC.Should().Be(10);
    }

    [Test]
    public void When_Shield_Is_Equiped_Effective_Ac_Should_Be_Increased()
    {
        // Arrange
        _armorClass.HasShieldEquipped = true;

        // Assert
        _armorClass.EffectiveAC.Should().Be(12);
    }
}
