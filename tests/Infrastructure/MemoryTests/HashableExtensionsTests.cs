using FluentAssertions;
using Memory.Hashes;
using NUnit.Framework;

namespace MemoryTests;

[TestFixture]
public class HashableExtensionsTests
{
    private class ClassWithNormalProperties : IHashable
    {
        public ClassWithNormalProperties(int integer, string @string)
        {
            Integer = integer;
            String = @string;
        }

        public int Integer { get; set; }
        public string String { get; set; }
    }

    private class ClassWithNestedHashable : IHashable
    {
        public ClassWithNestedHashable(int integer, int nestedInt, string @string)
        {
            Integer = integer;
            NestedHashable = new ClassWithNormalProperties(nestedInt, @string);
        }

        public ClassWithNormalProperties NestedHashable { get; set; }
        public int Integer { get; set; }
    }

    private class ClassWithListOfNormalProperties : IHashable
    {
        public ClassWithListOfNormalProperties(int integer, List<int> integerList)
        {
            Integer = integer;
            IntegerList = integerList;
        }

        public int Integer { get; set; }
        public List<int> IntegerList { get; set; }
    }

    private class ClassWithListOfHashableProperties : IHashable
    {
        public ClassWithListOfHashableProperties(int integer, ClassWithNormalProperties[] hashableList)
        {
            Integer = integer;
            HashableList = hashableList;
        }

        public int Integer { get; set; }
        public ClassWithNormalProperties[] HashableList { get; set; }
    }

    private class ClassThatIsAListOfNormalType : List<int>, IHashable
    {
        public ClassThatIsAListOfNormalType(int integer)
        {
            Integer = integer;
        }

        public int Integer { get; set; }
    }

    private class ClassThatIsAListOfHashableTypes : List<ClassWithNormalProperties>, IHashable
    {
        public ClassThatIsAListOfHashableTypes(int integer)
        {
            Integer = integer;
        }

        public int Integer { get; set; }
    }

    [TestFixture]
    private class ClassWithNormalPropertiesTests : HashableExtensionsTests
    {
        private ClassWithNormalProperties _class = null!;

        [SetUp]
        public void Setup()
        {
            _class = new ClassWithNormalProperties(10, "hello world");
        }

        [Test]
        public void Hashing_A_Class_Twice_Should_Give_The_Same_Result()
        {
            // Arrange

            // Act
            var hash1 = _class.Hash();
            var hash2 = _class.Hash();

            // Assert
            hash1.Should().Be(hash2);
        }

        [Test]
        public void Hashing_Classes_With_Same_Properties_Should_Give_Same_Result()
        {
            // Arrange
            var class2 = new ClassWithNormalProperties(10, "hello world");

            // Act
            var hash1 = _class.Hash();
            var hash2 = class2.Hash();

            // Assert
            hash1.Should().Be(hash2);
        }

        [Test]
        public void Changing_Any_Property_Should_Change_Hash()
        {
            // Arrange
            var initialHash = _class.Hash();

            // Act
            _class.Integer += 1;
            var hash1 = _class.Hash();
            _class.Integer -= 1;

            _class.String += "!";
            var hash2 = _class.Hash();

            // Assert
            hash1.Should().NotBe(initialHash);
            hash2.Should().NotBe(initialHash);
        }
    }

    [TestFixture]
    private class ClassWithNestedHashableTests : HashableExtensionsTests
    {
        private ClassWithNestedHashable _class = null!;

        [SetUp]
        public void Setup()
        {
            _class = new ClassWithNestedHashable(15, 10, "hello world");
        }

        [Test]
        public void Changing_A_Property_Of_The_Nested_Object_Should_Affect_The_Hash()
        {
            // Arrange
            var initialHash = _class.Hash();

            // Act
            _class.NestedHashable.Integer += 1;
            var newHash = _class.Hash();

            // Assert
            newHash.Should().NotBe(initialHash);
        }
    }

    [TestFixture]
    private class ClassWithListOfNormalPropertiesTests : HashableExtensionsTests
    {
        private ClassWithListOfNormalProperties _class = null!;

        [SetUp]
        public void Setup()
        {
            _class = new ClassWithListOfNormalProperties(10, [1, 2, 3, 4]);
        }

        [Test]
        public void Editing_An_Element_Of_The_List_Should_Update_The_Hash()
        {
            // Arrange
            var initialHash = _class.Hash();

            // Act
            _class.IntegerList[2] = 0;
            var newHash = _class.Hash();

            // Assert
            newHash.Should().NotBe(initialHash);
        }
    }

    [TestFixture]
    private class ClassWithListOfHashablePropertiesTests : HashableExtensionsTests
    {
        private ClassWithListOfHashableProperties _class = null!;

        [SetUp]
        public void Setup()
        {
            _class = new ClassWithListOfHashableProperties(10, 
            [
                new (10, "hello world"),
                new (11, "John Doe"),
                new (33, "an apple a day")
            ]);
        }

