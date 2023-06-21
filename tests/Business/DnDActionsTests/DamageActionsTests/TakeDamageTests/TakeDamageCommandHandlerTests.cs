using DnDActions.DamageActions.TakeDamage;
using DnDActions.HitPointActions.LooseHp;
using DnDActions.HitPointActions.LooseTempHp;
using DnDEntities.Characters;
using DnDEntities.HitPoint;
using FakeItEasy;
using Fight;
using FluentAssertions;
using NUnit.Framework;
using System;
using UndoableMediator.Mediators;

namespace DnDActionsTests.DamageActionsTests.TakeDamageTests;

[TestFixture]
public class TakeDamageCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IFightContext _fightContext = null!;

    private Character _character = null!;

    private TakeDamageCommand _command = null!;
    private TakeDamageCommandHandler _commandHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>();
        _fightContext = A.Fake<IFightContext>();

        _character = new Character();
        _character.HitPoints = new HitPoints() { MaxHps = 25, CurrentHps = 12 };

        _command = new TakeDamageCommand(Guid.NewGuid(), 10);
        _commandHandler = new TakeDamageCommandHandler(_mediator, _fightContext);

        A.CallTo(() => _fightContext.GetCharacterById(A<Guid>._))
            .Returns(_character);
    }

    private HitPoints _hps { get => _character.HitPoints; }

    [TestFixture]
    public class ExecuteTests : TakeDamageCommandHandlerTests
    {
        [Test]
        public void Should_Execute_A_Command_To_Reduce_Hps()
        {
            // Act
            _commandHandler.Execute(_command);

            // Assert
            A.CallTo(() => _mediator.Execute(A<LooseHpCommand>.That.Matches(x => x.Amount == 10), null!))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Should_Not_Execute_A_Command_To_Remove_Temp_Hps_When_There_Is_None()
        {
            // Act
            _commandHandler.Execute(_command);

            // Assert
            A.CallTo(() => _mediator.Execute(A<LooseTempHpCommand>._, null!))
                .MustNotHaveHappened();
        }

        [Test]
        public void Should_Execute_A_Command_To_Remove_Temp_Hps_When_There_Are_Some()
        {
            // Arrange
            _hps.CurrentTempHps = 2;

            // Act
            _commandHandler.Execute(_command);

            // Assert
            A.CallTo(() => _mediator.Execute(A<LooseHpCommand>.That.Matches(x => x.Amount == 8), null!))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _mediator.Execute(A<LooseTempHpCommand>.That.Matches(x => x.Amount == 2), null!))
                .MustHaveHappenedOnceExactly();
        }
    }

    [TestFixture]
    public class RedoTests : TakeDamageCommandHandlerTests
    {
        // Smoke test to double check that it executes the command well, most basic Execute test.
        [Test]
        public void Should_Execute_A_Command_To_Reduce_Hps()
        {
            // Act
            _commandHandler.Execute(_command);

            // Assert
            A.CallTo(() => _mediator.Execute(A<LooseHpCommand>.That.Matches(x => x.Amount == 10), null!))
                .MustHaveHappenedOnceExactly();
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
