using System.Linq;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using DomainTestsUtilities.Factories.Characters;
using FakeItEasy;
using FluentAssertions;
using DnDFightTool.Infrastructure.Mapping;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace FightTests
{
    [TestFixture]
    public class FightContextTests
    {
        private ILogger<FightContext> _log = null!;
        private ICharacterRepository _characterRepository = null!;
        private IMapper _mapper = null!;

        private FightContext _fightContext = null!;

        [SetUp]
        public void Setup()
        {
            _log = A.Fake<ILogger<FightContext>>(options => options.Strict());
            _characterRepository = A.Fake<ICharacterRepository>(options => options.Strict());
            _mapper = A.Fake<IMapper>(options => options.Strict());

            _fightContext = new FightContext(_log, _mapper);
        }

        [TestFixture] 
        public class AddToFightTests : FightContextTests
        {
            [Test]
            public void Should_Add_Duplicate_Of_Monster_To_Fight()
            {
                // Arrange
                var monster = CharacterFactory.BuildMonster(name: "Imp");
                var clonedMonster = CharacterFactory.BuildMonster(name: "Imp2");
                A.CallTo(() => _mapper.Clone(A<ICharacter>._))
                    .Returns(clonedMonster);

                // Act
                var addedFighter = _fightContext.Add(monster, 10);

                // Assert
                addedFighter.Should().NotBeNull();
                _fightContext[clonedMonster.Id]!.Id.Should().Be(clonedMonster.Id);
            }

            [Test]
            public void Should_NumberFirstMonster_WithSuffix1()
            {
                // Arrange
                var monster = CharacterFactory.BuildMonster(name: "Goblin");
                var clone = CharacterFactory.BuildMonster(name: "Goblin");
                A.CallTo(() => _mapper.Clone(A<ICharacter>._)).Returns(clone);

                // Act
                var addedFighter = _fightContext.Add(monster, 10);

                // Assert
                addedFighter.Should().NotBeNull();
                _fightContext[clone.Id]!.Name.Should().Be("Goblin 1");
            }

            [Test]
            public void Should_NumberSecondMonsterOfSameType_WithSuffix2()
            {
                // Arrange — same original monster added twice (same template ID)
                var monster = CharacterFactory.BuildMonster(name: "Goblin");
                var clone1 = CharacterFactory.BuildMonster(name: "Goblin");
                var clone2 = CharacterFactory.BuildMonster(name: "Goblin");
                A.CallTo(() => _mapper.Clone(A<ICharacter>._)).ReturnsNextFromSequence(clone1, clone2);

                // Act
                var added1 = _fightContext.Add(monster, 10);
                var added2 = _fightContext.Add(monster, 12);

                // Assert
                added1.Should().NotBeNull();
                added2.Should().NotBeNull();
                _fightContext[clone1.Id]!.Name.Should().Be("Goblin 1");
                _fightContext[clone2.Id]!.Name.Should().Be("Goblin 2");
            }

            [Test]
            public void Should_NumberMonstersIndependently_When_DifferentOriginalIds()
            {
                // Arrange — two different monster templates (different original IDs)
                var goblin = CharacterFactory.BuildMonster(name: "Goblin");
                var orc = CharacterFactory.BuildMonster(name: "Orc");
                var goblinClone = CharacterFactory.BuildMonster(name: "Goblin");
                var orcClone = CharacterFactory.BuildMonster(name: "Orc");
                A.CallTo(() => _mapper.Clone(A<ICharacter>._)).ReturnsNextFromSequence(goblinClone, orcClone);

                // Act
                var addedGoblin = _fightContext.Add(goblin, 10);
                var addedOrc = _fightContext.Add(orc, 12);

                // Assert
                addedGoblin.Should().NotBeNull();
                addedOrc.Should().NotBeNull();
                _fightContext[goblinClone.Id]!.Name.Should().Be("Goblin 1");
                _fightContext[orcClone.Id]!.Name.Should().Be("Orc 1");
            }

            [Test]
            public void Should_Add_Player_To_Fight()
            {
                // Arrange
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");

                // Act
                var addedPlayer = _fightContext.Add(player, 10);

                // Assert
                addedPlayer.Should().NotBeNull();
                var fighters = _fightContext.Fighters.ToArray();
                fighters.Should().Contain(x => x.Name == player.Name);
                fighters.Should().Contain(x => x.Id == player.Id);
            }
        }

        [TestFixture]
        public class RemoveFromFightTests : FightContextTests
        {
            [Test]
            public void Should_DecrementCounter_When_Removed_So_NextAdd_Reuses_Number()
            {
                // Arrange — add 2 goblins, remove the first, add another
                var monster = CharacterFactory.BuildMonster(name: "Goblin");
                var clone1 = CharacterFactory.BuildMonster(name: "Goblin");
                var clone2 = CharacterFactory.BuildMonster(name: "Goblin");
                var clone3 = CharacterFactory.BuildMonster(name: "Goblin");
                A.CallTo(() => _mapper.Clone(A<ICharacter>._)).ReturnsNextFromSequence(clone1, clone2, clone3);
                _fightContext.Add(monster, 10);
                _fightContext.Add(monster, 12);
                _fightContext.Remove(_fightContext[clone1.Id]!);

                // Act
                var addedClone3 = _fightContext.Add(monster, 14);

                // Assert — counter decrements on Remove; next number reuses freed slot (count was 1 after remove → +1 = 2)
                _fightContext[clone3.Id]!.Name.Should().Be("Goblin 2");
            }

            [Test]
            public void Should_RemoveCounterKey_When_LastMonsterOfKind_Removed()
            {
                // Arrange
                var monster = CharacterFactory.BuildMonster(name: "Goblin");
                var clone1 = CharacterFactory.BuildMonster(name: "Goblin");
                var clone2 = CharacterFactory.BuildMonster(name: "Goblin");
                A.CallTo(() => _mapper.Clone(A<ICharacter>._)).ReturnsNextFromSequence(clone1, clone2);
                _fightContext.Add(monster, 10);
                _fightContext.Remove(_fightContext[clone1.Id]!);

                // Act — re-adding after the last of the kind was removed re-starts the count from 1
                var addedClone2 = _fightContext.Add(monster, 12);

                // Assert
                _fightContext[clone2.Id]!.Name.Should().Be("Goblin 1");
            }

            [Test]
            public void Should_FireOnFighterRemoved()
            {
                // Arrange
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");
                var addedPlayer = _fightContext.Add(player, 10);
                addedPlayer.Should().NotBeNull();
                IFightingCharacter? removed = null;
                _fightContext.OnFighterRemoved += (_, f) => removed = f;

                // Act
                _fightContext.Remove(_fightContext[player.Id]!);

                // Assert
                removed.Should().NotBeNull();
                removed!.Id.Should().Be(player.Id);
            }
        }

        [TestFixture]
        public class OnFighterAddedTests : FightContextTests
        {
            [Test]
            public void Should_Fire_OnFighterAdded_When_Player_Added()
            {
                // Arrange
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");
                IFightingCharacter? added = null;
                _fightContext.OnFighterAdded += (_, f) => added = f;

                // Act
                var addedPlayer = _fightContext.Add(player, 10);

                // Assert
                addedPlayer.Should().NotBeNull();
                added!.Id.Should().Be(player.Id);
            }

            [Test]
            public void Should_Fire_OnFighterAdded_When_Monster_Added()
            {
                // Arrange
                var monster = CharacterFactory.BuildMonster(name: "Goblin");
                var clone = CharacterFactory.BuildMonster(name: "Goblin");
                A.CallTo(() => _mapper.Clone(A<ICharacter>._)).Returns(clone);
                IFightingCharacter? added = null;
                _fightContext.OnFighterAdded += (_, f) => added = f;

                // Act
                var addedMonster = _fightContext.Add(monster, 10);

                // Assert
                addedMonster.Should().NotBeNull();
                added.Should().NotBeNull();
                added!.Id.Should().Be(clone.Id);
            }
        }

        [TestFixture]
        public class OriginalCharacterIdTests : FightContextTests
        {
            [Test]
            public void Player_Should_Have_OriginalCharacterId_EqualToCharacterId()
            {
                // Arrange
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");

                // Act
                var addedPlayer = _fightContext.Add(player, 10);

                // Assert
                addedPlayer.Should().NotBeNull();
                _fightContext[player.Id]!.OriginalCharacterId.Should().Be(player.Id);
            }

            [Test]
            public void Monster_Should_Have_OriginalCharacterId_EqualToTemplateId_NotClonedId()
            {
                // Arrange
                var monster = CharacterFactory.BuildMonster(name: "Goblin");
                var clone = CharacterFactory.BuildMonster(name: "Goblin");
                A.CallTo(() => _mapper.Clone(A<ICharacter>._)).Returns(clone);

                // Act
                var addedMonster = _fightContext.Add(monster, 10);

                // Assert
                addedMonster.Should().NotBeNull();
                _fightContext[clone.Id]!.OriginalCharacterId.Should().Be(monster.Id);
                _fightContext[clone.Id]!.OriginalCharacterId.Should().NotBe(clone.Id);
            }
        }

        [TestFixture]
        public class RestoreTests : FightContextTests
        {
            [Test]
            public void Should_ReInsert_TheSameInstance()
            {
                // Arrange
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");
                var addedPlayer = _fightContext.Add(player, 10);
                addedPlayer.Should().NotBeNull();
                var fighter = _fightContext[player.Id]!;
                _fightContext.Remove(fighter);

                // Act
                _fightContext.Restore(fighter.Id);

                // Assert
                _fightContext[player.Id].Should().BeSameAs(fighter);
            }

            [Test]
            public void Should_Fire_OnFighterAdded()
            {
                // Arrange
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");
                var addedPlayer = _fightContext.Add(player, 10);
                addedPlayer.Should().NotBeNull();
                var fighter = _fightContext[player.Id]!;
                _fightContext.Remove(fighter);
                IFightingCharacter? added = null;
                _fightContext.OnFighterAdded += (_, f) => added = f;

                // Act
                _fightContext.Restore(fighter.Id);

                // Assert
                added.Should().BeSameAs(fighter);
            }

            [Test]
            public void Should_ReIncrementMonsterCounter_So_NextAdd_GetsHigherNumber()
            {
                // Arrange — add 1 goblin, remove it, restore it, then add another
                var monster = CharacterFactory.BuildMonster(name: "Goblin");
                var clone1 = CharacterFactory.BuildMonster(name: "Goblin");
                var clone2 = CharacterFactory.BuildMonster(name: "Goblin");
                A.CallTo(() => _mapper.Clone(A<ICharacter>._)).ReturnsNextFromSequence(clone1, clone2);
                var added1 = _fightContext.Add(monster, 10);
                added1.Should().NotBeNull();
                var fighter = _fightContext[clone1.Id]!;
                _fightContext.Remove(fighter);
                _fightContext.Restore(fighter.Id);

                // Act
                var added2 = _fightContext.Add(monster, 12);

                // Assert — counter went 0 → 1 (Add) → 0 (Remove) → 1 (Restore) → 2 (Add). Name = "Goblin 2".
                _fightContext[clone2.Id]!.Name.Should().Be("Goblin 2");
            }
        }

        [TestFixture]
        public class GetFighters : FightContextTests
        {
            [Test]
            public void Should_Return_Fighters()
            {
                // Arrange
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");
                var addedPlayer = _fightContext.Add(player, 10);
                addedPlayer.Should().NotBeNull();

                // Act
                var fighters = _fightContext.Fighters;

                // Assert
                fighters.Should().NotBeEmpty();
                fighters.Should().Contain(x => x.Id == player.Id);
            }
        }
    }
}
