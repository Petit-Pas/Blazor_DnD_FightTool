using DnDActions.HitPointActions.LooseHp;
using DnDEntities.Characters;
using DnDEntities.HitPoint;
using FakeItEasy;
using Fight;
using Fight.Characters;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;

namespace DnDActionsTests.HitPointActionsTests.LooseHpTests;

[TestFixture]
internal class LooseHpCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IFightContext _fightContext = null!;

    private Character _character = null!;

    private LooseHpCommand _command = null!;
    private LooseHpCommandHandler _commandHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>();
        _fightContext = A.Fake<IFightContext>();

        _character = new Character();
        _character.HitPoints = new HitPoints() { MaxHps = 25, CurrentHps = 12 };

        _command = new LooseHpCommand(Guid.NewGuid(), 10) { CorrectedAmount = 10 };
        _commandHandler = new LooseHpCommandHandler(_mediator, _fightContext);

        A.CallTo(() => _fightContext.GetCharacterById(A<Guid>._))
            .Returns(_character);
    }

    private int _hps 
    { 
        get => _character.HitPoints.CurrentHps; 
        set => _character.HitPoints.CurrentHps = value; 
    }

    [TestFixture]
    private class ExecuteTests : LooseHpCommandHandlerTests
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
            var startingHps = _hps;

            // Act
            _commandHandler.Execute(_command);

            // Assert
            _hps.Should().Be(startingHps - _command.Amount);
        }

        [Test]
        public void Should_Not_Go_Lower_Than_Zero_Hps()
        {
            // Arrange
            _hps = 5;

            // Act
            _commandHandler.Execute(_command);

            // Assert
            _hps.Should().Be(0);
        }

        [Test]
        [TestCase(20, 10)]
        [TestCase(5, 5)]
        public void Should_Set_CorrectedAmount(int hps, int correctedAmountExpected)
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
    private class UndoTests : LooseHpCommandHandlerTests
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
            _hps.Should().Be(startingHps + correctedAmount);
        }
    }

    [TestFixture]
    private class RedoTests : LooseHpCommandHandlerTests
    {
        [Test]
        public void Should_Update_Hps()
        {
            // Arrange
            var startingHps = _hps;

            // Act
            _commandHandler.Execute(_command);

            // Assert
            _hps.Should().Be(startingHps - _command.Amount);
        }
    }

    [TestFixture]
    public class FullCycleTest : LooseHpCommandHandlerTests
    {
        [Test]
        public void Redo_Should_Do_The_Same_As_Execute()
        {
            // Arrange
            _commandHandler.Execute(_command);
            var remainingHps = _hps;
            _commandHandler.Undo(_command);

            // Act
            _commandHandler.Redo(_command);

            // Assert
            _hps.Should().Be(remainingHps);
        }
    }
}
