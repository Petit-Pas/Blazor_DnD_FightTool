using System;
using DnDFightTool.Business.DnDActions;
using DnDFightTool.Domain.Fight;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

namespace DnDActionsTests;

[TestFixture]
public class TargetCommandBaseTests
{
    private IFightContext _fightContext = null!;
    private TargetCommandBase _command = null!;

    [SetUp]
    public void SetUp()
    {
        _fightContext = A.Fake<IFightContext>();
        _command = new TargetCommandBase(Guid.NewGuid());

        A.CallTo(() => _fightContext[A<Guid>._])
            .Returns(null!);
    }
}
