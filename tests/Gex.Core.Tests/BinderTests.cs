using Gex.Core;
using Gex.Core.Sample;
using Xunit;

public class BinderTests
{
    [Fact]
    public void Binds_RunArgs_Properties()
    {
        var args = new[] { "--config=app.json", "--threads=4", "--verbose=true" };
        var result = Parser.Parse(args);
        var bound = BinderPrototype.Bind<RunArgs>(result);
        Assert.Equal("app.json", bound.Config);
        Assert.Equal(4, bound.Threads);
        Assert.True(bound.Verbose);
    }
}
