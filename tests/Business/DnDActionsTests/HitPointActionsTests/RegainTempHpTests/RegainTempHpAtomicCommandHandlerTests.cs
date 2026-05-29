using System;
using System.Threading.Tasks;
using DnDFightTool.Business.DnDActions.HitPointActions.RegainTempHp;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.HitPoint;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using DomainTestsUtilities.Extensions;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;

namespace DnDActionsTests.HitPointActionsTests.RegainTempHpTests;

[TestFixture]
internal class RegainTempHpAtomicCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IFightContext _fightContext = null!;

    private FightingCharacter _character = null!;
    
    private RegainTempHpAtomicCommand _command = null!;
    private RegainTempHpAtomicCommandHandler _commandHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>(options => options.Strict().Implements<ISubCommandDispatcher>());
        _fightContext = A.Fake<IFightContext>(options => options.Strict());

        _character = new Character
        {
            HitPoints = new HitPoints() { CurrentTempHps = 5 }
        }.AsFighter();

        _command = new RegainTempHpAtomicCommand(Guid.NewGuid(), 10) { CorrectedAmount = 10 };
        _commandHandler = new RegainTempHpAtomicCommandHandler(_mediator, _fightContext);

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
    private class ExecuteTests : RegainTempHpAtomicCommandHandlerTests
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
            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            _tempHps.Should().Be(_command.Amount);
        }

        [Test]
        [TestCase(10, 0)]
        [TestCase(5, 5)]
        [TestCase(0, 10)]
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
    private class UndoTests : RegainTempHpAtomicCommandHandlerTests
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
            _tempHps.Should().Be(startingHps - correctedAmount);
        }
    }

    [TestFixture]
    private class RedoTests : RegainTempHpAtomicCommandHandlerTests
    {
        [Test]
        public async Task Should_Update_Hps()
        {
            // Arrange
            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            _tempHps.Should().Be(_command.Amount);
        }
    }
}
