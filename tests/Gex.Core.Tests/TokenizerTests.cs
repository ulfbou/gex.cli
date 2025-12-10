using Gex.Core;
using Xunit;

public class TokenizerTests
{
    [Fact]
    public void Tokenize_SimpleInput_ReturnsTokens()
    {
        var input = new[] { "foo", "bar", "baz" };
        var enumerator = Tokenizer.GetTokens(input);
        int count = 0;
        string[] expected = { "foo", "bar", "baz" };
        while (enumerator.MoveNext())
        {
            Assert.Equal(expected[count], enumerator.Current.Span.ToString());
            count++;
        }
        Assert.Equal(3, count);
    }
}
