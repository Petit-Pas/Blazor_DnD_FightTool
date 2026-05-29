using DnDFightTool.Business.DnDActions.HitPointActions.LooseHp;
using DnDFightTool.Business.DnDActions.LogActions.WriteLog;
using DnDFightTool.Domain.CharacterSheet.HitPoint;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;

namespace DnDActionsTests.HitPointActionsTests.LooseHpTests;

[TestFixture]
internal class LooseHpCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IFightContext _fightContext = null!;

    private IFightingCharacter _character = null!;
    private HitPoints _hitPoints = null!;

    private LooseHpCommand _command = null!;
    private LooseHpCommandHandler _commandHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>(options => options.Strict().Implements<ISubCommandDispatcher>());
        _fightContext = A.Fake<IFightContext>(options => options.Strict());

        _hitPoints = new HitPoints { MaxHps = 25, CurrentHps = 12 };
        _character = A.Fake<IFightingCharacter>(options => options.Strict());
        A.CallTo(() => _character.HitPoints).Returns(_hitPoints);
        A.CallTo(() => _character.Name).Returns("Goblin");

        _command = new LooseHpCommand(Guid.NewGuid(), 10);
        _commandHandler = new LooseHpCommandHandler(_mediator, _fightContext);

        A.CallTo(() => _fightContext[A<Guid>._]).Returns(_character);

        A.CallTo(() => _mediator.SendAsSubCommandAsync(A<LooseHpAtomicCommand>._, A<LooseHpCommand>._))
            .Returns(Task.FromResult<ICommandResponse<int>>(CommandResponse.Success<int>(7)));
        A.CallTo(() => _mediator.SendAsSubCommandAsync(A<WriteLogCommand>._, A<LooseHpCommand>._))
            .Returns(Task.FromResult<ICommandResponse<NoResponse>>(CommandResponse.Success()));
    }

    [TestFixture]
    private class ExecuteTests : LooseHpCommandHandlerTests
    {
        [Test]
        public async Task Should_Return_Success()
        {
            var response = await _commandHandler.ExecuteAsync(_command);
            response.Status.Should().Be(RequestStatus.Success);
        }

        [Test]
        public async Task Should_Dispatch_LooseHpAtomicCommand()
        {
            await _commandHandler.ExecuteAsync(_command);

            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                A<LooseHpAtomicCommand>.That.Matches(x => x.Amount == _command.Amount),
                A<LooseHpCommand>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_Send_WriteLogCommand_With_HpLoss()
        {
            await _commandHandler.ExecuteAsync(_command);

            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                A<WriteLogCommand>.That.Matches(x => x.Content.Contains("loses") && x.Content.Contains("HPs") && x.Content.Contains("7")),
                A<LooseHpCommand>._))
                .MustHaveHappenedOnceExactly();
        }
    }

    [TestFixture]
    private class RedoTests : LooseHpCommandHandlerTests
    {
        [Test]
        public async Task Should_Clear_SubCommands_And_Re_Execute()
        {
            await _commandHandler.RedoAsync(_command);

            A.CallTo(() => _mediator.SendAsSubCommandAsync(A<LooseHpAtomicCommand>._, A<LooseHpCommand>._))
                .MustHaveHappenedOnceExactly();
        }
    }
}
