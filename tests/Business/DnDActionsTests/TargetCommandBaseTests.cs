using DnDActions;
using DnDActions.HitPointActions;
using FakeItEasy;
using Fight;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace DnDActionsTests;

[TestFixture]
public class TargetCommandBaseTests
{
    IFightContext _fightContext = null!;
    TargetCommandBase _command = null!;

    [SetUp]
    public void SetUp()
    {
        _fightContext = A.Fake<IFightContext>();
        _command = new TargetCommandBase(Guid.NewGuid());

        A.CallTo(() => _fightContext.GetCharacterById(A<Guid>._))
            .Returns(null);
    }

    [Test]
    public void Should_Throw_InvalidOperationException_When_Cannot_Get_Valid_Target()
    {
        // Arrange
        var gettingHitPoints = () => _command.GetTarget(_fightContext);

        // Act & Assert
        gettingHitPoints.Should().Throw<InvalidOperationException>();
    }
}
