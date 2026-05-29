using System;
using System.Threading.Tasks;
using DnDFightTool.Business.DnDActions.TurnActions.SetCurrentFighter;
using DnDFightTool.Domain.Fight.Characters;
using DnDFightTool.Domain.Fight.TurnTracking;
using DomainTestsUtilities.Extensions;
using DomainTestsUtilities.Factories.Characters;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;

namespace DnDActionsTests.TurnActions;

[TestFixture]
internal class SetCurrentFighterCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private ICombatTurnService _combatTurnService = null!;
    private FightingCharacter _fighter1 = null!;
    private FightingCharacter _fighter2 = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>(options => options.Strict().Implements<ISubCommandDispatcher>());
        _combatTurnService = A.Fake<ICombatTurnService>(options => options.Strict());

        _fighter1 = CharacterFactory.BuildMonster(name: "Goblin").AsFighter();
        _fighter2 = CharacterFactory.BuildMonster(name: "Orc").AsFighter();

        A.CallTo(() => _combatTurnService.CurrentTurnFighter).Returns((FightingCharacter?)null);
        A.CallTo(() => _combatTurnService.SetCurrentTurnFighter(A<Guid?>._)).DoesNothing();
    }

    [TestFixture]
    internal class ExecuteTests : SetCurrentFighterCommandHandlerTests
    {
        [Test]
        public async Task Should_Return_Success()
        {
            // Arrange
            var command = new SetCurrentFighterCommand(_fighter1.Id);
            var handler = new SetCurrentFighterCommandHandler(_mediator, _combatTurnService);

            // Act
            var response = await handler.ExecuteAsync(command);

            // Assert
            response.Status.Should().Be(RequestStatus.Success);
        }

        [Test]
        public async Task Should_Set_CurrentTurnFighter()
        {
            // Arrange
            var command = new SetCurrentFighterCommand(_fighter1.Id);
            var handler = new SetCurrentFighterCommandHandler(_mediator, _combatTurnService);

            // Act
            await handler.ExecuteAsync(command);

            // Assert
            A.CallTo(() => _combatTurnService.SetCurrentTurnFighter(_fighter1.Id))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_Store_PreviousFighterId()
        {
            // Arrange
            A.CallTo(() => _combatTurnService.CurrentTurnFighter).Returns(_fighter2);
            var command = new SetCurrentFighterCommand(_fighter1.Id);
            var handler = new SetCurrentFighterCommandHandler(_mediator, _combatTurnService);

            // Act
            await handler.ExecuteAsync(command);

            // Assert
            command.PreviousFighterId.Should().Be(_fighter2.Id);
        }

        [Test]
        public async Task Should_Store_Null_PreviousFighterId_When_NoCurrent()
        {
            // Arrange
            A.CallTo(() => _combatTurnService.CurrentTurnFighter).Returns((FightingCharacter?)null);
            var command = new SetCurrentFighterCommand(_fighter1.Id);
            var handler = new SetCurrentFighterCommandHandler(_mediator, _combatTurnService);

            // Act
            await handler.ExecuteAsync(command);

            // Assert
            command.PreviousFighterId.Should().BeNull();
        }

        [Test]
        public async Task Should_Clear_CurrentFighter_When_Null_FighterId()
        {
            // Arrange
            var command = new SetCurrentFighterCommand(null);
            var handler = new SetCurrentFighterCommandHandler(_mediator, _combatTurnService);

            // Act
            var response = await handler.ExecuteAsync(command);

            // Assert
            response.Status.Should().Be(RequestStatus.Success);
            A.CallTo(() => _combatTurnService.SetCurrentTurnFighter(null))
                .MustHaveHappenedOnceExactly();
        }
    }

    [TestFixture]
    internal class UndoTests : SetCurrentFighterCommandHandlerTests
    {
        [Test]
        public async Task Should_Restore_PreviousFighter()
        {
            // Arrange
            var command = new SetCurrentFighterCommand(_fighter1.Id) { PreviousFighterId = _fighter2.Id };
            var handler = new SetCurrentFighterCommandHandler(_mediator, _combatTurnService);

            // Act
            await handler.UndoAsync(command);

            // Assert
            A.CallTo(() => _combatTurnService.SetCurrentTurnFighter(_fighter2.Id))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_Clear_Fighter_When_No_Previous()
        {
            // Arrange
            var command = new SetCurrentFighterCommand(_fighter1.Id) { PreviousFighterId = null };
            var handler = new SetCurrentFighterCommandHandler(_mediator, _combatTurnService);

            // Act
            await handler.UndoAsync(command);

            // Assert
            A.CallTo(() => _combatTurnService.SetCurrentTurnFighter(null))
                .MustHaveHappenedOnceExactly();
        }
    }

    [TestFixture]
    internal class RedoTests : SetCurrentFighterCommandHandlerTests
    {
        [Test]
        public async Task Should_Reapply_FighterId()
        {
            // Arrange
            var command = new SetCurrentFighterCommand(_fighter1.Id);
            var handler = new SetCurrentFighterCommandHandler(_mediator, _combatTurnService);

            // Act
            await handler.RedoAsync(command);

            // Assert
            A.CallTo(() => _combatTurnService.SetCurrentTurnFighter(_fighter1.Id))
                .MustHaveHappenedOnceExactly();
        }
    }
}
