using DnDFightTool.Business.DnDActions.DamageActions.ApplyDamageRollResults;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Damage;
using DnDFightTool.Domain.Fight;
using NUnit.Framework;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;
using System.Threading.Tasks;
using DnDFightTool.Business.DnDActions.MartialAttackActions.ExecuteMartialAttack;
using DnDFightTool.Domain.DnDEntities.DamageAffinities;
using FakeItEasy;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using System.Linq;
using DnDFightTool.Business.DnDQueries.MartialAttackQueries;
using UndoableMediator.Queries;
using FluentAssertions;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using DomainTestsUtilities.Factories.MartialAttacks;
using System;
using DnDFightTool.Business.DnDActions.StatusActions.TryApplyStatus;
using DomainTestsUtilities.Factories.Damage;

namespace DnDActionsTests.MartialAttackActionsTests.ExecuteMartialAttackTests;

[TestFixture]
public class ExecuteMartialAttackCommandHandlerTests
{

    private IUndoableMediator _mediator = null!;
    private IFightContext _fightContext = null!;

    private Character _caster = null!;
    private Character _target = null!;
    
    private ExecuteMartialAttackCommand _command = null!;
    private ExecuteMartialAttackCommandHandler _commandHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>();
        _fightContext = A.Fake<IFightContext>();

        _caster = new Character(true);
        _caster.MartialAttacks.Add(MartialAttackTemplateFactory.Build()); ;
        _target = new Character(true)
        {
            DamageAffinities = new DamageAffinitiesCollection(true)
        };

        _command = new ExecuteMartialAttackCommand(_caster.Id, _attackTemplate.Id);
        _commandHandler = new ExecuteMartialAttackCommandHandler(_mediator, _fightContext);

        A.CallTo(() => _fightContext.GetCharacterById(_caster.Id))
            .Returns(_caster);
        A.CallTo(() => _fightContext.GetCharacterById(_target.Id))
            .Returns(_target);

