using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.HitPoint;
using FakeItEasy;
using DnDFightTool.Domain.Fight;
using FluentAssertions;
using NUnit.Framework;
using System;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;
using DnDFightTool.Business.DnDActions.HitPointActions.RegainTempHp;
using System.Threading.Tasks;

namespace DnDActionsTests.HitPointActionsTests.RegainTempHpTests;

[TestFixture]
internal class RegainTempHpCommandHandlerBase
{
    private IUndoableMediator _mediator = null!;
    private IFightContext _fightContext = null!;

    private Character _character = null!;
    
    private RegainTempHpCommand _command = null!;
    private RegainTempHpCommandHandler _commandHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>();
        _fightContext = A.Fake<IFightContext>();

        _character = new Character();
        _character.HitPoints = new HitPoints() { CurrentTempHps = 5 };

        _command = new RegainTempHpCommand(Guid.NewGuid(), 10) { CorrectedAmount = 10 };
        _commandHandler = new RegainTempHpCommandHandler(_mediator, _fightContext);

        A.CallTo(() => _fightContext.GetCharacterById(A<Guid>._))
            .Returns(_character);
    }

    private int _tempHps
    {
        get => _character.HitPoints.CurrentTempHps;
        set => _character.HitPoints.CurrentTempHps = value;
    }

    [TestFixture]
    private class ExecuteTests : RegainTempHpCommandHandlerBase
    {
        [Test]
        public async Task Should_Return_Success()
        {
            // Act 
            var response = await _commandHandler.Execute(_command);

            // Assert
            response.Status.Should().Be(RequestStatus.Success);
        }

        [Test]
        public void Should_Update_Hps()
        {
            // Arrange
            // Act
            _commandHandler.Execute(_command);

            // Assert
            _tempHps.Should().Be(_command.Amount);
        }

        [Test]
        [TestCase(10, 0)]
        [TestCase(5, 5)]
        [TestCase(0, 10)]
        public void Should_Set_CorrectedAmount(int hps, int correctedAmountExpected)
        {
            // Arrange
            _tempHps = hps;

            // Act
            _commandHandler.Execute(_command);

            // Assert
            _command.CorrectedAmount.Should().Be(correctedAmountExpected);
        }
    }

    [TestFixture]
    private class UndoTests : RegainTempHpCommandHandlerBase
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
            var startingHps = _tempHps;
            _command.CorrectedAmount = correctedAmount;

            // Act
            _commandHandler.Undo(_command);

            // Assert
            _tempHps.Should().Be(startingHps - correctedAmount);
        }
    }

    [TestFixture]
    private class RedoTests : RegainTempHpCommandHandlerBase
    {
        [Test]
        public void Should_Update_Hps()
        {
            // Arrange
            // Act
            _commandHandler.Execute(_command);

            // Assert
            _tempHps.Should().Be(_command.Amount);
        }
    }
}
