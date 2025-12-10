namespace Gex.Core;

/// <summary>
/// Represents a token in the input stream.
/// </summary>
public sealed record Token(TokenKind Kind, string Text, int Position, int Line, int Column)
{
    /// <summary>
    /// Gets a value indicating whether this token is valid.
    /// </summary>
    public bool IsValid => Kind != TokenKind.Invalid;
}

/// <summary>
/// Represents the kind of token.
/// </summary>
public enum TokenKind
{
    Invalid,
    EndOfFile,
    Identifier,
    Number,
    String,
    LeftParen,
    RightParen,
    LeftBrace,
    RightBrace,
    LeftBracket,
    RightBracket,
    Comma,
    Dot,
    Semicolon,
    Colon,
    Equals,
    Plus,
    Minus,
    Star,
    Slash,
    Percent,
    Ampersand,
    Pipe,
    Caret,
    Tilde,
    ExclamationMark,
    QuestionMark,
    LessThan,
    GreaterThan,
    Whitespace,
    Comment,
}

/// <summary>
/// Tokenizes input text into a stream of tokens.
/// </summary>
public sealed class Tokenizer
{
    private readonly string _input;
    private int _position;
    private int _line;
    private int _column;

    /// <summary>
    /// Initializes a new instance of the <see cref="Tokenizer"/> class.
    /// </summary>
    /// <param name="input">The input text to tokenize.</param>
    public Tokenizer(string input)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _position = 0;
        _line = 1;
        _column = 1;
    }

    /// <summary>
    /// Gets the next token from the input stream.
    /// </summary>
    /// <returns>The next token.</returns>
    public Token NextToken()
    {
        if (_position >= _input.Length)
        {
            return new Token(TokenKind.EndOfFile, string.Empty, _position, _line, _column);
        }

        var startPosition = _position;
        var startLine = _line;
        var startColumn = _column;

        var currentChar = _input[_position];

        // Skip whitespace
        if (char.IsWhiteSpace(currentChar))
        {
            return ReadWhitespace(startPosition, startLine, startColumn);
        }

        // Comments
        if (currentChar == '/' && _position + 1 < _input.Length)
        {
            if (_input[_position + 1] == '/')
            {
                return ReadLineComment(startPosition, startLine, startColumn);
            }
            if (_input[_position + 1] == '*')
            {
                return ReadBlockComment(startPosition, startLine, startColumn);
            }
        }

        // Identifiers and keywords
        if (char.IsLetter(currentChar) || currentChar == '_')
        {
            return ReadIdentifier(startPosition, startLine, startColumn);
        }

        // Numbers
        if (char.IsDigit(currentChar))
        {
            return ReadNumber(startPosition, startLine, startColumn);
        }

        // Strings
        if (currentChar == '"' || currentChar == '\'')
        {
            return ReadString(startPosition, startLine, startColumn);
        }

        // Single character tokens
        var kind = currentChar switch
        {
            '(' => TokenKind.LeftParen,
            ')' => TokenKind.RightParen,
            '{' => TokenKind.LeftBrace,
            '}' => TokenKind.RightBrace,
            '[' => TokenKind.LeftBracket,
            ']' => TokenKind.RightBracket,
            ',' => TokenKind.Comma,
            '.' => TokenKind.Dot,
            ';' => TokenKind.Semicolon,
            ':' => TokenKind.Colon,
            '=' => TokenKind.Equals,
            '+' => TokenKind.Plus,
            '-' => TokenKind.Minus,
            '*' => TokenKind.Star,
            '/' => TokenKind.Slash,
            '%' => TokenKind.Percent,
            '&' => TokenKind.Ampersand,
            '|' => TokenKind.Pipe,
            '^' => TokenKind.Caret,
            '~' => TokenKind.Tilde,
            '!' => TokenKind.ExclamationMark,
            '?' => TokenKind.QuestionMark,
            '<' => TokenKind.LessThan,
            '>' => TokenKind.GreaterThan,
            _ => TokenKind.Invalid
        };

        Advance();
        return new Token(kind, currentChar.ToString(), startPosition, startLine, startColumn);
    }

    /// <summary>
    /// Gets all tokens from the input stream.
    /// </summary>
    /// <returns>An enumerable of tokens.</returns>
    public IEnumerable<Token> Tokenize()
    {
        Token token;
        do
        {
            token = NextToken();
            if (token.Kind != TokenKind.Whitespace && token.Kind != TokenKind.Comment)
            {
                yield return token;
            }
        } while (token.Kind != TokenKind.EndOfFile);
    }

    private Token ReadWhitespace(int startPosition, int startLine, int startColumn)
    {
        var start = _position;
        while (_position < _input.Length && char.IsWhiteSpace(_input[_position]))
        {
            Advance();
        }
        return new Token(TokenKind.Whitespace, _input[start.._position], startPosition, startLine, startColumn);
    }

    private Token ReadLineComment(int startPosition, int startLine, int startColumn)
    {
        var start = _position;
        while (_position < _input.Length && _input[_position] != '\n')
        {
            Advance();
        }
        return new Token(TokenKind.Comment, _input[start.._position], startPosition, startLine, startColumn);
    }

    private Token ReadBlockComment(int startPosition, int startLine, int startColumn)
    {
        var start = _position;
        _position += 2; // Skip /*
        while (_position < _input.Length - 1)
        {
            if (_input[_position] == '*' && _input[_position + 1] == '/')
            {
                _position += 2;
                break;
            }
            Advance();
        }
        return new Token(TokenKind.Comment, _input[start.._position], startPosition, startLine, startColumn);
    }

    private Token ReadIdentifier(int startPosition, int startLine, int startColumn)
    {
        var start = _position;
        while (_position < _input.Length && (char.IsLetterOrDigit(_input[_position]) || _input[_position] == '_'))
        {
            Advance();
        }
        return new Token(TokenKind.Identifier, _input[start.._position], startPosition, startLine, startColumn);
    }

    private Token ReadNumber(int startPosition, int startLine, int startColumn)
    {
        var start = _position;
        while (_position < _input.Length && char.IsDigit(_input[_position]))
        {
            Advance();
        }

        if (_position < _input.Length && _input[_position] == '.')
        {
            Advance();
            while (_position < _input.Length && char.IsDigit(_input[_position]))
            {
                Advance();
            }
        }

        return new Token(TokenKind.Number, _input[start.._position], startPosition, startLine, startColumn);
    }

    private Token ReadString(int startPosition, int startLine, int startColumn)
    {
        var quote = _input[_position];
        var start = _position;
        Advance(); // Skip opening quote

        while (_position < _input.Length && _input[_position] != quote)
        {
            if (_input[_position] == '\\' && _position + 1 < _input.Length)
            {
                Advance(); // Skip escape character
            }
            Advance();
        }

        if (_position < _input.Length)
        {
            Advance(); // Skip closing quote
        }

        return new Token(TokenKind.String, _input[start.._position], startPosition, startLine, startColumn);
    }

    private void Advance()
    {
        if (_position < _input.Length)
        {
            if (_input[_position] == '\n')
            {
                _line++;
                _column = 1;
            }
            else
            {
                _column++;
            }
            _position++;
        }
    }
}