        When_Query_Returns();
    }

    private MartialAttackTemplate _attackTemplate => _caster.MartialAttacks.Values.First();

    /// <summary>
    ///     Allows to configure the result of the query. Will configure a default success one if nothing is specified.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="status"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    private void When_Query_Returns(MartialAttackRollResult? result = null, RequestStatus status = RequestStatus.Success)
    {
        result ??= MartialAttackRollResultFactory.Build();

        // TODO should update mediator package to make the constructor public, this is annoying as hell in the end.
        var queryResponse = status switch
        {
            RequestStatus.Success => QueryResponse<MartialAttackRollResult>.Success(result),
            RequestStatus.Canceled => QueryResponse<MartialAttackRollResult>.Canceled(result),
            RequestStatus.Failed => QueryResponse<MartialAttackRollResult>.Failed(result),
            _ => throw new System.NotImplementedException(),
        };

        A.CallTo(() => _mediator.Execute(A<MartialAttackRollResultQuery>._))
            .Returns(queryResponse);
    }

    [TestFixture]
    public class ExecuteTests : ExecuteMartialAttackCommandHandlerTests
    {
        [Test]
        [TestCase(RequestStatus.Failed)]
        [TestCase(RequestStatus.Canceled)]
        public async Task Should_Return_A_Command_Response_Equivalent_To_The_Query_When_Query_Failed(RequestStatus queryStatus)
        {
            // Arrange
            When_Query_Returns(status: queryStatus);
            
            // Act
            var result = await _commandHandler.Execute(_command);

            // Assert
            result.Status.Should().Be(queryStatus);
        }

        [Test]
        public async Task Should_Return_Success_When_Everything_Went_Smooth()
        {
            // Act
            var result = await _commandHandler.Execute(_command);

            // Assert
            result.Status.Should().Be(RequestStatus.Success);
        }

        [Test]
        public async Task Should_Store_Result_Of_Query_In_Command()
        {
            // Arrange
            var result = MartialAttackRollResultFactory.Build();
            When_Query_Returns(result);

            // Act
            await _commandHandler.Execute(_command);

            // Assert
            _command.MartialAttackRollResult.Should().Be(result);
        }

        [Test]
        public async Task Should_Get_Target_By_Id_In_The_MartialAttackRollResult()
        {
            // Act
            var result = MartialAttackRollResultFactory.Build(targetId: Guid.NewGuid());
            When_Query_Returns(result);

            await _commandHandler.Execute(_command);

            // Assert
            A.CallTo(() => _fightContext.GetCharacterById(result.TargetId))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_Not_Apply_Damage_When_The_Attack_Does_Not_Hit()
        {
            // Arrange
            var result = MartialAttackRollResultFactory.Build(hitRollResult: new HitRollResult() { Result = 3 });
            When_Query_Returns(result);

            // Act
            await _commandHandler.Execute(_command);

            // 
            A.CallTo(() => _mediator.Execute(A<ApplyDamageRollResultsCommand>._, null))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Should_Apply_Rolled_Damage_When_The_Attack_Does_Hit()
        {
            // Arrange
            var result = 
                MartialAttackRollResultFactory.Build(
                    hitRollResult: new HitRollResult() { Result = 17 },
                    damageRollResult: [
                        DamageRollResultFactory.BuildRolledDice(
                            damageType: DamageTypeEnum.Thunder, 
                            damage: 7) ]);
            When_Query_Returns(result);

            // Act
            await _commandHandler.Execute(_command);

            // 
            A.CallTo(() => _mediator.Execute(An<ApplyDamageRollResultsCommand>.That.Matches(x => 
                x.DamageRolls.Count(d => d.DamageType == DamageTypeEnum.Thunder && d.Damage == 7) == 1
            ), null))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_Try_Applying_Status_When_The_Attack_Does_Hit()
        {
            // Arrange
            var result = MartialAttackRollResultFactory.Build(hitRollResult: new HitRollResult() { Result = 17 });
            When_Query_Returns(result);

            // Act
            await _commandHandler.Execute(_command);

            // Assert
            A.CallTo(() => _mediator.Execute(An<TryApplyStatusCommand>.That.Matches(x => x.StatusId == _attackTemplate.Statuses.First().Value.Id), null))
                .MustHaveHappenedOnceExactly();
        }
    }

    [TestFixture]
    public class RedoTests : ExecuteMartialAttackCommandHandlerTests
    {
        [Test]
        public async Task Should_Recompute_If_The_Attack_Hits_In_Case_Target_Got_Updated()
        {
            // Arrange
            _target.ArmorClass.BaseArmorClass = 20;
            _target.HitPoints.CurrentHps = 100;
            var result =
                MartialAttackRollResultFactory.Build(
                    targetId: _target.Id,
                    hitRollResult: new HitRollResult() { Result = 17 },
                    damageRollResult: [
                        DamageRollResultFactory.BuildRolledDice(
                            damageType: DamageTypeEnum.Thunder,
                            damage: 7) ]);
            When_Query_Returns(result);

            await _commandHandler.Execute(_command);
            A.CallTo(() => _mediator.Execute(A<ApplyDamageRollResultsCommand>._, null))
                .MustNotHaveHappened();
            _commandHandler.Undo(_command);

            _target.ArmorClass.BaseArmorClass = 1;

            // Act
            await _commandHandler.Redo(_command);

            // Assert
            A.CallTo(() => _mediator.Execute(A<ApplyDamageRollResultsCommand>.That.Matches(x => x.DamageRolls.First().Damage == 7), null))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_Not_Redo_Query_When_Template_Is_The_Same()
        {
            // Arrange
            var result = MartialAttackRollResultFactory.Build();
            When_Query_Returns(result);

            await _commandHandler.Execute(_command);
            _commandHandler.Undo(_command);

            // Act
            await _commandHandler.Redo(_command);

            // Assert
            A.CallTo(() => _mediator.Execute(A<MartialAttackRollResultQuery>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_Redo_Even_The_Query_When_Template_Got_Updated()
        {
            // Arrange
            When_Query_Returns(MartialAttackRollResultFactory.Build());
            await _commandHandler.Execute(_command);
            _commandHandler.Undo(_command);
            _attackTemplate.Name = "New name to change hash value";
            A.CallTo(() => _mediator.Execute(A<MartialAttackRollResultQuery>._))
                .MustHaveHappenedOnceExactly();
            // Act
            await _commandHandler.Redo(_command);

            // Assert
            A.CallTo(() => _mediator.Execute(A<MartialAttackRollResultQuery>._))
                .MustHaveHappenedTwiceExactly();
        }

        [Test]
        public async Task Should_Update_Hash_When_Template_Got_Updated()
        {
            // Arrange
            When_Query_Returns(MartialAttackRollResultFactory.Build());
            await _commandHandler.Execute(_command);
            var firstHash = _command.AttackTemplateHash;
            _commandHandler.Undo(_command);
            _attackTemplate.Name = "New name to change hash value";

            // Act
            await _commandHandler.Redo(_command);

            // Assert
            firstHash.Should().NotBe(_command.AttackTemplateHash);
        }

        [Test]
        public async Task Should_Clear_Sub_Commands_When_The_Attack_Does_Not_Hit()
        {
            // Arrange
            _target.ArmorClass.BaseArmorClass = 1;
            var result = MartialAttackRollResultFactory.Build(hitRollResult: new HitRollResult() { Result = 10 }, targetId: _target.Id);
            When_Query_Returns(result);

            await _commandHandler.Execute(_command);
            _commandHandler.Undo(_command);

            _target.ArmorClass.BaseArmorClass = 20;

            // Act
            await _commandHandler.Redo(_command);

            // Assert
            _command.SubCommands.Should().BeEmpty();
        }
    }

}
