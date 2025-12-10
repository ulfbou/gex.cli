using Gex.Core;
using Xunit;

public class CliIntegrationTests
{
    [Fact]
    public void Parses_MixedArgs()
    {
        var args = new[] { "-a", "--config=app.json", "input.txt", "--threads=2" };
        var result = Parser.Parse(args);
        Assert.Equal("app.json", result.GetValue("config"));
        Assert.Equal("input.txt", result.Positionals[0].ToString());
        Assert.NotNull(result.Options["a"]);
        Assert.Equal("2", result.GetValue("threads"));
    }
}
