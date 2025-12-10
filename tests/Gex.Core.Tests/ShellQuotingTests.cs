using Gex.Core;
using Xunit;

public class ShellQuotingTests
{
    [Fact]
    public void Parses_WindowsStyleQuotes()
    {
        var args = new[] { "\"win quoted\"" };
        var result = Parser.Parse(args);
        Assert.Equal("win quoted", result.Positionals[0].ToString());
    }

    [Fact]
    public void Parses_UnixStyleQuotes()
    {
        var args = new[] { "'unix quoted'" };
        var result = Parser.Parse(args);
        Assert.Equal("unix quoted", result.Positionals[0].ToString());
    }
}
