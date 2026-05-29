using DnDFightTool.Business.DnDActions.HitPointActions.RegainHp;
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

namespace DnDActionsTests.HitPointActionsTests.RegainHpTests;

[TestFixture]
internal class RegainHpAtomicCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IFightContext _fightContext = null!;

    private FightingCharacter _character = null!;

    private RegainHpAtomicCommand _command = null!;
    private RegainHpAtomicCommandHandler _commandHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>(options => options.Strict().Implements<ISubCommandDispatcher>());
        _fightContext = A.Fake<IFightContext>(options => options.Strict());

        _character = new Character
        {
            HitPoints = new HitPoints() { CurrentHps = 12, MaxHps = 25 }
        }.AsFighter();

        _command = new RegainHpAtomicCommand(Guid.NewGuid(), 10) { CorrectedAmount = 10 };
        _commandHandler = new RegainHpAtomicCommandHandler(_mediator, _fightContext);

        A.CallTo(() => _fightContext[A<Guid>._])
            .Returns(_character);
        A.CallTo(() => _fightContext.NotifyFighterUpdated(A<Guid>._)).DoesNothing();
    }


    private int _hps
    {
        get => _character.HitPoints.CurrentHps;
        set => _character.HitPoints.CurrentHps = value;
    }

    [TestFixture]
    private class ExecuteTests : RegainHpAtomicCommandHandlerTests
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
            _hps.Should().Be(startingHps + _command.Amount);
        }

        [Test]
        public async Task Should_Not_Go_Higher_Than_Max_Hps()
        {
            // Arrange
            _hps = _character.HitPoints.MaxHps - 5;

            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            _hps.Should().Be(_character.HitPoints.MaxHps);
        }

        [Test]
        [TestCase(20, 5)]
        [TestCase(5, 10)]
        public async Task Should_Set_CorrectAmount(int hps, int correctedAmountExpected)
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
    private class UndoTests : RegainHpAtomicCommandHandlerTests
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
            _hps.Should().Be(startingHps - correctedAmount);
        }
    }

    [TestFixture]
    private class RedoTests : RegainHpAtomicCommandHandlerTests
    {
        [Test]
        public async Task Should_Update_Hps()
        {
            // Arrange
            var startingHps = _hps;

            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            _hps.Should().Be(startingHps + _command.Amount);
        }
    }
}
