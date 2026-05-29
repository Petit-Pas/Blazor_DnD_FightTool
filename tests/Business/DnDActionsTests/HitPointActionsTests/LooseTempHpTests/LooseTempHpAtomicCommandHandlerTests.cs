using DnDFightTool.Business.DnDActions.HitPointActions.LooseTempHp;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.HitPoint;
using FakeItEasy;
using DnDFightTool.Domain.Fight;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;
using DomainTestsUtilities.Extensions;
using DnDFightTool.Domain.Fight.Characters;

namespace DnDActionsTests.HitPointActionsTests.LooseTempHpTests;

[TestFixture]
internal class LooseTempHpAtomicCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IFightContext _fightContext = null!;

    private FightingCharacter _character = null!;

    private LooseTempHpAtomicCommand _command = null!;
    private LooseTempHpAtomicCommandHandler _commandHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>(options => options.Strict().Implements<ISubCommandDispatcher>());
        _fightContext = A.Fake<IFightContext>(options => options.Strict());

        _character = new Character
        {
            HitPoints = new HitPoints() { CurrentTempHps = 12 }
        }.AsFighter();

        _command = new LooseTempHpAtomicCommand(Guid.NewGuid(), 10) { CorrectedAmount = 10 };
        _commandHandler = new LooseTempHpAtomicCommandHandler(_mediator, _fightContext);

        A.CallTo(() => _fightContext[A<Guid>._])
            .Returns(_character);
        A.CallTo(() => _fightContext.NotifyFighterUpdated(A<Guid>._)).DoesNothing();
    }

    private int _tempHps
    {
        get => _character.HitPoints.CurrentTempHps;
        set => _character.HitPoints.CurrentTempHps = value;
    }

    [TestFixture]
    private class ExecuteTests : LooseTempHpAtomicCommandHandlerTests
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
            var startingHps = _tempHps;

            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            _tempHps.Should().Be(startingHps - _command.Amount);
        }

        [Test]
        public async Task Should_Not_Go_Lower_Than_Zero_Hps()
        {
            // Arrange
            _tempHps = 5;

            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            _tempHps.Should().Be(0);
        }

        [Test]
        [TestCase(20, 10)]
        [TestCase(5, 5)]
        public async Task Should_Set_CorrectedAmount(int hps, int correctedAmountExpected)
        {
            // Arrange
            _tempHps = hps;

            // Act
            var response = await _commandHandler.ExecuteAsync(_command);

            // Assert
            _command.CorrectedAmount.Should().Be(correctedAmountExpected);
            response.Response.Should().Be(correctedAmountExpected);
        }

    }

    [TestFixture]
    private class UndoTests : LooseTempHpAtomicCommandHandlerTests
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
            var startingHps = _tempHps;
            _command.CorrectedAmount = correctedAmount;

            // Act
            await _commandHandler.UndoAsync(_command);

            // Assert
            _tempHps.Should().Be(startingHps + correctedAmount);
        }
    }

    [TestFixture]
    private class RedoTests : LooseTempHpAtomicCommandHandlerTests
    {
        [Test]
        public async Task Should_Update_Hps()
        {
            // Arrange
            var startingHps = _tempHps;

            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            _tempHps.Should().Be(startingHps - _command.Amount);
        }
    }
}
