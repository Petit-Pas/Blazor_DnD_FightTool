using System;
using System.Threading.Tasks;
using DnDFightTool.Business.DnDActions.FightActions.RemoveFromFight;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using DomainTestsUtilities.Extensions;
using DomainTestsUtilities.Factories.Characters;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;

namespace DnDActionsTests.FightActions;

[TestFixture]
internal class RemoveFromFightAtomicCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IFightContext _fightContext = null!;

    private RemoveFromFightAtomicCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>(options => options.Strict().Implements<ISubCommandDispatcher>());
        _fightContext = A.Fake<IFightContext>(options => options.Strict());

        _handler = new RemoveFromFightAtomicCommandHandler(_mediator, _fightContext);
    }

    [TestFixture]
    internal class ExecuteTests : RemoveFromFightAtomicCommandHandlerTests
    {
        [Test]
        public async Task Should_Return_Success()
        {
            // Arrange
            var fighter = CharacterFactory.BuildPlayer(name: "Aragorn").AsFighter();
            A.CallTo(() => _fightContext[fighter.Id]).Returns(fighter);
            A.CallTo(() => _fightContext.Remove(fighter)).DoesNothing();
            var command = new RemoveFromFightAtomicCommand(fighter.Id);

            // Act
            var response = await _handler.ExecuteAsync(command);

            // Assert
            response.Status.Should().Be(RequestStatus.Success);
        }

        [Test]
        public async Task Should_Remove_Fighter_From_FightContext()
        {
            // Arrange
            var fighter = CharacterFactory.BuildPlayer(name: "Aragorn").AsFighter();
            A.CallTo(() => _fightContext[fighter.Id]).Returns(fighter);
            A.CallTo(() => _fightContext.Remove(fighter)).DoesNothing();
            var command = new RemoveFromFightAtomicCommand(fighter.Id);

            // Act
            await _handler.ExecuteAsync(command);

            // Assert
            A.CallTo(() => _fightContext.Remove(fighter))
                .MustHaveHappenedOnceExactly();
        }
    }

    [TestFixture]
    internal class UndoTests : RemoveFromFightAtomicCommandHandlerTests
    {
        [Test]
        public async Task Should_Restore_Fighter_From_Stash()
        {
            // Arrange
            var fighterId = Guid.NewGuid();
            A.CallTo(() => _fightContext.Restore(fighterId)).DoesNothing();
            var command = new RemoveFromFightAtomicCommand(fighterId);

            // Act
            await _handler.UndoAsync(command);

            // Assert
            A.CallTo(() => _fightContext.Restore(fighterId))
                .MustHaveHappenedOnceExactly();
        }
    }
}
