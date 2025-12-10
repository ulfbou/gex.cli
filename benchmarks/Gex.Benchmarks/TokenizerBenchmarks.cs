using BenchmarkDotNet.Attributes;
using Gex.Core;
using Gex.Core.Sample;

[MemoryDiagnoser]
public class TokenizerBenchmarks
{
    private string[] input = Enumerable.Repeat("foo", 1000).ToArray();

    [Benchmark]
    public int SpanTokenizer()
    {
        var enumerator = Tokenizer.GetTokens(input);
        int count = 0;
        while (enumerator.MoveNext())
        {
            count++;
        }
        return count;
    }

    [Benchmark]
    public int ParserThroughput()
    {
        var args = new[] { "--config=app.json", "--threads=8", "-v", "input.txt" };
        var result = Parser.Parse(args);
        return result.Options.Count + result.Positionals.Count;
    }

    [Benchmark]
    public RunArgs ReflectionBinder()
    {
        var args = new[] { "--config=app.json", "--threads=8", "--verbose=true" };
        var result = Parser.Parse(args);
        return BinderPrototype.Bind<RunArgs>(result);
    }
}
