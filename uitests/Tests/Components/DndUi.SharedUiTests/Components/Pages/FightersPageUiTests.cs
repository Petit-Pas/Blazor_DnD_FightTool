using DnDFightTool.Business.DnDActions.FightActions.AddToFight;
using DnDFightTool.Business.DnDActions.FightActions.RemoveFromFight;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.Fight;
using DnDFightTool.UiTests.Framework;
using DnDFightTool.UiTests.Framework.Extensions;
using DnDFightTool.UiTests.TestableComponents.Components.TestableDndUi.Shared.Components.Pages;
using DomainTestsUtilities.Factories.Characters;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using UndoableMediator.Mediators;

namespace DnDFightTool.UiTests.Tests.Components.DndUi.SharedUiTests.Components.Pages;

/// <summary>
///     Independent search, addition and removal capabilities of the fighters page.
/// </summary>
public static class FightersPageUiTests
{
    /// <summary>
    ///     Filtering the available characters without changing the other character list.
    /// </summary>
    public sealed class SearchFighters : IsolatedScenarioFixture
    {
        private TestableFightersPage _fighters = null!;

        /// <summary>
        ///     Seeds both character lists and opens the fighters page before each search test.
        /// </summary>
        [SetUp]
        public async Task SetUpSearchFighters()
        {
            var repository = Services.GetRequiredService<ICharacterRepository>();
            repository.Save(CharacterFactory.BuildPlayer(name: "Alpha"));
            repository.Save(CharacterFactory.BuildPlayer(name: "Beta"));
            repository.Save(CharacterFactory.BuildMonster(name: "Alpha"));
            repository.Save(CharacterFactory.BuildMonster(name: "Beta"));
            _fighters = new TestableFightersPage(Page, BaseUrl);
            await _fighters.GoToAsync();
        }

        /// <summary>
        ///     A matching player search filters players without changing the monster list.
        /// </summary>
        [Test]
        public async Task Should_Filter_Players_By_Partial_Name()
        {
            // Act
            await _fighters.SearchPlayers("Al");

            // Assert
            (await _fighters.AvailablePlayerNames()).Should().Equal("Alpha");
            (await _fighters.AvailableMonsterNames()).Should().BeEquivalentTo("Alpha", "Beta");
        }

        /// <summary>
        ///     A matching monster search filters monsters without changing the player list.
        /// </summary>
        [Test]
        public async Task Should_Filter_Monsters_By_Partial_Name()
        {
            // Act
            await _fighters.SearchMonsters("Al");

            // Assert
            (await _fighters.AvailableMonsterNames()).Should().Equal("Alpha");
            (await _fighters.AvailablePlayerNames()).Should().BeEquivalentTo("Alpha", "Beta");
        }

        /// <summary>
        ///     A nonmatching player search leaves no visible player result.
        /// </summary>
        [Test]
        public async Task Should_Show_No_Players_When_Nothing_Matches()
        {
            // Act
            await _fighters.SearchPlayers("Missing");

            // Assert
            (await _fighters.AvailablePlayerNames()).Should().BeEmpty();
        }

        /// <summary>
        ///     A nonmatching monster search leaves no visible monster result.
        /// </summary>
        [Test]
        public async Task Should_Show_No_Monsters_When_Nothing_Matches()
        {
            // Act
            await _fighters.SearchMonsters("Missing");

            // Assert
            (await _fighters.AvailableMonsterNames()).Should().BeEmpty();
        }
    }

    /// <summary>
    ///     Adding players and monsters, including availability and displayed monster groups.
    /// </summary>
    public sealed class AddFighters : IsolatedScenarioFixture
    {
        private TestableFightersPage _fighters = null!;

        /// <summary>
        ///     Seeds available players and monster templates and opens the fighters page.
        /// </summary>
        [SetUp]
        public async Task SetUpAddFighters()
        {
            var repository = Services.GetRequiredService<ICharacterRepository>();
            repository.Save(CharacterFactory.BuildPlayer(name: "Aragorn"));
            repository.Save(CharacterFactory.BuildPlayer(name: "Legolas"));
            repository.Save(CharacterFactory.BuildMonster(name: "Goblin"));
            repository.Save(CharacterFactory.BuildMonster(name: "Orc"));
            _fighters = new TestableFightersPage(Page, BaseUrl);
            await _fighters.GoToAsync();
        }

        /// <summary>
        ///     Clicking Add for a player adds that source to the fight service.
        /// </summary>
        [Test]
        public async Task Should_Add_A_Player_To_The_Fight_Service()
        {
            var player = Services.GetRequiredService<ICharacterRepository>().GetAllCharacters()
                .Single(character => character.Name == "Aragorn");
            var fight = Services.GetRequiredService<IFightContext>();

            // Act
            await _fighters.AddPlayer("Aragorn");

            // Assert
            await fight.Eventually(context => context.Fighters.Select(fighter => fighter.OriginalCharacterId)
                .Should().Equal(player.Id));
        }

