using DnDFightTool.Domain.Logs;
using DnDFightTool.UI.FightBlazorComponents.Log.Parsing;
using FluentAssertions;
using NUnit.Framework;

namespace FightBlazorComponentsTests.Log;

[TestFixture]
public class LogTokenParserTests
{
    [TestFixture]
    public class PlainTextTests : LogTokenParserTests
    {
        [Test]
        public void Should_ParsePlainText()
        {
            // Act
            var tokens = LogTokenParser.Parse("Hello world");

            // Assert
            tokens.Should().HaveCount(1);
            tokens[0].Should().Be(new LogToken.TextToken("Hello world"));
        }

        [Test]
        public void Should_ReturnEmpty_When_InputIsEmpty()
        {
            // Act
            var tokens = LogTokenParser.Parse("");

            // Assert
            tokens.Should().BeEmpty();
        }

        [Test]
        public void Should_ReturnEmpty_When_InputIsNull()
        {
            // Act
            var tokens = LogTokenParser.Parse(null!);

            // Assert
            tokens.Should().BeEmpty();
        }
    }

    [TestFixture]
    public class BoldTests : LogTokenParserTests
    {
        [Test]
        public void Should_ParseBoldTags()
        {
            // Act
            var tokens = LogTokenParser.Parse("[b]bold text[/b]");

            // Assert
            tokens.Should().HaveCount(3);
            tokens[0].Should().BeOfType<LogToken.BoldStart>();
            tokens[1].Should().Be(new LogToken.TextToken("bold text"));
            tokens[2].Should().BeOfType<LogToken.BoldEnd>();
        }

        [Test]
        public void Should_ParseBoldWithSurroundingText()
        {
            // Act
            var tokens = LogTokenParser.Parse("before [b]bold[/b] after");

            // Assert
            tokens.Should().HaveCount(5);
            tokens[0].Should().Be(new LogToken.TextToken("before "));
            tokens[1].Should().BeOfType<LogToken.BoldStart>();
            tokens[2].Should().Be(new LogToken.TextToken("bold"));
            tokens[3].Should().BeOfType<LogToken.BoldEnd>();
            tokens[4].Should().Be(new LogToken.TextToken(" after"));
        }
    }

    [TestFixture]
    public class ColorTests : LogTokenParserTests
    {
        [Test]
        public void Should_ParseColorTags()
        {
            // Act
            var tokens = LogTokenParser.Parse("[c:fire]6 fire damage[/c]");

            // Assert
            tokens.Should().HaveCount(3);
            tokens[0].Should().Be(new LogToken.ColorStart(LogColorToken.Fire));
            tokens[1].Should().Be(new LogToken.TextToken("6 fire damage"));
            tokens[2].Should().BeOfType<LogToken.ColorEnd>();
        }

        [Test]
        public void Should_ParseKebabCaseColorToken()
        {
            // Act
            var tokens = LogTokenParser.Parse("[c:bludgeoning-silver]5 damage[/c]");

            // Assert
            tokens.Should().HaveCount(3);
            tokens[0].Should().Be(new LogToken.ColorStart(LogColorToken.BludgeoningSilver));
        }

        [Test]
        public void Should_RenderAsLiteralText_When_UnknownColorToken()
        {
            // Act
            var tokens = LogTokenParser.Parse("[c:invalid]text[/c]");

            // Assert
            // The [c:invalid] is unknown, so it's literal text. [/c] is a valid closing tag though.
            var textTokens = tokens.OfType<LogToken.TextToken>().ToList();
            textTokens.Should().Contain(t => t.Text.Contains("[c:invalid]"));
        }
    }

    [TestFixture]
    public class HoverTests : LogTokenParserTests
    {
        [Test]
        public void Should_ParseHoverTags()
        {
            // Act
            var tokens = LogTokenParser.Parse("[hover:d20 roll: 15 + 3]18[/hover]");

            // Assert
            tokens.Should().HaveCount(3);
            tokens[0].Should().Be(new LogToken.HoverStart("d20 roll: 15 + 3"));
            tokens[1].Should().Be(new LogToken.TextToken("18"));
            tokens[2].Should().BeOfType<LogToken.HoverEnd>();
        }
    }

    [TestFixture]
    public class NestingTests : LogTokenParserTests
    {
        [Test]
        public void Should_ParseNestedBoldInColor()
        {
            // Act
            var tokens = LogTokenParser.Parse("[c:fire][b]10[/b] fire damage[/c]");

            // Assert
            tokens.Should().HaveCount(6);
            tokens[0].Should().Be(new LogToken.ColorStart(LogColorToken.Fire));
            tokens[1].Should().BeOfType<LogToken.BoldStart>();
            tokens[2].Should().Be(new LogToken.TextToken("10"));
            tokens[3].Should().BeOfType<LogToken.BoldEnd>();
            tokens[4].Should().Be(new LogToken.TextToken(" fire damage"));
            tokens[5].Should().BeOfType<LogToken.ColorEnd>();
        }

        [Test]
        public void Should_ParseNestedColorInHover()
        {
            // Act
            var tokens = LogTokenParser.Parse("[hover:details][c:heal][b]5[/b] HPs[/c][/hover]");

            // Assert
            tokens.Should().HaveCount(8);
            tokens[0].Should().Be(new LogToken.HoverStart("details"));
            tokens[1].Should().Be(new LogToken.ColorStart(LogColorToken.Heal));
            tokens[2].Should().BeOfType<LogToken.BoldStart>();
            tokens[3].Should().Be(new LogToken.TextToken("5"));
            tokens[4].Should().BeOfType<LogToken.BoldEnd>();
            tokens[5].Should().Be(new LogToken.TextToken(" HPs"));
            tokens[6].Should().BeOfType<LogToken.ColorEnd>();
            tokens[7].Should().BeOfType<LogToken.HoverEnd>();
        }
    }

    [TestFixture]
    public class MalformedTests : LogTokenParserTests
    {
        [Test]
        public void Should_TreatUnclosedBracketAsLiteral()
        {
            // Act
            var tokens = LogTokenParser.Parse("text with [unclosed bracket");

            // Assert
            tokens.Should().HaveCount(1);
            tokens[0].Should().Be(new LogToken.TextToken("text with [unclosed bracket"));
        }

        [Test]
        public void Should_TreatUnknownTagAsLiteral()
        {
            // Act
            var tokens = LogTokenParser.Parse("[unknown]text[/unknown]");

            // Assert
            // Unknown tags become literal text
            tokens.Should().AllSatisfy(t => t.Should().BeOfType<LogToken.TextToken>());
        }
    }
}
