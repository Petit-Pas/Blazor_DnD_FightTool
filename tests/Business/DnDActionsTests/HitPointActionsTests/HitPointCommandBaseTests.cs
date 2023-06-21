﻿using DnDActions.HitPointActions;
using FakeItEasy;
using Fight;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace DnDActionsTests.HitPointActionsTests;

[TestFixture]
public class HitPointCommandBaseTests
{
    IFightContext _fightContext = null!;
    HitPointCommandBase _command = null!;

    [SetUp]
    public void SetUp()
    {
        _fightContext = A.Fake<IFightContext>();
        _command = new HitPointCommandBase(Guid.NewGuid());

        A.CallTo(() => _fightContext.GetCharacterById(A<Guid>._))
            .Returns(null);
    }

    [Test]
    public void Should_Throw_InvalidOperationException_When_Cannot_Get_Valid_HitPoints()
    {
        // Arrange
        var gettingHitPoints = () => _command.GetHitPoints(_fightContext);

        // Act & Assert
        gettingHitPoints.Should().Throw<InvalidOperationException>();
    }
}