        [Test]
        public void Editing_An_Elements_Property_Of_The_List_Should_Update_The_Hash()
        {
            // Arrange
            var initialHash = _class.Hash();

            // Act
            _class.HashableList[2].Integer = 0;
            var newHash = _class.Hash();

            // Assert
            newHash.Should().NotBe(initialHash);
        }
    }

    [TestFixture]
    private class ClassThatIsAListOfNormalTypeTests : HashableExtensionsTests
    {
        private ClassThatIsAListOfNormalType _class = null!;

        [SetUp]
        public void Setup()
        {
            _class = new ClassThatIsAListOfNormalType(10)
            {
                1, 2, 3, 4
            };
        }

        [Test]
        public void Editing_An_Element_Of_The_List_Should_Update_The_Hash()
        {
            // Arrange
            var initialHash = _class.Hash();

            // Act
            _class[2] = 0;
            var newHash = _class.Hash();

            // Assert
            newHash.Should().NotBe(initialHash);
        }

        [Test]
        public void Editing_A_Property_On_The_List_Itself_Should_Update_The_Hash()
        {
            // Arrange
            var initialHash = _class.Hash();

            // Act
            _class.Integer = 0;
            var newHash = _class.Hash();

            // Assert
            newHash.Should().NotBe(initialHash);
        }
    }

    [TestFixture]
    private class ClassThatIsAListOfHashableTypesTests : HashableExtensionsTests
    {
        private ClassThatIsAListOfHashableTypes _class = null!;

        [SetUp]
        public void Setup()
        {
            _class = new ClassThatIsAListOfHashableTypes(10)
            {
                new (10, "hello world"),
                new (11, "John Doe"),
                new (33, "an apple a day")
            };
        }

        [Test]
        public void Editing_An_Elements_Property_Of_The_List_Should_Update_The_Hash()
        {
            // Arrange
            var initialHash = _class.Hash();

            // Act
            _class[2].Integer = 0;
            var newHash = _class.Hash();

            // Assert
            newHash.Should().NotBe(initialHash);
        }


        [Test]
        public void Editing_A_Property_On_The_List_Itself_Should_Update_The_Hash()
        {
            // Arrange
            var initialHash = _class.Hash();

            // Act
            _class.Integer = 0;
            var newHash = _class.Hash();

            // Assert
            newHash.Should().NotBe(initialHash);
        }
    }

    [TestFixture]
    private class HashSpecifiedProperties : HashableExtensionsTests
    {
        private ClassWithNormalProperties _class = null!;

        [SetUp]
        public void SetUp()
        {
            _class = new ClassWithNormalProperties(10, "hello");
        }

        [Test]
        public void Editing_An_Element_Not_Hashed_Should_Not_Have_An_Impact()
        {
            // Arrange 
            var hash = _class.Hash([typeof(ClassWithNormalProperties).GetProperty(nameof(ClassWithNormalProperties.Integer))!]);
            _class.String = "world";

            // Act
            var newHash = _class.Hash([typeof(ClassWithNormalProperties).GetProperty(nameof(ClassWithNormalProperties.Integer))!]);

            // Assert
            newHash.Should().Be(hash);
        }

        [Test]
        public void Editing_a_Hashed_Element_Should_Have_An_Impact()
        {
            // Arrange 
            var hash = _class.Hash([typeof(ClassWithNormalProperties).GetProperty(nameof(ClassWithNormalProperties.Integer))!]);
            _class.Integer = 156;

            // Act
            var newHash = _class.Hash([typeof(ClassWithNormalProperties).GetProperty(nameof(ClassWithNormalProperties.Integer))!]);

            // Assert
            newHash.Should().NotBe(hash);
        }
    }

    [TestFixture]
    private class HashExceptSpecifiedProperties : HashableExtensionsTests
    {
        private ClassWithNormalProperties _class = null!;

        [SetUp]
        public void SetUp()
        {
            _class = new ClassWithNormalProperties(10, "hello");
        }

        [Test]
        public void Editing_An_Element_Not_Hashed_Should_Not_Have_An_Impact()
        {
            // Arrange 
            var hash = _class.HashExcept([typeof(ClassWithNormalProperties).GetProperty(nameof(ClassWithNormalProperties.String))!]);
            _class.String = "world";

            // Act
            var newHash = _class.HashExcept([typeof(ClassWithNormalProperties).GetProperty(nameof(ClassWithNormalProperties.String))!]);

            // Assert
            newHash.Should().Be(hash);
        }

        [Test]
        public void Editing_a_Hashed_Element_Should_Have_An_Impact()
        {
            // Arrange 
            var hash = _class.HashExcept([typeof(ClassWithNormalProperties).GetProperty(nameof(ClassWithNormalProperties.String))!]);
            _class.Integer = 156;

            // Act
            var newHash = _class.HashExcept([typeof(ClassWithNormalProperties).GetProperty(nameof(ClassWithNormalProperties.String))!]);

            // Assert
            newHash.Should().NotBe(hash);
        }
    }
}