        /// <summary>
        ///     Clicking Add for a first monster adds its source to the fight service.
        /// </summary>
        [Test]
        public async Task Should_Add_A_Monster_To_The_Fight_Service()
        {
            var monster = Services.GetRequiredService<ICharacterRepository>().GetAllCharacters()
                .Single(character => character.Name == "Goblin");
            var fight = Services.GetRequiredService<IFightContext>();

            // Act
            await _fighters.AddMonsterWithInitiative("Goblin");

            // Assert
            await fight.Eventually(context => context.Fighters.Select(fighter => fighter.OriginalCharacterId)
                .Should().Equal(monster.Id));
        }

        /// <summary>
        ///     Clicking Add for an existing monster kind adds another instance to the service.
        /// </summary>
        [Test]
        public async Task Should_Add_Another_Monster_To_The_Fight_Service()
        {
            // Arrange
            var monster = Services.GetRequiredService<ICharacterRepository>().GetAllCharacters()
                .Single(character => character.Name == "Goblin");
            var fight = Services.GetRequiredService<IFightContext>();
            await Services.GetRequiredService<IUndoableMediator>()
                .SendAsync(new AddToFightAtomicCommand(monster.Id, initiative: 15));

            // Act
            await _fighters.AddMonster("Goblin");

            // Assert
            await fight.Eventually(context => context.Fighters.Select(fighter => fighter.OriginalCharacterId)
                .Should().Equal(monster.Id, monster.Id));
        }
    }

    /// <summary>
    ///     Rendering notified service changes without using the page's Add or Remove controls.
    /// </summary>
    public sealed class DisplayRoster : IsolatedScenarioFixture
    {
        private TestableFightersPage _fighters = null!;
        private IUndoableMediator _mediator = null!;
        private IFightContext _fight = null!;
        private Character _player = null!;
        private Character _monster = null!;
        private Character _otherMonster = null!;

        /// <summary>
        ///     Opens a seeded library before service-driven roster changes.
        /// </summary>
        [SetUp]
        public async Task SetUpRoster()
        {
            var repository = Services.GetRequiredService<ICharacterRepository>();
            _mediator = Services.GetRequiredService<IUndoableMediator>();
            _fight = Services.GetRequiredService<IFightContext>();
            _player = CharacterFactory.BuildPlayer(name: "Aragorn");
            _monster = CharacterFactory.BuildMonster(name: "Goblin");
            _otherMonster = CharacterFactory.BuildMonster(name: "Orc");
            repository.Save(_player);
            repository.Save(CharacterFactory.BuildPlayer(name: "Legolas"));
            repository.Save(_monster);
            repository.Save(_otherMonster);
            _fighters = new TestableFightersPage(Page, BaseUrl);
            await _fighters.GoToAsync();
        }

        /// <summary>
        ///     A service-added player moves from Available to In Fight.
        /// </summary>
        [Test]
        public async Task Should_Render_A_Player_Added_By_The_Service()
        {
            await _mediator.SendAsync(new AddToFightAtomicCommand(_player.Id, initiative: 15));

            await _fighters.WaitForInFightCount(1);
            await _fighters.WaitForAvailable("Players", 1);
            (await _fighters.InFightNames()).Should().ContainSingle().Which.Should().StartWith("Aragorn (init ");
            (await _fighters.AvailablePlayerNames()).Should().Equal("Legolas");
        }

        /// <summary>
        ///     A service-added monster leaves its template available.
        /// </summary>
        [Test]
        public async Task Should_Render_A_Monster_Added_By_The_Service()
        {
            await _mediator.SendAsync(new AddToFightAtomicCommand(_monster.Id, initiative: 15));

            await _fighters.ExpectMonsterCount("Goblin", 1);
            (await _fighters.AvailableMonsterNames()).Should().BeEquivalentTo("Goblin", "Orc");
        }

        /// <summary>
        ///     Service fighters of the same kind share a counted row; other kinds stay separate.
        /// </summary>
        [Test]
        public async Task Should_Group_Service_Monsters_By_Kind()
        {
            await _mediator.SendAsync(new AddToFightAtomicCommand(_monster.Id, initiative: 15));
            await _mediator.SendAsync(new AddToFightAtomicCommand(_otherMonster.Id, initiative: 10));
            await _fighters.WaitForInFightCount(2);

            await _mediator.SendAsync(new AddToFightAtomicCommand(_monster.Id, initiative: 15));

            await _fighters.ExpectMonsterCount("Goblin", 2);
            await _fighters.ExpectMonsterCount("Orc", 1);
            (await _fighters.InFightNames()).Should().HaveCount(2);
        }

        /// <summary>
        ///     Removing a player through the service returns it to Available.
        /// </summary>
        [Test]
        public async Task Should_Render_A_Player_Removed_By_The_Service()
        {
            await _mediator.SendAsync(new AddToFightAtomicCommand(_player.Id, initiative: 15));
            await _fighters.WaitForAvailable("Players", 1);

            await _mediator.SendAsync(new RemoveFromFightCommand(_fight.Fighters.Single().Id));

            await _fighters.WaitForInFightCount(0);
            await _fighters.WaitForAvailable("Players", 2);
            (await _fighters.AvailablePlayerNames()).Should().BeEquivalentTo("Aragorn", "Legolas");
        }

