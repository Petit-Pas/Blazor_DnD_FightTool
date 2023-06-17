using DnDActions.DamageActions.ApplyDamageRollResults;
using DnDEntities.Characters;
using FakeItEasy;
using Fight;
using NUnit.Framework;
using System;
using UndoableMediator.Mediators;
using Fight.Damage;
using FightTestsUtilities.Factories.Damage;
using DnDEntities.Damage;
using DnDEntities.DamageAffinities;
using FluentAssertions;
using System.Linq;
using DnDActions.DamageActions.TakeDamage;
using DnDActions.HitPointActions.LooseHp;

namespace DnDActionsTests.DamageActionsTests.ApplyDamageRollResultsTests;

[TestFixture]
public class ApplyDamageRollResultsCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IFightContext _fightContext = null!;

    private Character _character = null!;
    private DamageRollResult[] _damageRollResults = null!;

    private ApplyDamageRollResultsCommand _command = null!;
    private ApplyDamageRollResultsCommandHandler _commandHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>();
        _fightContext = A.Fake<IFightContext>();

        _character = new Character();

        _damageRollResults = new DamageRollResult[]
        {
            DamageRollResultFactory.Build(DamageTypeEnum.Fire, 10),
        };

        _character.DamageAffinities = new DamageAffinitiesCollection(true);

        _command = new ApplyDamageRollResultsCommand(Guid.NewGuid(), _damageRollResults);
        _commandHandler = new ApplyDamageRollResultsCommandHandler(_mediator, _fightContext);

        A.CallTo(() => _fightContext.GetCharacterById(A<Guid>._))
            .Returns(_character);
    }

    DamageAffinitiesCollection _affinities { get => _character.DamageAffinities; }

    [TestFixture]
    public class ExecuteTests : ApplyDamageRollResultsCommandHandlerTests
    {
        [Test]
        public void Should_Throw_ArgumentException_When_FightContext_Returns_No_Character()
        {
            // Arrange
            A.CallTo(() => _fightContext.GetCharacterById(A<Guid>._))
                .Returns(null);

            // Act
            var executing = () => _commandHandler.Execute(_command);

            // Assert
            executing.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Should_Execute_Take_Damage_Command_With_Rolled_Damage()
        {
            // Act
            _commandHandler.Execute(_command);
            var takeDamageCommand = _command.SubCommands.OfType<TakeDamageCommand>().SingleOrDefault();

            // Assert
            takeDamageCommand.Should().NotBeNull();
            takeDamageCommand!.Damage.Should().Be(10);
        }

        [Test]
        public void Should_Sum_All_DamageRolls()
        {
            // Arrange
            _damageRollResults = new DamageRollResult[] { _damageRollResults.First(), _damageRollResults.First() };
            _command = new ApplyDamageRollResultsCommand(Guid.NewGuid(), _damageRollResults);

            // Act
            _commandHandler.Execute(_command);
            var takeDamageCommand = _command.SubCommands.OfType<TakeDamageCommand>().SingleOrDefault();

            // Assert
            takeDamageCommand!.Damage.Should().Be(20);
        }


        [Test]
        public void Should_Apply_Damage_Resistance()
        {
            // Arrange
            _affinities.First(x => x.Type == DamageTypeEnum.Fire).Affinity = DamageAffinityEnum.Weak;

            // Act
            _commandHandler.Execute(_command);
            var takeDamageCommand = _command.SubCommands.OfType<TakeDamageCommand>().SingleOrDefault();

            // Assert
            takeDamageCommand!.Damage.Should().Be(20);
        }

    }

    [TestFixture]
    public class RedoTests : ApplyDamageRollResultsCommandHandlerTests
    {
        // Smoke test to double check that it executes the command well, most basic Execute test.
        [Test]
        public void Should_Execute_Take_Damage_Command_With_Rolled_Damage()
        {
            // Act
            _commandHandler.Execute(_command);
            var takeDamageCommand = _command.SubCommands.OfType<TakeDamageCommand>().SingleOrDefault();

            // Assert
            takeDamageCommand.Should().NotBeNull();
            takeDamageCommand!.Damage.Should().Be(10);
        }

        [Test]
        public void Should_Clear_SubCommands_To_Avoid_Multiplying_Them()
        {
            // Arrange
            _command.AddToSubCommands(_command);

            // Act
            _commandHandler.Redo(_command);

            // Assert
            _command.SubCommands.Should().NotContain(_command);
        }
    }
}
