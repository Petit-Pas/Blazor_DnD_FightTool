using DnDFightTool.Domain.Logs;
using FluentAssertions;
using NUnit.Framework;

namespace LogsTests;

[TestFixture]
public class DnDLogServiceTests
{
    private DnDLogService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _sut = new DnDLogService();
    }

    [TestFixture]
    public class AddEntryTests : DnDLogServiceTests
    {
        [Test]
        public void Should_AddEntry_ToCurrentBlock()
        {
            // Arrange
            _sut.OpenBlock("Test Block");

            // Act
            var id = _sut.AddEntry("Hello world");

            // Assert
            _sut.Blocks.Should().HaveCount(1);
            _sut.Blocks[0].Entries.Should().HaveCount(1);
            _sut.Blocks[0].Entries[0].Id.Should().Be(id);
            _sut.Blocks[0].Entries[0].Content.Should().Be("Hello world");
            _sut.Blocks[0].Entries[0].IndentLevel.Should().Be(0);
        }

        [Test]
        public void Should_ThrowInvalidOperationException_When_NoBlockOpen()
        {
            // Act
            var act = () => _sut.AddEntry("No block");

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void Should_ReturnUniqueIds_ForEachEntry()
        {
            // Arrange
            _sut.OpenBlock("Block");

            // Act
            var id1 = _sut.AddEntry("First");
            var id2 = _sut.AddEntry("Second");

            // Assert
            id1.Should().NotBe(id2);
        }

        [Test]
        public void Should_FireOnChanged_When_EntryAdded()
        {
            // Arrange
            _sut.OpenBlock("Block");
            var fired = false;
            _sut.OnChanged += () => fired = true;

            // Act
            _sut.AddEntry("Entry");

            // Assert
            fired.Should().BeTrue();
        }
    }

    [TestFixture]
    public class HideShowTests : DnDLogServiceTests
    {
        [Test]
        public void Should_HideEntry()
        {
            // Arrange
            _sut.OpenBlock("Block");
            var id = _sut.AddEntry("Entry");

            // Act
            _sut.Hide(id);

            // Assert
            _sut.IsHidden(id).Should().BeTrue();
        }

        [Test]
        public void Should_ShowEntry_AfterHiding()
        {
            // Arrange
            _sut.OpenBlock("Block");
            var id = _sut.AddEntry("Entry");
            _sut.Hide(id);

            // Act
            _sut.Show(id);

            // Assert
            _sut.IsHidden(id).Should().BeFalse();
        }

        [Test]
        public void Should_BeIdempotent_When_HidingAlreadyHidden()
        {
            // Arrange
            _sut.OpenBlock("Block");
            var id = _sut.AddEntry("Entry");
            _sut.Hide(id);
            var changeCount = 0;
            _sut.OnChanged += () => changeCount++;

            // Act
            _sut.Hide(id);

            // Assert
            changeCount.Should().Be(0);
        }

        [Test]
        public void Should_BeIdempotent_When_ShowingAlreadyVisible()
        {
            // Arrange
            _sut.OpenBlock("Block");
            var id = _sut.AddEntry("Entry");
            var changeCount = 0;
            _sut.OnChanged += () => changeCount++;

            // Act
            _sut.Show(id);

            // Assert
            changeCount.Should().Be(0);
        }

        [Test]
        public void Should_ReturnFalse_When_IsHidden_CalledWithUnknownId()
        {
            // Act & Assert
            _sut.IsHidden(Guid.NewGuid()).Should().BeFalse();
        }

        [Test]
        public void Should_FireOnChanged_When_Hidden()
        {
            // Arrange
            _sut.OpenBlock("Block");
            var id = _sut.AddEntry("Entry");
            var fired = false;
            _sut.OnChanged += () => fired = true;

            // Act
            _sut.Hide(id);

            // Assert
            fired.Should().BeTrue();
        }

        [Test]
        public void Should_FireOnChanged_When_Shown()
        {
            // Arrange
            _sut.OpenBlock("Block");
            var id = _sut.AddEntry("Entry");
            _sut.Hide(id);
            var fired = false;
            _sut.OnChanged += () => fired = true;

            // Act
            _sut.Show(id);

            // Assert
            fired.Should().BeTrue();
        }
    }

    [TestFixture]
    public class BlockTests : DnDLogServiceTests
    {
        [Test]
        public void Should_OpenBlock_WithName()
        {
            // Act
            _sut.OpenBlock("Martial Attack");

            // Assert
            _sut.Blocks.Should().HaveCount(1);
            _sut.Blocks[0].Name.Should().Be("Martial Attack");
        }

        [Test]
        public void Should_ThrowInvalidOperationException_When_OpeningBlockWhileAnotherOpen()
        {
            // Arrange
            _sut.OpenBlock("First");

            // Act
            var act = () => _sut.OpenBlock("Second");

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void Should_ThrowInvalidOperationException_When_ClosingBlockWithNoneOpen()
        {
            // Act
            var act = () => _sut.CloseBlock();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void Should_AllowOpeningNewBlock_AfterClosing()
        {
            // Arrange
            _sut.OpenBlock("First");
            _sut.CloseBlock();

            // Act
            _sut.OpenBlock("Second");

            // Assert
            _sut.Blocks.Should().HaveCount(2);
            _sut.Blocks[1].Name.Should().Be("Second");
        }
    }

    [TestFixture]
    public class ScopeTests : DnDLogServiceTests
    {
        [Test]
        public void Should_IncrementIndentLevel_When_ScopeOpened()
        {
            // Arrange
            _sut.OpenBlock("Block");
            _sut.OpenScope();

            // Act
            _sut.AddEntry("Indented");

            // Assert
            _sut.Blocks[0].Entries[0].IndentLevel.Should().Be(1);
        }

        [Test]
        public void Should_DecrementIndentLevel_When_ScopeClosed()
        {
            // Arrange
            _sut.OpenBlock("Block");
            _sut.OpenScope();
            _sut.AddEntry("Indented");
            _sut.CloseScope();

            // Act
            _sut.AddEntry("Back to root");

            // Assert
            _sut.Blocks[0].Entries[1].IndentLevel.Should().Be(0);
        }

        [Test]
        public void Should_ThrowInvalidOperationException_When_ClosingScopeAtZero()
        {
            // Act
            var act = () => _sut.CloseScope();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void Should_SupportNestedScopes()
        {
            // Arrange
            _sut.OpenBlock("Block");
            _sut.OpenScope();
            _sut.OpenScope();

            // Act
            _sut.AddEntry("Double indented");

            // Assert
            _sut.Blocks[0].Entries[0].IndentLevel.Should().Be(2);
        }

        [Test]
        public void Should_ResetIndentLevel_When_BlockClosed()
        {
            // Arrange
            _sut.OpenBlock("Block");
            _sut.OpenScope();
            _sut.AddEntry("Indented");
            _sut.CloseBlock();
            _sut.OpenBlock("New Block");

            // Act
            _sut.AddEntry("Root level");

            // Assert
            _sut.Blocks[1].Entries[0].IndentLevel.Should().Be(0);
        }
    }

    [TestFixture]
    public class ClearTests : DnDLogServiceTests
    {
        [Test]
        public void Should_RemoveAllBlocksAndEntries()
        {
            // Arrange
            _sut.OpenBlock("Block");
            _sut.AddEntry("Entry");
            _sut.CloseBlock();

            // Act
            _sut.Clear();

            // Assert
            _sut.Blocks.Should().BeEmpty();
        }

        [Test]
        public void Should_ClearHiddenState()
        {
            // Arrange
            _sut.OpenBlock("Block");
            var id = _sut.AddEntry("Entry");
            _sut.Hide(id);
            _sut.CloseBlock();

            // Act
            _sut.Clear();

            // Assert
            _sut.IsHidden(id).Should().BeFalse();
        }

        [Test]
        public void Should_FireOnChanged_When_Cleared()
        {
            // Arrange
            var fired = false;
            _sut.OnChanged += () => fired = true;

            // Act
            _sut.Clear();

            // Assert
            fired.Should().BeTrue();
        }

        [Test]
        public void Should_AllowNewBlocks_AfterClear()
        {
            // Arrange
            _sut.OpenBlock("Old");
            _sut.AddEntry("Old entry");
            _sut.CloseBlock();
            _sut.Clear();

            // Act
            _sut.OpenBlock("New");
            _sut.AddEntry("New entry");

            // Assert
            _sut.Blocks.Should().HaveCount(1);
            _sut.Blocks[0].Name.Should().Be("New");
        }
    }
}