        /// <summary>
        ///     Removing the last service monster clears its row, not its template.
        /// </summary>
        [Test]
        public async Task Should_Render_The_Last_Monster_Removed_By_The_Service()
        {
            await _mediator.SendAsync(new AddToFightAtomicCommand(_monster.Id, initiative: 15));
            await _fighters.ExpectMonsterCount("Goblin", 1);

            await _mediator.SendAsync(new RemoveFromFightCommand(_fight.Fighters.Single().Id));

            await _fighters.WaitForInFightCount(0);
            (await _fighters.AvailableMonsterNames()).Should().BeEquivalentTo("Goblin", "Orc");
        }

        /// <summary>
        ///     Removing one service monster updates the existing group count.
        /// </summary>
        [Test]
        public async Task Should_Render_A_Reduced_Service_Monster_Group()
        {
            await _mediator.SendAsync(new AddToFightAtomicCommand(_monster.Id, initiative: 15));
            await _mediator.SendAsync(new AddToFightAtomicCommand(_monster.Id, initiative: 15));
            await _fighters.ExpectMonsterCount("Goblin", 2);

            await _mediator.SendAsync(new RemoveFromFightCommand(_fight.Fighters.First().Id));

            await _fighters.ExpectMonsterCount("Goblin", 1);
            (await _fighters.InFightNames()).Should().HaveCount(1);
        }
    }

    /// <summary>
    ///     Removing players, the last monster of a kind, or one instance from a group.
    /// </summary>
    public sealed class RemoveFighters : IsolatedScenarioFixture
    {
        /// <summary>
        ///     Clicking Remove deletes only the selected player from the service roster.
        /// </summary>
        [Test]
        public async Task Should_Remove_The_Selected_Player()
        {
            // Arrange
            var repository = Services.GetRequiredService<ICharacterRepository>();
            var mediator = Services.GetRequiredService<IUndoableMediator>();
            var target = CharacterFactory.BuildPlayer(name: "Target");
            var other = CharacterFactory.BuildMonster(name: "Other");
            repository.Save(target);
            repository.Save(other);
            await mediator.SendAsync(new AddToFightAtomicCommand(target.Id, initiative: 15));
            await mediator.SendAsync(new AddToFightAtomicCommand(other.Id, initiative: 10));
            var fighters = new TestableFightersPage(Page, BaseUrl);
            await fighters.GoToAsync();

            // Act
            await fighters.RemoveFromFight("Target");

            // Assert
            var fight = Services.GetRequiredService<IFightContext>();
            await fight.Eventually(context => context.Fighters.Select(fighter => fighter.OriginalCharacterId)
                .Should().Equal(other.Id));
        }

        /// <summary>
        ///     Clicking Remove deletes only the selected monster from the service roster.
        /// </summary>
        [Test]
        public async Task Should_Remove_The_Selected_Monster()
        {
            // Arrange
            var repository = Services.GetRequiredService<ICharacterRepository>();
            var mediator = Services.GetRequiredService<IUndoableMediator>();
            var target = CharacterFactory.BuildMonster(name: "Target");
            var other = CharacterFactory.BuildMonster(name: "Other");
            repository.Save(target);
            repository.Save(other);
            await mediator.SendAsync(new AddToFightAtomicCommand(target.Id, initiative: 15));
            await mediator.SendAsync(new AddToFightAtomicCommand(other.Id, initiative: 10));
            var fighters = new TestableFightersPage(Page, BaseUrl);
            await fighters.GoToAsync();

            // Act
            await fighters.RemoveFromFight("Target");

            // Assert
            var fight = Services.GetRequiredService<IFightContext>();
            await fight.Eventually(context => context.Fighters.Select(fighter => fighter.OriginalCharacterId)
                .Should().Equal(other.Id));
        }

        /// <summary>
        ///     Subtracting from a monster group removes only one instance.
        /// </summary>
        [Test]
        public async Task Should_Remove_Only_One_Monster_From_A_Group()
        {
            // Arrange
            var repository = Services.GetRequiredService<ICharacterRepository>();
            var mediator = Services.GetRequiredService<IUndoableMediator>();
            var goblin = CharacterFactory.BuildMonster(name: "Goblin");
            repository.Save(goblin);
            await mediator.SendAsync(new AddToFightAtomicCommand(goblin.Id, initiative: 15));
            await mediator.SendAsync(new AddToFightAtomicCommand(goblin.Id, initiative: 15));
            var fighters = new TestableFightersPage(Page, BaseUrl);
            await fighters.GoToAsync();

            // Act
            await fighters.RemoveFromFight("Goblin");

            // Assert
            var fight = Services.GetRequiredService<IFightContext>();
            await fight.Eventually(context => context.Fighters.Select(fighter => fighter.OriginalCharacterId)
                .Should().Equal(goblin.Id));
        }
    }
}
