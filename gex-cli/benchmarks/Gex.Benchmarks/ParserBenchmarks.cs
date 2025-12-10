using BenchmarkDotNet.Attributes;
using Gex.Core;

namespace Gex.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 5)]
public class ParserBenchmarks
{
    private List<Token> _simpleTokens = null!;
    private List<Token> _complexTokens = null!;
    private List<Token> _longTokens = null!;

    [GlobalSetup]
    public void Setup()
    {
        _simpleTokens = new Tokenizer("1 + 2 * 3").Tokenize().ToList();
        _complexTokens = new Tokenizer("myVar + (foo * bar) - baz / qux + 42").Tokenize().ToList();
        _longTokens = new Tokenizer("a + b * c - d / e + f * g - h / i + j * k - l / m + n * o - p / q").Tokenize().ToList();
    }

    [Benchmark]
    public void Parse_SimpleExpression()
    {
        var parser = new Parser(_simpleTokens);
        _ = parser.Parse();
    }

    [Benchmark]
    public void Parse_ComplexExpression()
    {
        var parser = new Parser(_complexTokens);
        _ = parser.Parse();
    }

    [Benchmark]
    public void Parse_LongExpression()
    {
        var parser = new Parser(_longTokens);
        _ = parser.Parse();
    }
}
