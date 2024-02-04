using System;
using System.Threading.Tasks;
using DnDFightTool.Business.DnDActions.StatusActions.ApplyStatus;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using DnDFightTool.Domain.DnDEntities.Statuses;
using DnDFightTool.Domain.Fight;
using DomainTestsUtilities.Factories.Saves;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using UndoableMediator.Mediators;

namespace DnDActionsTests.StatusActionsTests.ApplyStatusTests;

[TestFixture]
public class ApplyStatusCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IFightContext _fightContext = null!;
    private IAppliedStatusRepository _appliedStatusRepository = null!;

    private ApplyStatusCommand _command = null!;
    private ApplyStatusCommandHandler _commandHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>();
        _fightContext = A.Fake<IFightContext>();
        _appliedStatusRepository = A.Fake<IAppliedStatusRepository>();

        _command = new ApplyStatusCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), SaveRollResultFactory.Build());
        _commandHandler = new ApplyStatusCommandHandler(_mediator, _fightContext, _appliedStatusRepository);

        A.CallTo(() => _fightContext.GetCharacterById(_command.CasterId))
            .Returns(new Character() 
            { 
                Id = _command.CasterId,
                MartialAttacks = new MartialAttackTemplateCollection(false)
                {
                    new MartialAttackTemplate()
                    {
                        Statuses = new StatusTemplateCollection(false)
                        {
                            new StatusTemplate()
                            {
                                Name = "TestStatus",
                                Id = _command.StatusId
                            }
                        }
                    }
                }
            });

        A.CallTo(() => _fightContext.GetCharacterById(_command.TargetId))
            .Returns(new Character()
            {
                Id = _command.TargetId
            });
    }

    [TestFixture]
    public class ExecuteTests : ApplyStatusCommandHandlerTests
    {

        [SetUp]
        public async Task Setup()
        {
            await _commandHandler.Execute(_command);
        }

        [Test]
        [Order(1)]
        public void Should_Add_Status_To_Collection()
        {
            A.CallTo(() => _appliedStatusRepository.Add(An<AppliedStatus>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        [Order(2)]
        public void Status_Should_Have_Proper_CasterId()
        {
            A.CallTo(() => _appliedStatusRepository.Add(An<AppliedStatus>.That.Matches(x => x.CasterId == _command.CasterId)))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        [Order(2)]
        public void Status_Should_Have_Proper_TargetId()
        {
            A.CallTo(() => _appliedStatusRepository.Add(An<AppliedStatus>.That.Matches(x => x.TargetId == _command.TargetId)))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        [Order(2)]
        public void AppliedStatusId_Should_Be_Stored_In_Command()
        {
            A.CallTo(() => _appliedStatusRepository.Add(An<AppliedStatus>.That.Matches(x => x.Id == _command.AppliedStatusId)))
                .MustHaveHappenedOnceExactly();
        }
    }
    
    [TestFixture]
    public class UndoTests : ApplyStatusCommandHandlerTests
    {
        [Test]
        public void Should_Remove_Status_From_Collection()
        {
            // Act
            _commandHandler.Undo(_command);

            // Assert
            A.CallTo(() => _appliedStatusRepository.RemoveIfExists(_command.AppliedStatusId))
                .MustHaveHappenedOnceExactly();
        }
    }
}
