using System;
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

            _fightContext = new FightContext(_log, _characterRepository, _mapper);
        }

        [TestFixture] 
        public class AddToFightTests : FightContextTests
        {
            [Test]
            public void Should_Add_Duplicate_Of_Monster_To_Fight()
            {
                // Arrange
                var monster = CharacterFactory.BuildMonster(name: "Imp");

                // Act
                _fightContext.AddToFight(monster);

                // Assert
                var fighters = _fightContext.GetFighters().ToArray();
                fighters.Should().Contain(x => x.Name == monster.Name);
                fighters.Should().NotContain(x => x.CharacterId == monster.Id);
            }

            [Test]
            public void Should_Add_Player_To_Fight()
            {
                // Arrange
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");

                // Act
                _fightContext.AddToFight(player);

                // Assert
                var fighters = _fightContext.GetFighters().ToArray();
                fighters.Should().Contain(x => x.Name == player.Name);
                fighters.Should().Contain(x => x.CharacterId == player.Id);
            }
        }

        [TestFixture]
        public class SetMovingFighter : FightContextTests
        {
            [Test]
            public void Should_Set_Moving_Fighter()
            {
                // Arrange
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");
                _fightContext.AddToFight(player);

                // Act
                _fightContext.SetMovingFighter(_fightContext.GetFighters().First());

                // Assert
                _fightContext.MovingFighter.Should().NotBeNull();
                _fightContext.MovingFighter!.CharacterId.Should().Be(player.Id);
            }

            [Test]
            public void Should_Raise_Event_When_Moving_Fighter_Changes()
            {
                // Arrange
                Fighter? fighter = default;
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");
                _fightContext.AddToFight(player);

                // Act
                _fightContext.MovingFighterChanged += (sender, args) => fighter = args;
                _fightContext.SetMovingFighter(_fightContext.GetFighters().First());

                // Assert
                fighter.Should().Be(_fightContext.GetFighters().First());
            }

            [Test]
            public void Should_Not_Raise_Event_When_Moving_Fighter_Does_Not_Change()
            {
                // Arrange
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");
                _fightContext.AddToFight(player);
                _fightContext.SetMovingFighter(_fightContext.GetFighters().First());

                // Act
                _fightContext.MovingFighterChanged += (sender, args) => Assert.Fail("Should not raise event");
                _fightContext.SetMovingFighter(_fightContext.GetFighters().First());

                // Assert
                Assert.Pass();
            }
        }

        [TestFixture]
        public class GetMovingFighterCharacter : FightContextTests
        {
            [Test]
            public void Should_Return_Null_When_No_Moving_Fighter()
            {
                // Arrange
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");
                _fightContext.AddToFight(player);

                // Act
                var character = _fightContext.GetMovingFighterCharacter();

                // Assert
                character.Should().BeNull();
            }

            [Test]
            public void Should_Return_Character_Of_Moving_Fighter()
            {
                // Arrange
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");
                _fightContext.AddToFight(player);
                _fightContext.SetMovingFighter(_fightContext.GetFighters().First());
                A.CallTo(() => _characterRepository.GetCharacterById(player.Id))
                    .Returns(player);

                // Act
                var character = _fightContext.GetMovingFighterCharacter();

                // Assert
                character.Should().NotBeNull();
                character!.Id.Should().Be(player.Id);
            }
        }

        [TestFixture]
        public class GetCharacterById : FightContextTests
        {
            [Test]
            public void Should_Return_Null_When_No_Character_With_Id()
            {
                // Arrange
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");
                _fightContext.AddToFight(player);

                // Act
                var character = _fightContext.GetCharacterById(Guid.NewGuid());

                // Assert
                character.Should().BeNull();
            }

            [Test]
            public void Should_Return_Character_With_Id()
            {
                // Arrange
                var player = CharacterFactory.BuildPlayer(name: "Omesmo");
                _fightContext.AddToFight(player);
                A.CallTo(() => _characterRepository.GetCharacterById(player.Id))
                    .Returns(player);

                // Act
                var character = _fightContext.GetCharacterById(player.Id);

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
                _fightContext.AddToFight(player);

                // Act
                var fighters = _fightContext.GetFighters();

                // Assert
                fighters.Should().NotBeEmpty();
                fighters.Should().Contain(x => x.CharacterId == player.Id);
            }
        }
    }
}
