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
            _log = A.Fake<ILogger<FightContext>>();
            _characterRepository = A.Fake<ICharacterRepository>();
            _mapper = A.Fake<IMapper>();

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
                A.CallTo(() => _mapper.Clone(monster))
                    .Returns(clonedMonster);

                // Act
                _fightContext.Add(monster);

                // Assert
                _fightContext[clonedMonster.Id]!.Id.Should().Be(clonedMonster.Id);
            }

            [Test]
            public void Should_NumberFirstMonster_WithSuffix1()
            {
                // Arrange
                var monster = CharacterFactory.BuildMonster(name: "Goblin");
                var clone = CharacterFactory.BuildMonster(name: "Goblin");
                A.CallTo(() => _mapper.Clone(monster)).Returns(clone);

                // Act
                _fightContext.Add(monster);

                // Assert
                _fightContext[clone.Id]!.Name.Should().Be("Goblin 1");
            }

            [Test]
            public void Should_NumberSecondMonsterOfSameType_WithSuffix2()
            {
                // Arrange — same original monster added twice (same template ID)
                var monster = CharacterFactory.BuildMonster(name: "Goblin");
                var clone1 = CharacterFactory.BuildMonster(name: "Goblin");
                var clone2 = CharacterFactory.BuildMonster(name: "Goblin");
                A.CallTo(() => _mapper.Clone(monster)).ReturnsNextFromSequence(clone1, clone2);

                // Act
                _fightContext.Add(monster);
                _fightContext.Add(monster);

                // Assert
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
                A.CallTo(() => _mapper.Clone(goblin)).Returns(goblinClone);
                A.CallTo(() => _mapper.Clone(orc)).Returns(orcClone);

                // Act
                _fightContext.Add(goblin);
                _fightContext.Add(orc);

                // Assert
                _fightContext[goblinClone.Id]!.Name.Should().Be("Goblin 1");
                _fightContext[orcClone.Id]!.Name.Should().Be("Orc 1");
            }

            [Test]
            public void Should_Add_Player_To_Fight()
            {
                // Arrange
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");

                // Act
                _fightContext.Add(player);

                // Assert
                var fighters = _fightContext.Fighters.ToArray();
                fighters.Should().Contain(x => x.Name == player.Name);
                fighters.Should().Contain(x => x.Id == player.Id);
            }
        }

        [TestFixture]
        public class RemoveFromFightTests : FightContextTests
        {
            [Test]
            public void Should_ContinueSequence_When_RemovedAndReAdded()
            {
                // Arrange — add 2 goblins, remove the first, add another
                var monster = CharacterFactory.BuildMonster(name: "Goblin");
                var clone1 = CharacterFactory.BuildMonster(name: "Goblin");
                var clone2 = CharacterFactory.BuildMonster(name: "Goblin");
                var clone3 = CharacterFactory.BuildMonster(name: "Goblin");
                A.CallTo(() => _mapper.Clone(monster)).ReturnsNextFromSequence(clone1, clone2, clone3);
                _fightContext.Add(monster);
                _fightContext.Add(monster);
                _fightContext.Remove(_fightContext[clone1.Id]!);

                // Act
                _fightContext.Add(monster);

                // Assert — counter is monotonic; next number is 3, not 2
                _fightContext[clone3.Id]!.Name.Should().Be("Goblin 3");
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
                _fightContext.Add(player);

                // Act
                var fighters = _fightContext.Fighters;

                // Assert
                fighters.Should().NotBeEmpty();
                fighters.Should().Contain(x => x.Id == player.Id);
            }
        }
    }
}
