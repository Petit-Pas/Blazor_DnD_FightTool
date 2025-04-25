using System.Linq;
using DnDFightTool.Domain.DnDEntities.Damage;
using DnDFightTool.Domain.DnDEntities.DamageAffinities;
using FluentAssertions;
using NUnit.Framework;

namespace DnDEntitiesTests.DamageAffinities;

[TestFixture]
public class DamageAffinitiesCollectionTests
{
    private DamageAffinitiesCollection _affinities = null!;

    [SetUp]
    public void SetUp()
    {
        _affinities = new DamageAffinitiesCollection(true);

        _affinities.First(x => x.Type == DamageTypeEnum.Cold).Affinity = DamageAffinityEnum.Immune;
        _affinities.First(x => x.Type == DamageTypeEnum.Thunder).Affinity = DamageAffinityEnum.Resistant;
        _affinities.First(x => x.Type == DamageTypeEnum.Fire).Affinity = DamageAffinityEnum.Normal;
        _affinities.First(x => x.Type == DamageTypeEnum.Poison).Affinity = DamageAffinityEnum.Weak;
    }

    [TestFixture]
    public class GetDamageFactorForTests : DamageAffinitiesCollectionTests
    {
        [Test]
        [TestCase(DamageTypeEnum.Cold, 0)]
        [TestCase(DamageTypeEnum.Thunder, 0.5)]
        [TestCase(DamageTypeEnum.Fire, 1)]
        [TestCase(DamageTypeEnum.Poison, 2)]
        public void Returns_Appropriate_Damage_Factor(DamageTypeEnum damageType, double damageFactor) 
        {
            //Act
            double factor = _affinities.GetDamageFactorFor(damageType);

            // Assert
            factor.Should().Be(damageFactor);
        }
    }
}
