using System;
using System.Threading.Tasks;
using DnDFightTool.Business.DnDActions.StatusActions.ApplyStatus;
using DnDFightTool.Business.DnDActions.StatusActions.TryApplyStatus;
using DnDFightTool.Business.DnDQueries.SaveQueries;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using DnDFightTool.Domain.DnDEntities.Saves;
using DnDFightTool.Domain.DnDEntities.Statuses;
using DnDFightTool.Domain.Fight;
using DomainTestsUtilities.Factories.Saves;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Memory.Hashes;
using UndoableMediator.Mediators;
using UndoableMediator.Queries;
using UndoableMediator.Requests;

namespace DnDActionsTests.StatusActionsTests.TryApplyStatusTests
{
    [TestFixture]
    public class TryApplyStatusCommandHandlerTests
    {
        [TestFixture]
        public class ExecuteTests : TryApplyStatusCommandHandlerTests
        {
            private IUndoableMediator _mediator = null!;
            private IFightContext _fightContext = null!;

            private TryApplyStatusCommand _command = null!;
            private TryApplyStatusCommandHandler _commandHandler = null!;

            [SetUp]
            public void SetUp()
            {
                _mediator = A.Fake<IUndoableMediator>();
                _fightContext = A.Fake<IFightContext>();

                _command = new TryApplyStatusCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
                _commandHandler = new TryApplyStatusCommandHandler(_mediator, _fightContext);

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

            private void WhenQueryReturns(IQueryResponse<SaveRollResult> saveRollResult)
            {
                A.CallTo(() => _mediator.Execute(A<SaveRollResultQuery>.Ignored))
                    .Returns(saveRollResult);
            }

            [Test]
            public async Task Should_Not_Query_SaveRoll_When_Status_Must_Be_Applied_Automatically()
            {
                // Arrange
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
                                        Id = _command.StatusId,
                                        IsAppliedAutomatically = true
                                    }
                                }
                            }
                        }
                    });

                // Act
                await _commandHandler.Execute(_command);

                // Assert
                A.CallTo(() => _mediator.Execute(A<SaveRollResultQuery>._))
                    .MustNotHaveHappened();
            }

            [Test]
            public async Task Should_Fail_When_SaveRollResultQuery_Returns_Error()
            {
                // Arrange
                WhenQueryReturns(QueryResponse<SaveRollResult>.Failed(SaveRollResultFactory.Build()));

                // Act
                var result = await _commandHandler.Execute(_command);

                // Assert
                result.Status.Should().Be(RequestStatus.Failed);
            }

            [Test]
            public async Task Should_Cancel_When_SaveRollResultQuery_Returns_Error()
            {
                // Arrange
                WhenQueryReturns(QueryResponse<SaveRollResult>.Canceled(SaveRollResultFactory.Build()));

                // Act
                var result = await _commandHandler.Execute(_command);

                // Assert
                result.Status.Should().Be(RequestStatus.Canceled);
            }

            [Test]
            public async Task Should_Not_Apply_Status_When_It_Should_Not_Be_Applied()
            {
                // Arrange
                WhenQueryReturns(QueryResponse<SaveRollResult>.Success(SaveRollResultFactory.Build(rolledResult: 1)));

                // Act
                await _commandHandler.Execute(_command);

                // Assert
                A.CallTo(() => _mediator.Execute(A<ApplyStatusCommand>._, null))
                    .MustNotHaveHappened();
            }

            [Test]
            public async Task Should_Apply_Status_When_It_Should()
            {
                // Arrange
                WhenQueryReturns(QueryResponse<SaveRollResult>.Success(SaveRollResultFactory.Build(rolledResult: 1)));
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
                                        Id = _command.StatusId,
                                        IsAppliedAutomatically = true
                                    }
                                }
                            }
                        }
                    });

                // Act
                await _commandHandler.Execute(_command);

                // Assert
                A.CallTo(() => _mediator.Execute(A<ApplyStatusCommand>._, null))
                        .MustHaveHappenedOnceExactly();
            }

            [Test]
            public async Task Should_Store_Hash_Of_Status_In_Command()
            {
                // Arrange 
                var status = new StatusTemplate()
                {
                    Name = "TestStatus",
                    Id = _command.StatusId,
                    IsAppliedAutomatically = true,
                };
                A.CallTo(() => _fightContext.GetCharacterById(_command.CasterId))
                    .Returns(new Character()
                    {
                        Id = _command.CasterId,
                        MartialAttacks = new MartialAttackTemplateCollection(false)
                        {
                            new MartialAttackTemplate()
                            {
                                Statuses = new StatusTemplateCollection(false) { status }
                            }
                        }
                    });

                // Act 
                await _commandHandler.Execute(_command);

                // Assert
                _command.StatusHash.Should().Be(status.Hash());
            }
        }

        [TestFixture]
        public class RedoTests : TryApplyStatusCommandHandlerTests
        {
            private IUndoableMediator _mediator = null!;
            private IFightContext _fightContext = null!;

            private TryApplyStatusCommand _command = null!;
            private TryApplyStatusCommandHandler _commandHandler = null!;

            [SetUp]
            public void SetUp()
            {
                _mediator = A.Fake<IUndoableMediator>();
                _fightContext = A.Fake<IFightContext>();

                _command = new TryApplyStatusCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
                _commandHandler = new TryApplyStatusCommandHandler(_mediator, _fightContext);

                A.CallTo(() => _fightContext.GetCharacterById(_command.TargetId))
                    .Returns(new Character()
                    {
                        Id = _command.TargetId
                    });
            }


            [Test]
            public async Task Should_Not_Query_Anything_When_Status_Has_Not_Changed()
            {
                // Arrange 
                var status = new StatusTemplate()
                {
                    Name = "TestStatus",
                    Id = _command.StatusId,
                };
                A.CallTo(() => _fightContext.GetCharacterById(_command.CasterId))
                    .Returns(new Character()
                    {
                        Id = _command.CasterId,
                        MartialAttacks = new MartialAttackTemplateCollection(false)
                        {
                            new MartialAttackTemplate()
                            {
                                Statuses = new StatusTemplateCollection(false) { status }
                            }
                        }
                    });
                _command.StatusHash = status.Hash();

                // Act
                await _commandHandler.Redo(_command);

                // Assert
                A.CallTo(() => _mediator.Execute(A<SaveRollResultQuery>._))
                    .MustNotHaveHappened();
            }

            [Test]
            public async Task Should_Query_SaveRoll_When_Status_Has_Changed()
            {
                // Arrange 
                var status = new StatusTemplate()
                {
                    Name = "TestStatus",
                    Id = _command.StatusId,
                };
                A.CallTo(() => _fightContext.GetCharacterById(_command.CasterId))
                    .Returns(new Character()
                    {
                        Id = _command.CasterId,
                        MartialAttacks = new MartialAttackTemplateCollection(false)
                        {
                            new MartialAttackTemplate()
                            {
                                Statuses = new StatusTemplateCollection(false) { status }
                            }
                        }
                    });

                // Act
                await _commandHandler.Redo(_command);

                // Assert
                A.CallTo(() => _mediator.Execute(A<SaveRollResultQuery>._))
                    .MustHaveHappenedOnceExactly();
            }

            [Test]
            public async Task Should_Execute_Apply_Status_Command()
            {
                // Arrange 
                var status = new StatusTemplate()
                {
                    Name = "TestStatus",
                    Id = _command.StatusId,
                    IsAppliedAutomatically = true,
                };
                A.CallTo(() => _fightContext.GetCharacterById(_command.CasterId))
                    .Returns(new Character()
                    {
                        Id = _command.CasterId,
                        MartialAttacks = new MartialAttackTemplateCollection(false)
                        {
                            new MartialAttackTemplate()
                            {
                                Statuses = new StatusTemplateCollection(false) { status }
                            }
                        }
                    });

                // Act
                await _commandHandler.Redo(_command);

                // Assert
                A.CallTo(() => _mediator.Execute(A<ApplyStatusCommand>._, null))
                    .MustHaveHappenedOnceExactly();
            }
        }
    }
}
