---
applyTo: "tests/**/*.cs"
---

# Test Conventions

## Framework & Libraries

- **Test framework**: NUnit (v4.x) — `[TestFixture]`, `[Test]`, `[TestCase]`, `[SetUp]`.
- **Assertions**: FluentAssertions (v7.x) — `.Should().Be(...)`, `.Should().NotBeNull()`, etc.
- **Mocking**: FakeItEasy (v9.x) — `A.Fake<T>()`, `A.CallTo(() => ...).Returns(...)`, `A.CallTo(...).MustHaveHappenedOnceExactly()`.
  - **Always use strict fakes**: every `A.Fake<T>()` MUST use `options => options.Strict()`. This makes any unconfigured call throw immediately, preventing silent test pollution.
  - When the fake also needs to implement an extra interface, chain: `options => options.Strict().Implements<IExtra>()`.

## Structure

- **Test project naming**: `{ProjectName}Tests` matching the source project.
- **Namespace**: Mirrors the source namespace structure without the `DnDFightTool.` prefix.
- **Nested test fixtures**: Group related tests using nested classes that inherit from the outer fixture:
  ```csharp
  [TestFixture]
  public class MyHandlerTests
  {
      // shared SetUp, fields

      [TestFixture]
      public class ExecuteTests : MyHandlerTests { ... }

      [TestFixture]
      public class RedoTests : MyHandlerTests { ... }
  }
  ```
- **AAA pattern**: Arrange / Act / Assert with `// Arrange`, `// Act`, `// Assert` comments.
- **SetUp**: Use `[SetUp]` for shared initialization. Assign fields with `= null!` and initialize in `SetUp()`.

## Test Utilities (`DomainTestsUtilities`)

- **Factories**: Static factory classes in `Factories/{Entity}/` (e.g., `CharacterFactory.BuildMonster()`). Use optional parameters for customization.
- **Fakes**: Custom fake implementations in `Fakes/` for complex domain types that can't be easily mocked (e.g., `FakeSaveRollResult`).
- **Extensions**: Helper extension methods in `Extensions/` (e.g., `CharactersExtensions.AsFighter()`).

## Naming

- **Test class**: `{ClassUnderTest}Tests`.
- **Test method**: `Should_{ExpectedBehavior}` or `Should_{ExpectedBehavior}_When_{Condition}`.
- **Parameterized**: Use `[TestCase]` for value-driven tests with inline data.
