namespace Gex.Core;

/// <summary>
/// Represents a syntax node in the parse tree.
/// </summary>
public abstract record SyntaxNode
{
    /// <summary>
    /// Gets the start position of this node.
    /// </summary>
    public abstract int Position { get; }

    /// <summary>
    /// Gets the line number where this node starts.
    /// </summary>
    public abstract int Line { get; }

    /// <summary>
    /// Gets the column number where this node starts.
    /// </summary>
    public abstract int Column { get; }
}

/// <summary>
/// Represents an expression node.
/// </summary>
public abstract record ExpressionNode : SyntaxNode;

/// <summary>
/// Represents a literal expression.
/// </summary>
public sealed record LiteralExpression(Token Token) : ExpressionNode
{
    public override int Position => Token.Position;
    public override int Line => Token.Line;
    public override int Column => Token.Column;
}

/// <summary>
/// Represents an identifier expression.
/// </summary>
public sealed record IdentifierExpression(Token Token) : ExpressionNode
{
    public override int Position => Token.Position;
    public override int Line => Token.Line;
    public override int Column => Token.Column;
    public string Name => Token.Text;
}

/// <summary>
/// Represents a binary expression.
/// </summary>
public sealed record BinaryExpression(ExpressionNode Left, Token Operator, ExpressionNode Right) : ExpressionNode
{
    public override int Position => Left.Position;
    public override int Line => Left.Line;
    public override int Column => Left.Column;
}

/// <summary>
/// Represents a unary expression.
/// </summary>
public sealed record UnaryExpression(Token Operator, ExpressionNode Operand) : ExpressionNode
{
    public override int Position => Operator.Position;
    public override int Line => Operator.Line;
    public override int Column => Operator.Column;
}

/// <summary>
/// Represents a call expression.
/// </summary>
public sealed record CallExpression(ExpressionNode Callee, IReadOnlyList<ExpressionNode> Arguments, Token LeftParen) : ExpressionNode
{
    public override int Position => Callee.Position;
    public override int Line => Callee.Line;
    public override int Column => Callee.Column;
}

/// <summary>
/// Represents a member access expression.
/// </summary>
public sealed record MemberAccessExpression(ExpressionNode Target, Token Dot, Token Member) : ExpressionNode
{
    public override int Position => Target.Position;
    public override int Line => Target.Line;
    public override int Column => Target.Column;
}

/// <summary>
/// Represents a parse error.
/// </summary>
public sealed record ParseError(string Message, int Position, int Line, int Column);

/// <summary>
/// Parses tokens into a syntax tree.
/// </summary>
public sealed class Parser
{
    private readonly List<Token> _tokens;
    private readonly List<ParseError> _errors;
    private int _position;

    /// <summary>
    /// Initializes a new instance of the <see cref="Parser"/> class.
    /// </summary>
    /// <param name="tokens">The tokens to parse.</param>
    public Parser(IEnumerable<Token> tokens)
    {
        _tokens = tokens?.ToList() ?? throw new ArgumentNullException(nameof(tokens));
        _errors = new List<ParseError>();
        _position = 0;
    }

    /// <summary>
    /// Gets the parse errors.
    /// </summary>
    public IReadOnlyList<ParseError> Errors => _errors;

    /// <summary>
    /// Gets a value indicating whether parsing was successful.
    /// </summary>
    public bool HasErrors => _errors.Count > 0;

    /// <summary>
    /// Parses the tokens into an expression.
    /// </summary>
    /// <returns>The parsed expression, or null if parsing failed.</returns>
    public ExpressionNode? Parse()
    {
        try
        {
            return ParseExpression();
        }
        catch (Exception ex)
        {
            AddError($"Unexpected error: {ex.Message}");
            return null;
        }
    }

    private ExpressionNode? ParseExpression()
    {
        return ParseBinaryExpression();
    }

