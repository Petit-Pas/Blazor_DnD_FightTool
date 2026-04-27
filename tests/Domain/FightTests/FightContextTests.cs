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
