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
using DomainTestsUtilities.Fakes.Savings;
using System.Threading.Tasks;

namespace DnDActionsTests.DamageActionsTests.ApplyDamageRollResultsTests;

[TestFixture]
public class ApplyDamageRollResultsCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IFightContext _fightContext = null!;

    private Character _caster = null!;
    private Character _target = null!;
    private DamageRollResult[] _damageRollResults = null!;

    private ApplyDamageRollResultsCommand _command = null!;
    private ApplyDamageRollResultsCommandHandler _commandHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>();
        _fightContext = A.Fake<IFightContext>();

        _caster = new Character();
        _target = new Character();

        _damageRollResults = new DamageRollResult[]
        {
            DamageRollResultFactory.Build(DamageTypeEnum.Fire, 10),
        };

        _target.DamageAffinities = new DamageAffinitiesCollection(true);

        _command = new ApplyDamageRollResultsCommand(_target.Id, _caster.Id, _damageRollResults);
        _commandHandler = new ApplyDamageRollResultsCommandHandler(_mediator, _fightContext);

        A.CallTo(() => _fightContext.GetCharacterById(_caster.Id))
            .Returns(_caster);
        A.CallTo(() => _fightContext.GetCharacterById(_target.Id))
            .Returns(_target);
    }

    DamageAffinitiesCollection _affinities { get => _target.DamageAffinities; }

    [TestFixture]
    public class ExecuteTests : ApplyDamageRollResultsCommandHandlerTests
    {
        [Test]
        public async Task Should_Execute_Take_Damage_Command_With_Rolled_Damage()
        {
            // Act
            await _commandHandler.Execute(_command);
            var takeDamageCommand = _command.SubCommands.OfType<TakeDamageCommand>().SingleOrDefault();

            // Assert
            takeDamageCommand.Should().NotBeNull();
            takeDamageCommand!.Damage.Should().Be(10);
        }

        [Test]
        public async Task Should_Sum_All_DamageRolls()
        {
            // Arrange
            _damageRollResults = new DamageRollResult[] { _damageRollResults.First(), _damageRollResults.First() };
            _command = new ApplyDamageRollResultsCommand(Guid.NewGuid(), Guid.NewGuid(), _damageRollResults);

            // Act
            await _commandHandler.Execute(_command);
            var takeDamageCommand = _command.SubCommands.OfType<TakeDamageCommand>().SingleOrDefault();

            // Assert
            takeDamageCommand!.Damage.Should().Be(20);
        }


        [Test]
        public async Task Should_Apply_Damage_Resistance()
        {
            // Arrange
            _affinities.First(x => x.Type == DamageTypeEnum.Fire).Affinity = DamageAffinityEnum.Weak;

            // Act
            await _commandHandler.Execute(_command);
            var takeDamageCommand = _command.SubCommands.OfType<TakeDamageCommand>().SingleOrDefault();

            // Assert
            takeDamageCommand!.Damage.Should().Be(20);
        }

        [Test]
        [TestCase(SituationalDamageModifierEnum.Normal, 10)]
        [TestCase(SituationalDamageModifierEnum.Halved, 5)]
        public async Task Should_Apply_Damage_Modifier_Factor_When_Save_Is_Succesfull(SituationalDamageModifierEnum modifier, int expectedDamage)
        {
            // Arrange
            _command = new ApplyDamageRollResultsCommand(_target.Id, _caster.Id, _damageRollResults, new FakeSaveRollResult(true));
            _command.DamageRolls.First().SuccessfullSaveModifier = modifier;

            // Act
            await _commandHandler.Execute(_command);

            // Assert
            _command.SubCommands.OfType<TakeDamageCommand>().First().Damage.Should().Be(expectedDamage);
        }

    }

    [TestFixture]
    public class RedoTests : ApplyDamageRollResultsCommandHandlerTests
    {
        // Smoke test to double check that it executes the command well, most basic Execute test.
        [Test]
        public async Task Should_Execute_Take_Damage_Command_With_Rolled_Damage()
        {
            // Act
            await _commandHandler.Execute(_command);
            var takeDamageCommand = _command.SubCommands.OfType<TakeDamageCommand>().SingleOrDefault();

            // Assert
            takeDamageCommand.Should().NotBeNull();
            takeDamageCommand!.Damage.Should().Be(10);
        }

        [Test]
        public async Task Should_Clear_SubCommands_To_Avoid_Multiplying_Them()
        {
            // Arrange
            _command.AddToSubCommands(_command);

            // Act
            await _commandHandler.Redo(_command);

            // Assert
            _command.SubCommands.Should().NotContain(_command);
        }
    }
}
