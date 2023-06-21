using DnDActions.HitPointActions.RegainHp;
using DnDEntities.Characters;
using DnDEntities.HitPoint;
using FakeItEasy;
using Fight;
using Fight.Characters;
using FluentAssertions;
using NUnit.Framework;
using System;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;

namespace DnDActionsTests.HitPointActionsTests.RegainHpTests;

[TestFixture]
internal class RegainHpCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IFightContext _fightContext = null!;

    private Character _character = null!;

    private RegainHpCommand _command = null!;
    private RegainHpCommandHandler _commandHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>();
        _fightContext = A.Fake<IFightContext>();

        _character = new Character();
        _character.HitPoints = new HitPoints() { CurrentHps = 12, MaxHps = 25 };

        _command = new RegainHpCommand(Guid.NewGuid(), 10) { CorrectedAmount = 10 };
        _commandHandler = new RegainHpCommandHandler(_mediator, _fightContext);

        A.CallTo(() => _fightContext.GetCharacterById(A<Guid>._))
            .Returns(_character);
    }


    private int _hps
    {
        get => _character.HitPoints.CurrentHps;
        set => _character.HitPoints.CurrentHps = value;
    }

    [TestFixture]
    private class ExecuteTests : RegainHpCommandHandlerTests
    {
        [Test]
        public void Should_Return_Success()
        {
            // Act 
            var response = _commandHandler.Execute(_command);

            // Assert
            response.Status.Should().Be(RequestStatus.Success);
        }

        [Test]
        public void Should_Update_Hps()
        {
            // Arrange
            var startingHps = _hps;

            // Act
            _commandHandler.Execute(_command);

            // Assert
            _hps.Should().Be(startingHps + _command.Amount);
        }

        [Test]
        public void Should_Not_Go_Higher_Than_Max_Hps()
        {
            // Arrange
            _hps = _character.HitPoints.MaxHps - 5;

            // Act
            _commandHandler.Execute(_command);

            // Assert
            _hps.Should().Be(_character.HitPoints.MaxHps);
        }

        [Test]
        [TestCase(20, 5)]
        [TestCase(5, 10)]
        public void Should_Set_CorrectAmount(int hps, int correctedAmountExpected)
        {
            // Arrange
            _hps = hps;

            // Act
            _commandHandler.Execute(_command);

            // Assert
            _command.CorrectedAmount.Should().Be(correctedAmountExpected);
        }
    }

    [TestFixture]
    private class UndoTests : RegainHpCommandHandlerTests
    {
        [Test]
        public void Should_Throw_InvalidOperationException_When_CorrectedAmount_Is_Null()
        {
            // Arrange
            _command.CorrectedAmount = null;

            // Act
            var undoing = () => _commandHandler.Undo(_command);

            // Assert
            undoing.Should().Throw<InvalidOperationException>();
        }

        [Test]
        [TestCase(3)]
        [TestCase(10)]
        public void Should_Update_Hps_With_CorrectedAmount(int correctedAmount)
        {
            // Arrange
            var startingHps = _hps;
            _command.CorrectedAmount = correctedAmount;

            // Act
            _commandHandler.Undo(_command);

            // Assert
            _hps.Should().Be(startingHps - correctedAmount);
        }
    }

    [TestFixture]
    private class RedoTests : RegainHpCommandHandlerTests
    {
        [Test]
        public void Should_Update_Hps()
        {
            // Arrange
            var startingHps = _hps;

            // Act
            _commandHandler.Execute(_command);

            // Assert
            _hps.Should().Be(startingHps + _command.Amount);
        }
    }
}
