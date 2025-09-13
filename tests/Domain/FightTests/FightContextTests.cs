using System.Linq;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using DomainTestsUtilities.Factories.Characters;
using FakeItEasy;
using FluentAssertions;
using Mapping;
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
                _fightContext[clonedMonster.Id].Id.Should().Be(clonedMonster.Id);
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
        public class SetActiveFighter : FightContextTests
        {
            [Test]
            public void Should_Set_Moving_Fighter()
            {
                // Arrange
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");
                _fightContext.Add(player);

                // Act
                _fightContext.SetActiveFighter(_fightContext.Fighters.First().Id);

                // Assert
                _fightContext.ActiveFighter.Should().NotBeNull();
                _fightContext.ActiveFighter!.Id.Should().Be(player.Id);
            }

            [Test]
            public void Should_Raise_Event_When_Moving_Fighter_Changes()
            {
                // Arrange
                FightingCharacter? fighter = default;
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");
                _fightContext.Add(player);

                // Act
                _fightContext.OnActiveFighterChanged += (sender, args) => fighter = args;
                _fightContext.SetActiveFighter(_fightContext.Fighters.First().Id);

                // Assert
                fighter!.Id.Should().Be(_fightContext.Fighters.First().Id);
            }

            [Test]
            public void Should_Not_Raise_Event_When_Moving_Fighter_Does_Not_Change()
            {
                // Arrange
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");
                _fightContext.Add(player);
                _fightContext.SetActiveFighter(_fightContext.Fighters.First().Id);

                // Act
                _fightContext.OnActiveFighterChanged += (sender, args) => Assert.Fail("Should not raise event");
                _fightContext.SetActiveFighter(_fightContext.Fighters.First().Id);

                // Assert
                Assert.Pass();
            }
        }

        [TestFixture]
        public class GetActiveFighter : FightContextTests
        {
            [Test]
            public void Should_Return_Null_When_No_Moving_Fighter()
            {
                // Arrange
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");
                _fightContext.Add(player);

                // Act
                var character = _fightContext.ActiveFighter;

                // Assert
                character.Should().BeNull();
            }

            [Test]
            public void Should_Return_Character_Of_Moving_Fighter()
            {
                // Arrange
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");
                _fightContext.Add(player);
                _fightContext.SetActiveFighter(_fightContext.Fighters.First().Id);
                A.CallTo(() => _characterRepository.GetCharacterById(player.Id))
                    .Returns(player);

                // Act
                var character = _fightContext.ActiveFighter;

                // Assert
                character.Should().NotBeNull();
                character!.Id.Should().Be(player.Id);
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
