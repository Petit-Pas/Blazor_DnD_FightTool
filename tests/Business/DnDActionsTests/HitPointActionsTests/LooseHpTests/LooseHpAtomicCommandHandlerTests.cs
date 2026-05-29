using DnDFightTool.Business.DnDActions.HitPointActions.LooseHp;
using DnDFightTool.Domain.CharacterSheet.HitPoint;
using FakeItEasy;
using DnDFightTool.Domain.Fight;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;
using DnDFightTool.Domain.Fight.Characters;

namespace DnDActionsTests.HitPointActionsTests.LooseHpTests;

[TestFixture]
internal class LooseHpAtomicCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IFightContext _fightContext = null!;

    private IFightingCharacter _character = null!;
    private HitPoints _hitPoints = null!;

    private LooseHpAtomicCommand _command = null!;
    private LooseHpAtomicCommandHandler _commandHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>(options => options.Strict().Implements<ISubCommandDispatcher>());
        _fightContext = A.Fake<IFightContext>(options => options.Strict());

        _hitPoints = new HitPoints { MaxHps = 25, CurrentHps = 12 };
        _character = A.Fake<IFightingCharacter>(options => options.Strict());
        A.CallTo(() => _character.HitPoints).Returns(_hitPoints);

        _command = new LooseHpAtomicCommand(Guid.NewGuid(), 10) { CorrectedAmount = 10 };
        _commandHandler = new LooseHpAtomicCommandHandler(_mediator, _fightContext);

        A.CallTo(() => _fightContext[A<Guid>._])
            .Returns(_character);
        A.CallTo(() => _fightContext.NotifyFighterUpdated(A<Guid>._)).DoesNothing();
    }

    private int _hps 
    { 
        get => _hitPoints.CurrentHps; 
        set => _hitPoints.CurrentHps = value; 
    }

    [TestFixture]
    private class ExecuteTests : LooseHpAtomicCommandHandlerTests
    {
        [Test]
        public async Task Should_Return_Success()
        {
            // Act 
            var response = await _commandHandler.ExecuteAsync(_command);

            // Assert
            response.Status.Should().Be(RequestStatus.Success);
        }

        [Test]
        public async Task Should_Update_Hps()
        {
            // Arrange
            var startingHps = _hps;

            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            _hps.Should().Be(startingHps - _command.Amount);
        }

        [Test]
        public async Task Should_Not_Go_Lower_Than_Zero_Hps()
        {
            // Arrange
            _hps = 5;

            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            _hps.Should().Be(0);
        }

        [Test]
        [TestCase(20, 10)]
        [TestCase(5, 5)]
        public async Task Should_Set_CorrectedAmount(int hps, int correctedAmountExpected)
        {
            // Arrange
            _hps = hps;

            // Act
            var response = await _commandHandler.ExecuteAsync(_command);

            // Assert
            _command.CorrectedAmount.Should().Be(correctedAmountExpected);
            response.Response.Should().Be(correctedAmountExpected);
        }

    }

    [TestFixture]
    private class UndoTests : LooseHpAtomicCommandHandlerTests
    {
        [Test]
        public async Task Should_Throw_InvalidOperationException_When_CorrectedAmount_Is_Null()
        {
            // Arrange
            _command.CorrectedAmount = null;

            // Act
            var undoing = async () => await _commandHandler.UndoAsync(_command);

            // Assert
            await undoing.Should().ThrowAsync<InvalidOperationException>();
        }

        [Test]
        [TestCase(3)]
        [TestCase(10)]
        public async Task Should_Update_Hps_With_CorrectedAmount(int correctedAmount)
        {
            // Arrange
            var startingHps = _hps;
            _command.CorrectedAmount = correctedAmount;

            // Act
            await _commandHandler.UndoAsync(_command);

            // Assert
            _hps.Should().Be(startingHps + correctedAmount);
        }
    }

    [TestFixture]
    private class RedoTests : LooseHpAtomicCommandHandlerTests
    {
        [Test]
        public async Task Should_Update_Hps()
        {
            // Arrange
            var startingHps = _hps;

            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            _hps.Should().Be(startingHps - _command.Amount);
        }
    }

    [TestFixture]
    public class FullCycleTest : LooseHpAtomicCommandHandlerTests
    {
        [Test]
        public async Task Redo_Should_Do_The_Same_As_Execute()
        {
            // Arrange
            await _commandHandler.ExecuteAsync(_command);
            var remainingHps = _hps;
            await _commandHandler.UndoAsync(_command);

            // Act
            await _commandHandler.RedoAsync(_command);

            // Assert
            _hps.Should().Be(remainingHps);
        }
    }
}
