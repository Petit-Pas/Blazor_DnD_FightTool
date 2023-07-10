using DnDActions.DamageActions.ApplyDamageRollResults;
using DnDEntities.Characters;
using Fight.Damage;
using Fight;
using NUnit.Framework;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;
using System.Threading.Tasks;
using DnDActions.MartialAttackActions.ExecuteMartialAttack;
using DnDEntities.Damage;
using DnDEntities.DamageAffinities;
using FakeItEasy;
using FightTestsUtilities.Factories.Damage;
using Fight.MartialAttacks;
using System.Linq;
using DnDQueries.MartialAttackQueries;
using UndoableMediator.Queries;
using FluentAssertions;
using DnDEntities.Dices.DiceThrows;

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

    private DamageRollResult[] _damageRollResults = null!;
    private HitRollResult _hitRollResult = new HitRollResult() { Result = 10 };
    private RequestStatus _attackRollResultQueryStatus;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>();
        _fightContext = A.Fake<IFightContext>();
        _attackRollResultQueryStatus = RequestStatus.Success;

        _caster = new Character(true);
        _caster.MartialAttacks.Add(new MartialAttackTemplate());
        _target = new Character(true);

        _damageRollResults = new DamageRollResult[]
        {
            DamageRollResultFactory.Build(DamageTypeEnum.Thunder, 7),
        };

        _target.DamageAffinities = new DamageAffinitiesCollection(true);

        _command = new ExecuteMartialAttackCommand(_caster.Id, _attackTemplate.Id);
        _commandHandler = new ExecuteMartialAttackCommandHandler(_mediator, _fightContext);

        A.CallTo(() => _fightContext.GetCharacterById(_caster.Id))
            .Returns(_caster);
        A.CallTo(() => _fightContext.GetCharacterById(_target.Id))
            .Returns(_target);

        A.CallTo(() => _mediator.Execute(A<MartialAttackRollResultQuery>._))
            .ReturnsLazily((_) => Task.FromResult(BuildQueryResponse()));

    }

    MartialAttackTemplate _attackTemplate => _caster.MartialAttacks.First();

    IQueryResponse<MartialAttackRollResult> BuildQueryResponse()
    {
        var rollResult = new MartialAttackRollResult(_hitRollResult, _damageRollResults)
        {
            TargetId = _target.Id,
        };

        return _attackRollResultQueryStatus switch
        {
            RequestStatus.Success => QueryResponse<MartialAttackRollResult>.Success(rollResult),
            RequestStatus.Canceled => QueryResponse<MartialAttackRollResult>.Canceled(rollResult),
            RequestStatus.Failed => QueryResponse<MartialAttackRollResult>.Failed(rollResult),
            _ => throw new System.NotImplementedException(),
        };
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
            _attackRollResultQueryStatus = queryStatus;

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
            // Act
            await _commandHandler.Execute(_command);

            // Assert
            _command.MartialAttackRollResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_Get_Target_By_Id_In_The_MartialAttackRollResult()
        {
            // Act
            await _commandHandler.Execute(_command);

            // Assert
            A.CallTo(() => _fightContext.GetCharacterById(_target.Id))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_Not_Apply_Damage_When_The_Attack_Does_Not_Hit()
        {
            // Arrange
            _hitRollResult.Result = 3;

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
            _hitRollResult.Result = 17;

            // Act
            await _commandHandler.Execute(_command);

            // 
            A.CallTo(() => _mediator.Execute(An<ApplyDamageRollResultsCommand>.That.Matches(x => 
                x.DamageRolls.Count(d => d.DamageType == DamageTypeEnum.Thunder && d.Damage == 7) == 1
            ), null))
                .MustHaveHappenedOnceExactly();
        }
    }

}