    private ExpressionNode? ParseBinaryExpression(int precedence = 0)
    {
        var left = ParseUnaryExpression();
        if (left == null)
        {
            return null;
        }

        while (true)
        {
            var current = Current;
            if (current.Kind == TokenKind.EndOfFile)
            {
                break;
            }

            var operatorPrecedence = GetBinaryOperatorPrecedence(current.Kind);
            if (operatorPrecedence <= precedence)
            {
                break;
            }

            var operatorToken = Consume();
            var right = ParseBinaryExpression(operatorPrecedence);
            if (right == null)
            {
                return null;
            }

            left = new BinaryExpression(left, operatorToken, right);
        }

        return left;
    }

    private ExpressionNode? ParseUnaryExpression()
    {
        var current = Current;
        if (current.Kind is TokenKind.Plus or TokenKind.Minus or TokenKind.ExclamationMark or TokenKind.Tilde)
        {
            var operatorToken = Consume();
            var operand = ParseUnaryExpression();
            if (operand == null)
            {
                return null;
            }
            return new UnaryExpression(operatorToken, operand);
        }

        return ParsePostfixExpression();
    }

    private ExpressionNode? ParsePostfixExpression()
    {
        var expression = ParsePrimaryExpression();
        if (expression == null)
        {
            return null;
        }

        while (true)
        {
            var current = Current;
            if (current.Kind == TokenKind.LeftParen)
            {
                var leftParen = Consume();
                var arguments = new List<ExpressionNode>();

                if (Current.Kind != TokenKind.RightParen)
                {
                    do
                    {
                        var arg = ParseExpression();
                        if (arg == null)
                        {
                            return null;
                        }
                        arguments.Add(arg);

                        if (Current.Kind == TokenKind.Comma)
                        {
                            Consume();
                        }
                        else
                        {
                            break;
                        }
                    } while (Current.Kind != TokenKind.RightParen);
                }

                if (!Expect(TokenKind.RightParen))
                {
                    return null;
                }

                expression = new CallExpression(expression, arguments, leftParen);
            }
            else if (current.Kind == TokenKind.Dot)
            {
                var dot = Consume();
                if (!Expect(TokenKind.Identifier))
                {
                    return null;
                }
                var member = Previous;
                expression = new MemberAccessExpression(expression, dot, member);
            }
            else
            {
                break;
            }
        }

        return expression;
    }

    private ExpressionNode? ParsePrimaryExpression()
    {
        var current = Current;

        if (current.Kind == TokenKind.Identifier)
        {
            return new IdentifierExpression(Consume());
        }

        if (current.Kind is TokenKind.Number or TokenKind.String)
        {
            return new LiteralExpression(Consume());
        }

        if (current.Kind == TokenKind.LeftParen)
        {
            Consume();
            var expression = ParseExpression();
            if (expression == null)
            {
                return null;
            }

            if (!Expect(TokenKind.RightParen))
            {
                return null;
            }

            return expression;
        }

        AddError($"Unexpected token: {current.Kind}");
        return null;
    }

    private static int GetBinaryOperatorPrecedence(TokenKind kind)
    {
        return kind switch
        {
            TokenKind.Star or TokenKind.Slash or TokenKind.Percent => 6,
            TokenKind.Plus or TokenKind.Minus => 5,
            TokenKind.LessThan or TokenKind.GreaterThan => 4,
            TokenKind.Equals => 3,
            TokenKind.Ampersand => 2,
            TokenKind.Pipe => 1,
            _ => 0
        };
    }

    private Token Current => _position < _tokens.Count ? _tokens[_position] : new Token(TokenKind.EndOfFile, string.Empty, 0, 0, 0);
    private Token Previous => _position > 0 ? _tokens[_position - 1] : new Token(TokenKind.Invalid, string.Empty, 0, 0, 0);

    private Token Consume()
    {
        var token = Current;
        if (_position < _tokens.Count)
        {
            _position++;
        }
        return token;
    }

    private bool Expect(TokenKind kind)
    {
        if (Current.Kind == kind)
        {
            Consume();
            return true;
        }

        AddError($"Expected {kind}, but got {Current.Kind}");
        return false;
    }

    private void AddError(string message)
    {
        var token = Current;
        _errors.Add(new ParseError(message, token.Position, token.Line, token.Column));
    }
}
