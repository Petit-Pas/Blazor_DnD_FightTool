using System;
using System.Linq;
using System.Threading.Tasks;
using DnDFightTool.Business.DnDActions.DamageActions.ApplyDamageRollResults;
using DnDFightTool.Business.DnDActions.DamageActions.TakeDamage;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Damage;
using DnDFightTool.Domain.DnDEntities.DamageAffinities;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using DomainTestsUtilities.Factories.Damage;
using DomainTestsUtilities.Fakes.Savings;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using UndoableMediator.Mediators;

namespace DnDActionsTests.DamageActionsTests.ApplyDamageRollResultsTests;

[TestFixture]
public class ApplyDamageRollResultsCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IFightContext _fightContext = null!;

    private FightingCharacter _caster = null!;
    private FightingCharacter _target = null!;
    private DamageRollResult[] _damageRollResults = null!;

    private ApplyDamageRollResultsCommand _command = null!;
    private ApplyDamageRollResultsCommandHandler _commandHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>();
        _fightContext = A.Fake<IFightContext>();

        var innerCaster = new Character();
        var innerTarget = new Character();
        _caster = new FightingCharacter(innerCaster);
        _target = new FightingCharacter(innerTarget);

        _damageRollResults =
        [
            DamageRollResultFactory.BuildRolledDice(DamageTypeEnum.Fire, 10),
        ];

        innerTarget.DamageAffinities = new DamageAffinitiesCollection(true);

        _command = new ApplyDamageRollResultsCommand(_caster.Id, _target.Id, _damageRollResults);
        _commandHandler = new ApplyDamageRollResultsCommandHandler(_mediator, _fightContext);

        A.CallTo(() => _fightContext[_caster.Id])
            .Returns(_caster);
        A.CallTo(() => _fightContext[_target.Id])
            .Returns(_target);
    }

    private DamageAffinitiesCollection _affinities { get => _target.DamageAffinities; }

    [TestFixture]
    public class ExecuteTests : ApplyDamageRollResultsCommandHandlerTests
    {
        [Test]
        public async Task Should_Execute_Take_Damage_Command_With_Rolled_Damage()
        {
            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(A<TakeDamageCommand>.That.Matches(x => x.Damage == 10), A<ApplyDamageRollResultsCommand>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_Sum_All_DamageRolls()
        {
            // Arrange
            _damageRollResults = [_damageRollResults.First(), _damageRollResults.First()];
            _command = new ApplyDamageRollResultsCommand(Guid.NewGuid(), Guid.NewGuid(), _damageRollResults);

            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(A<TakeDamageCommand>.That.Matches(x => x.Damage == 20), A<ApplyDamageRollResultsCommand>._))
                .MustHaveHappenedOnceExactly();
        }


        [Test]
        public async Task Should_Apply_Damage_Resistance()
        {
            // Arrange
            _affinities[DamageTypeEnum.Fire].Affinity = DamageAffinityEnum.Weak;

            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(A<TakeDamageCommand>.That.Matches(x => x.Damage == 20), A<ApplyDamageRollResultsCommand>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        [TestCase(SituationalDamageModifierEnum.Normal, 10)]
        [TestCase(SituationalDamageModifierEnum.Halved, 5)]
        public async Task Should_Apply_Damage_Modifier_Factor_When_Save_Is_Succesfull(SituationalDamageModifierEnum modifier, int expectedDamage)
        {
            // Arrange
            _command = new ApplyDamageRollResultsCommand(_target.Id, _caster.Id, _damageRollResults, new FakeSaveRollResult(true));
            _command.DamageRolls.First().SuccessfulSaveModifier = modifier;

            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(A<TakeDamageCommand>.That.Matches(x => x.Damage == expectedDamage), A<ApplyDamageRollResultsCommand>._))
                .MustHaveHappenedOnceExactly();
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
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(A<TakeDamageCommand>.That.Matches(x => x.Damage == 10), A<ApplyDamageRollResultsCommand>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_Reexecute_On_Redo()
        {
            // Act
            await _commandHandler.RedoAsync(_command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(A<TakeDamageCommand>._, A<ApplyDamageRollResultsCommand>._))
                .MustHaveHappenedOnceExactly();
        }
    }
}
