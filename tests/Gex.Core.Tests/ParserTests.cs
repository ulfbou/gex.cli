using Gex.Core;
using Xunit;

public class ParserTests
{
    [Fact]
    public void Parses_LongOption_WithValue()
    {
        var args = new[] { "--config=app.json" };
        var result = Parser.Parse(args);
        Assert.Equal("app.json", result.GetValue("config"));
    }

    [Fact]
    public void Parses_ShortFlags_Group()
    {
        var args = new[] { "-abc" };
        var result = Parser.Parse(args);
        Assert.NotNull(result.Options["a"]);
        Assert.NotNull(result.Options["b"]);
        Assert.NotNull(result.Options["c"]);
    }

    [Fact]
    public void Parses_QuotedValue()
    {
        var args = new[] { "\"quoted\"" };
        var result = Parser.Parse(args);
        Assert.Equal("quoted", result.Positionals[0].ToString());
    }

    [Fact]
    public void Parses_EndOfOptionsMarker()
    {
        var args = new[] { "--", "pos1", "pos2" };
        var result = Parser.Parse(args);
        Assert.Equal("pos1", result.Positionals[0].ToString());
        Assert.Equal("pos2", result.Positionals[1].ToString());
    }
}
