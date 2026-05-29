using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DnDFightTool.Business.DnDActions.FightActions.AddToFight;
using DnDFightTool.Business.DnDActions.LogActions.WriteLog;
using DnDFightTool.Business.DnDQueries.FightQueries;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using DomainTestsUtilities.Extensions;
using DomainTestsUtilities.Factories.Characters;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using UndoableMediator.Mediators;
using UndoableMediator.Queries;
using UndoableMediator.Requests;
using UndoableMediator.Commands;

namespace DnDActionsTests.FightActions;

[TestFixture]
internal class AddToFightCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IFightContext _fightContext = null!;
    private ICharacterRepository _characterRepository = null!;

    private AddToFightCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>(options => options.Strict().Implements<ISubCommandDispatcher>());
        _fightContext = A.Fake<IFightContext>(options => options.Strict());
        _characterRepository = A.Fake<ICharacterRepository>(options => options.Strict());

        _handler = new AddToFightCommandHandler(_mediator, _fightContext, _characterRepository);
    }

    private void SetupPromptReturns(int initiative)
    {
        A.CallTo(() => _mediator.QueryAsync(A<InitiativeRollQuery>._))
            .Returns(QueryResponse<int>.Success(initiative));
    }

    private void SetupPromptCanceled()
    {
        A.CallTo(() => _mediator.QueryAsync(A<InitiativeRollQuery>._))
            .Returns(QueryResponse<int>.Canceled(0));
    }

    private void SetupAddCreatesNewFighter(Character character, FightingCharacter newFighter)
    {
        var existingIds = new HashSet<Guid>();

        A.CallTo(() => _fightContext.Fighters)
            .ReturnsLazily(() => existingIds.Count == 0
                ? []
                : [newFighter]);

        A.CallTo(() => _fightContext.Add(character, A<int>._))
            .Invokes(() => existingIds.Add(newFighter.Id))
            .Returns(newFighter);

        A.CallTo(() => _fightContext[newFighter.Id])
            .Returns(newFighter);

        A.CallTo(() => _mediator.SendAsSubCommandAsync(
                A<AddToFightAtomicCommand>.That.Matches(x => x.SourceCharacterId == character.Id),
                A<AddToFightCommand>._))
            .Returns(CommandResponse.Success<Guid>(newFighter.Id));

        A.CallTo(() => _mediator.SendAsSubCommandAsync(
                A<WriteLogCommand>._,
                A<AddToFightCommand>._))
            .Returns(CommandResponse.Success());
    }

    [TestFixture]
    internal class ExecuteTests : AddToFightCommandHandlerTests
    {
        [Test]
        public async Task Should_Return_Success_When_Player_Added_With_Prompted_Initiative()
        {
            // Arrange
            var player = CharacterFactory.BuildPlayer(name: "Aragorn");
            var fighter = player.AsFighter();
            A.CallTo(() => _characterRepository.GetCharacterById(player.Id)).Returns(player);
            SetupPromptReturns(14);
            SetupAddCreatesNewFighter(player, fighter);

            var command = new AddToFightCommand(player.Id);

            // Act
            var response = await _handler.ExecuteAsync(command);

            // Assert
            response.Status.Should().Be(RequestStatus.Success);
            command.AddedFighterId.Should().Be(fighter.Id);
            command.InitiativeRoll.Should().Be(14);
        }

        [Test]
        public async Task Should_Prompt_For_Initiative_When_First_Monster_Of_Kind()
        {
            // Arrange
            var monster = CharacterFactory.BuildMonster(name: "Goblin");
            var fighter = monster.AsFighter();
            A.CallTo(() => _characterRepository.GetCharacterById(monster.Id)).Returns(monster);
            A.CallTo(() => _fightContext.Fighters).Returns([]);
            SetupPromptReturns(8);
            SetupAddCreatesNewFighter(monster, fighter);

            var command = new AddToFightCommand(monster.Id);

            // Act
            var response = await _handler.ExecuteAsync(command);

            // Assert
            response.Status.Should().Be(RequestStatus.Success);
            A.CallTo(() => _mediator.QueryAsync(A<InitiativeRollQuery>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_Inherit_Initiative_When_SameKind_Monster_Already_In_Fight()
        {
            // Arrange
            var monster = CharacterFactory.BuildMonster(name: "Goblin");
            var existingFighter = monster.AsFighter();
            existingFighter.InitiativeRoll = 12;
            var newFighter = CharacterFactory.BuildMonster(name: "Goblin 2")
                .AsFighter(originalCharacterId: monster.Id);

            A.CallTo(() => _characterRepository.GetCharacterById(monster.Id)).Returns(monster);

            // First call: existing fighters (before Add); second call: includes new fighter
            A.CallTo(() => _fightContext.Fighters)
                .ReturnsNextFromSequence(
                    new[] { existingFighter },
                    new[] { existingFighter },
                    new[] { existingFighter, newFighter });

            A.CallTo(() => _fightContext[newFighter.Id]).Returns(newFighter);

            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                    A<AddToFightAtomicCommand>.That.Matches(x => x.SourceCharacterId == monster.Id),
                    A<AddToFightCommand>._))
                .Returns(CommandResponse.Success<Guid>(newFighter.Id));

            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                    A<WriteLogCommand>._,
                    A<AddToFightCommand>._))
                .Returns(CommandResponse.Success());

            var command = new AddToFightCommand(monster.Id);

            // Act
            var response = await _handler.ExecuteAsync(command);

            // Assert
            response.Status.Should().Be(RequestStatus.Success);
            command.InitiativeRoll.Should().Be(12);
            A.CallTo(() => _mediator.QueryAsync(A<InitiativeRollQuery>._))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Should_Prompt_When_DifferentKind_Monster_In_Fight()
        {
            // Arrange
            var goblin = CharacterFactory.BuildMonster(name: "Goblin");
            var orc = CharacterFactory.BuildMonster(name: "Orc");
            var goblinFighter = goblin.AsFighter();
            goblinFighter.InitiativeRoll = 10;
            var orcFighter = orc.AsFighter();

            A.CallTo(() => _characterRepository.GetCharacterById(orc.Id)).Returns(orc);
            // Call 1: same-kind check (no orc found, only goblin)
            // Call 2: fightersBefore snapshot
            // Call 3: after Add - includes new orcFighter
            A.CallTo(() => _fightContext.Fighters)
                .ReturnsNextFromSequence(
                    new[] { goblinFighter },
                    new[] { goblinFighter },
                    new[] { goblinFighter, orcFighter });
            A.CallTo(() => _fightContext[orcFighter.Id]).Returns(orcFighter);
            SetupPromptReturns(5);

            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                    A<AddToFightAtomicCommand>.That.Matches(x => x.SourceCharacterId == orc.Id),
                    A<AddToFightCommand>._))
                .Returns(CommandResponse.Success<Guid>(orcFighter.Id));

            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                    A<WriteLogCommand>._,
                    A<AddToFightCommand>._))
                .Returns(CommandResponse.Success());

            var command = new AddToFightCommand(orc.Id);

            // Act
            var response = await _handler.ExecuteAsync(command);

            // Assert
            response.Status.Should().Be(RequestStatus.Success);
            A.CallTo(() => _mediator.QueryAsync(A<InitiativeRollQuery>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_Return_Canceled_When_Prompt_Is_Cancelled()
        {
            // Arrange
            var player = CharacterFactory.BuildPlayer(name: "Legolas");
            A.CallTo(() => _characterRepository.GetCharacterById(player.Id)).Returns(player);
            A.CallTo(() => _fightContext.Fighters).Returns([]);
            SetupPromptCanceled();

            var command = new AddToFightCommand(player.Id);

            // Act
            var response = await _handler.ExecuteAsync(command);

            // Assert
            response.Status.Should().Be(RequestStatus.Canceled);
            A.CallTo(() => _fightContext.Add(A<Character>._, A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Should_Return_Failed_When_Character_Not_Found()
        {
            // Arrange
            var missingId = Guid.NewGuid();
            A.CallTo(() => _characterRepository.GetCharacterById(missingId)).Returns((Character?)null);

            var command = new AddToFightCommand(missingId);

            // Act
            var response = await _handler.ExecuteAsync(command);

            // Assert
            response.Status.Should().Be(RequestStatus.Failed);
        }

        [Test]
        public async Task Should_Send_WriteLogCommand_SubCommand()
        {
            // Arrange
            var player = CharacterFactory.BuildPlayer(name: "Gandalf");
            var fighter = player.AsFighter();
            A.CallTo(() => _characterRepository.GetCharacterById(player.Id)).Returns(player);
            SetupPromptReturns(18);
            SetupAddCreatesNewFighter(player, fighter);

            var command = new AddToFightCommand(player.Id);

            // Act
            await _handler.ExecuteAsync(command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                A<WriteLogCommand>.That.Matches(x => x.Content.Contains("joined the fight")),
                A<AddToFightCommand>._))
                .MustHaveHappenedOnceExactly();
        }
    }

    [TestFixture]
    internal class UndoTests : AddToFightCommandHandlerTests
    {
        [Test]
        public async Task Should_Remove_AddedFighter()
        {
            // Arrange
            var player = CharacterFactory.BuildPlayer(name: "Aragorn");
            var fighter = player.AsFighter();
            A.CallTo(() => _fightContext[fighter.Id]).Returns(fighter);

            var command = new AddToFightCommand(player.Id) { AddedFighterId = fighter.Id };

            // Act / Assert — removal is handled by the atomic sub-command cascade
            await _handler.UndoAsync(command);
        }

        [Test]
        public async Task Should_Not_Fail_When_Fighter_Already_Gone()
        {
            // Arrange
            var command = new AddToFightCommand(Guid.NewGuid()) { AddedFighterId = Guid.NewGuid() };
            A.CallTo(() => _fightContext[command.AddedFighterId.Value]).Returns((FightingCharacter?)null);

            // Act / Assert — should not throw
            await _handler.UndoAsync(command);
        }
    }

    [TestFixture]
    internal class RedoTests : AddToFightCommandHandlerTests
    {
        [Test]
        public async Task Should_ReExecute()
        {
            // Arrange
            var player = CharacterFactory.BuildPlayer(name: "Aragorn");
            var fighter = player.AsFighter();
            A.CallTo(() => _characterRepository.GetCharacterById(player.Id)).Returns(player);
            SetupPromptReturns(14);
            SetupAddCreatesNewFighter(player, fighter);

            var command = new AddToFightCommand(player.Id);

            // Act
            await _handler.RedoAsync(command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                    A<AddToFightAtomicCommand>.That.Matches(x => x.SourceCharacterId == player.Id),
                    A<AddToFightCommand>._))
                .MustHaveHappenedOnceExactly();
        }
    }
}
