using Gex.Core;
using FluentAssertions;

namespace Gex.Tests;

public class TokenizerTests
{
    [Fact]
    public void Tokenize_SimpleExpression_ReturnsCorrectTokens()
    {
        // Arrange
        var input = "1 + 2";
        var tokenizer = new Tokenizer(input);

        // Act
        var tokens = tokenizer.Tokenize().ToList();

        // Assert
        tokens.Should().HaveCount(4); // 1, +, 2, EOF
        tokens[0].Kind.Should().Be(TokenKind.Number);
        tokens[1].Kind.Should().Be(TokenKind.Plus);
        tokens[2].Kind.Should().Be(TokenKind.Number);
        tokens[3].Kind.Should().Be(TokenKind.EndOfFile);
    }

    [Fact]
    public void Tokenize_Identifier_ReturnsIdentifierToken()
    {
        // Arrange
        var input = "myVariable";
        var tokenizer = new Tokenizer(input);

        // Act
        var tokens = tokenizer.Tokenize().ToList();

        // Assert
        tokens.Should().HaveCount(2); // identifier, EOF
        tokens[0].Kind.Should().Be(TokenKind.Identifier);
        tokens[0].Text.Should().Be("myVariable");
    }

    [Fact]
    public void Tokenize_String_ReturnsStringToken()
    {
        // Arrange
        var input = "\"hello world\"";
        var tokenizer = new Tokenizer(input);

        // Act
        var tokens = tokenizer.Tokenize().ToList();

        // Assert
        tokens.Should().HaveCount(2); // string, EOF
        tokens[0].Kind.Should().Be(TokenKind.String);
    }
}

public class ParserTests
{
    [Fact]
    public void Parse_SimpleAddition_ReturnsBinaryExpression()
    {
        // Arrange
        var input = "1 + 2";
        var tokenizer = new Tokenizer(input);
        var tokens = tokenizer.Tokenize().ToList();
        var parser = new Parser(tokens);

        // Act
        var expression = parser.Parse();

        // Assert
        expression.Should().NotBeNull();
        expression.Should().BeOfType<BinaryExpression>();
        parser.HasErrors.Should().BeFalse();
    }

    [Fact]
    public void Parse_ComplexExpression_ReturnsCorrectStructure()
    {
        // Arrange
        var input = "a + b * c";
        var tokenizer = new Tokenizer(input);
        var tokens = tokenizer.Tokenize().ToList();
        var parser = new Parser(tokens);

        // Act
        var expression = parser.Parse();

        // Assert
        expression.Should().NotBeNull();
        expression.Should().BeOfType<BinaryExpression>();
        var binary = (BinaryExpression)expression!;
        binary.Right.Should().BeOfType<BinaryExpression>(); // b * c should be grouped
    }

    [Fact]
    public void Parse_FunctionCall_ReturnsCallExpression()
    {
        // Arrange
        var input = "print(\"hello\")";
        var tokenizer = new Tokenizer(input);
        var tokens = tokenizer.Tokenize().ToList();
        var parser = new Parser(tokens);

        // Act
        var expression = parser.Parse();

        // Assert
        expression.Should().NotBeNull();
        expression.Should().BeOfType<CallExpression>();
        var call = (CallExpression)expression!;
        call.Arguments.Should().HaveCount(1);
    }
}

public class BinderTests
{
    [Fact]
    public void BindExpression_LiteralNumber_ReturnsFloatType()
    {
        // Arrange
        var literal = new LiteralExpression(new Token(TokenKind.Number, "42", 0, 1, 1));
        var binder = new Binder();

        // Act
        var bound = binder.BindExpression(literal);

        // Assert
        bound.Should().NotBeNull();
        bound.Should().BeOfType<BoundLiteralExpression>();
        bound!.Type.Should().Be(TypeSymbol.Float);
    }

    [Fact]
    public void BindExpression_BinaryAddition_ReturnsCorrectType()
    {
        // Arrange
        var left = new LiteralExpression(new Token(TokenKind.Number, "1", 0, 1, 1));
        var right = new LiteralExpression(new Token(TokenKind.Number, "2", 0, 1, 1));
        var binary = new BinaryExpression(left, new Token(TokenKind.Plus, "+", 0, 1, 2), right);
        var binder = new Binder();

        // Act
        var bound = binder.BindExpression(binary);

        // Assert
        bound.Should().NotBeNull();
        bound.Should().BeOfType<BoundBinaryExpression>();
        bound!.Type.Should().Be(TypeSymbol.Float);
    }

    [Fact]
    public void BindExpression_UndefinedVariable_ProducesError()
    {
        // Arrange
        var identifier = new IdentifierExpression(new Token(TokenKind.Identifier, "undefined", 0, 1, 1));
        var binder = new Binder();

        // Act
        var bound = binder.BindExpression(identifier);

        // Assert
        binder.HasErrors.Should().BeTrue();
        binder.Errors.Should().HaveCount(1);
        binder.Errors[0].Message.Should().Contain("Undefined identifier");
    }
}
