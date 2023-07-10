﻿using DnDActions;
using FakeItEasy;
using Fight;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace DnDActionsTests;

[TestFixture]
public class TargetCasterCommandBaseTests
{
    IFightContext _fightContext = null!;
    TargetCasterCommandBase _command = null!;

    [SetUp]
    public void SetUp()
    {
        _fightContext = A.Fake<IFightContext>();
        _command = new TargetCasterCommandBase(Guid.NewGuid(), Guid.NewGuid());

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

    [Test]
    public void Should_Throw_InvalidOperationException_When_Cannot_Get_Valid_Caster()
    {
        // Arrange
        var gettingHitPoints = () => _command.GetCaster(_fightContext);

        // Act & Assert
        gettingHitPoints.Should().Throw<InvalidOperationException>();
    }
}
