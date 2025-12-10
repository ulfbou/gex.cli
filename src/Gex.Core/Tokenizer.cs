namespace Gex.Core;

public static class Tokenizer
{
    public static TokenEnumerator GetTokens(ReadOnlySpan<string> args) => new TokenEnumerator(args);
}
