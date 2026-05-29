using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DnDFightTool.Business.DnDActions.TurnActions.StartCombat;
using DnDFightTool.Domain.Fight;
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
internal class StartCombatCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private ICombatTurnService _combatTurnService = null!;
    private IFightContext _fightContext = null!;
    private StartCombatCommand _command = null!;
    private StartCombatCommandHandler _handler = null!;

    private FightingCharacter _fighter1 = null!;
    private FightingCharacter _fighter2 = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>(options => options.Strict().Implements<ISubCommandDispatcher>());
        _combatTurnService = A.Fake<ICombatTurnService>(options => options.Strict());
        _fightContext = A.Fake<IFightContext>(options => options.Strict());
        _command = new StartCombatCommand();
        _handler = new StartCombatCommandHandler(_mediator, _combatTurnService, _fightContext);

        _fighter1 = CharacterFactory.BuildMonster(name: "Goblin").AsFighter();
        _fighter2 = CharacterFactory.BuildMonster(name: "Orc").AsFighter();

        A.CallTo(() => _fightContext.Fighters).Returns([_fighter1, _fighter2]);
        A.CallTo(() => _combatTurnService.IsStarted).Returns(false);
        A.CallTo(() => _combatTurnService.Initialize(A<IEnumerable<IFightingCharacter>>._)).DoesNothing();
    }

    [TestFixture]
    internal class ExecuteTests : StartCombatCommandHandlerTests
    {
        [Test]
        public async Task Should_Return_Success()
        {
            // Act
            var response = await _handler.ExecuteAsync(_command);

            // Assert
            response.Status.Should().Be(RequestStatus.Success);
        }

        [Test]
        public async Task Should_Call_Initialize_On_CombatTurnService()
        {
            // Act
            await _handler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _combatTurnService.Initialize(A<IEnumerable<IFightingCharacter>>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Should_Throw_When_Already_Started()
        {
            // Arrange
            A.CallTo(() => _combatTurnService.IsStarted).Returns(true);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _handler.ExecuteAsync(_command));
        }
    }

    [TestFixture]
    internal class UndoTests : StartCombatCommandHandlerTests
    {
        [Test]
        public async Task Should_Reset_By_Initializing_With_Empty_List()
        {
            // Act
            await _handler.UndoAsync(_command);

            // Assert
            A.CallTo(() => _combatTurnService.Initialize(A<IEnumerable<IFightingCharacter>>.That.IsEmpty()))
                .MustHaveHappenedOnceExactly();
        }
    }

    [TestFixture]
    internal class RedoTests : StartCombatCommandHandlerTests
    {
        [Test]
        public async Task Should_Reinitialize_With_Fighters()
        {
            // Act
            await _handler.RedoAsync(_command);

            // Assert
            A.CallTo(() => _combatTurnService.Initialize(A<IEnumerable<IFightingCharacter>>.Ignored))
                .MustHaveHappenedOnceExactly();
        }
    }
}
