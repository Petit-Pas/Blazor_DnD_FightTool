using DnDFightTool.Business.DnDActions.HitPointActions;
using FakeItEasy;
using DnDFightTool.Domain.Fight;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace DnDActionsTests.HitPointActionsTests;

[TestFixture]
public class HitPointCommandBaseTests
{
    private IFightContext _fightContext = null!;
    private HitPointCommandBase _command = null!;

    [SetUp]
    public void SetUp()
    {
        _fightContext = A.Fake<IFightContext>();
        _command = new HitPointCommandBase(Guid.NewGuid());

        A.CallTo(() => _fightContext[A<Guid>._])
            .Returns(null!);
    }
}
