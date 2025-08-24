using DnDFightTool.Business.DnDActions;
using FakeItEasy;
using DnDFightTool.Domain.Fight;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace DnDActionsTests;

[TestFixture]
public class TargetCasterCommandBaseTests
{
    private IFightContext _fightContext = null!;
    private CasterTargetCommandBase _command = null!;

    [SetUp]
    public void SetUp()
    {
        _fightContext = A.Fake<IFightContext>();
        _command = new CasterTargetCommandBase(Guid.NewGuid(), Guid.NewGuid());

        A.CallTo(() => _fightContext[A<Guid>._])
            .Returns(null!);
    }
}

