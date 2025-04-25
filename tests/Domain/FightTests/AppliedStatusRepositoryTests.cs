using System;
using DnDFightTool.Domain.DnDEntities.Statuses;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Events.AppliedStatusUpdated;
using DomainTestsUtilities.Factories.Status;
using FluentAssertions;
using NUnit.Framework;

namespace FightTests;

[TestFixture]
public class AppliedStatusRepositoryTests
{
    private AppliedStatusRepository _repository = null!;

    [SetUp]
    public void Setup()
    {
        _repository = [];
    }

    [TestFixture]
    private class AddTests : AppliedStatusRepositoryTests
    {

        [Test]
        public void Should_Add_Status()
        {
            // Arrange
            var status = AppliedStatusFactory.BuildAppliedStatus();
    
            // Act
            _repository.Add(status);

            // Assert
            _repository.Should().ContainKey(status.Id);
            _repository[status.Id].Should().Be(status);
        }

        [Test]
        public void Should_Notify_AppliedStatusUpdated()
        {
            // Arrange
            var status = AppliedStatusFactory.BuildAppliedStatus();
            AppliedStatusUpdatedEventArgs? eventArgs = null;
            _repository.AppliedStatusUpdated += (sender, args) => eventArgs = args;
    
            // Act
            _repository.Add(status);
    
            // Assert
            eventArgs.Should().NotBeNull();
            eventArgs!.AffectedCharacterId.Should().Be(status.TargetId);
        }
    }

    [TestFixture]
    private class RemoveIfExistsTests : AppliedStatusRepositoryTests
    {
        [Test]
        public void Should_Remove_Status()
        {
            // Arrange
            var status = AppliedStatusFactory.BuildAppliedStatus();
            _repository.Add(status);
    
            // Act
            _repository.RemoveIfExists(status.Id);
    
            // Assert
            _repository.Should().NotContainKey(status.Id);
        }

        [Test]
        public void Should_Notify_AppliedStatusUpdated()
        {
            // Arrange
            var status = AppliedStatusFactory.BuildAppliedStatus();
            _repository.Add(status);
            AppliedStatusUpdatedEventArgs? eventArgs = null;
            _repository.AppliedStatusUpdated += (sender, args) => eventArgs = args;
    
            // Act
            _repository.RemoveIfExists(status.Id);
    
            // Assert
            eventArgs.Should().NotBeNull();
            eventArgs!.AffectedCharacterId.Should().Be(status.TargetId);
        }

        [Test]
        public void Should_Not_Notify_AppliedStatusUpdated_If_Status_Does_Not_Exist()
        {
            // Arrange
            var status = AppliedStatusFactory.BuildAppliedStatus();
            _repository.Add(status);
            AppliedStatusUpdatedEventArgs? eventArgs = null;
            _repository.AppliedStatusUpdated += (sender, args) => eventArgs = args;
    
            // Act
            _repository.RemoveIfExists(Guid.NewGuid());
    
            // Assert
            eventArgs.Should().BeNull();
        }
    }

    [TestFixture]
    public class GetStatusAppliedToTests : AppliedStatusRepositoryTests
    {
        [Test]
        public void Should_Return_Statuses_Applied_To_Character()
        {
            // Arrange
            var status1 = AppliedStatusFactory.BuildAppliedStatus(targetId: Guid.NewGuid());
            var status2 = AppliedStatusFactory.BuildAppliedStatus(targetId: Guid.NewGuid());
            var status3 = AppliedStatusFactory.BuildAppliedStatus(targetId: Guid.NewGuid());
            _repository.Add(status1);
            _repository.Add(status2);
            _repository.Add(status3);
    
            // Act
            var statuses = _repository.GetStatusAppliedTo(status2.TargetId);
    
            // Assert
            statuses.Should().Contain(status2);
            statuses.Should().NotContain(status1);
            statuses.Should().NotContain(status3);
        }
    }
}
