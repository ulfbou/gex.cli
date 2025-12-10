namespace Gex.Core;

public readonly ref struct Token
{
    public TokenKind Kind { get; }
    public int ArgIndex { get; }
    public ReadOnlySpan<char> Span { get; }

    public Token(TokenKind kind, int argIndex, ReadOnlySpan<char> span)
    {
        Kind = kind;
        ArgIndex = argIndex;
        Span = span;
    }

    public override string ToString() => $"{Kind}:{new string(Span)}";
}
