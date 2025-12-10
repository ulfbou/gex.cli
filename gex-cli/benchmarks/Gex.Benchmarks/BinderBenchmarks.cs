using BenchmarkDotNet.Attributes;
using Gex.Core;

namespace Gex.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 5)]
public class BinderBenchmarks
{
    private ExpressionNode _simpleExpression = null!;
    private ExpressionNode _complexExpression = null!;

    [GlobalSetup]
    public void Setup()
    {
        var simpleTokens = new Tokenizer("1 + 2 * 3").Tokenize().ToList();
        var simpleParser = new Parser(simpleTokens);
        _simpleExpression = simpleParser.Parse()!;

        var complexTokens = new Tokenizer("1 + 2 * 3 - 4 / 5").Tokenize().ToList();
        var complexParser = new Parser(complexTokens);
        _complexExpression = complexParser.Parse()!;
    }

    [Benchmark]
    public void Bind_SimpleExpression()
    {
        var binder = new Binder();
        _ = binder.BindExpression(_simpleExpression);
    }

    [Benchmark]
    public void Bind_ComplexExpression()
    {
        var binder = new Binder();
        _ = binder.BindExpression(_complexExpression);
    }
}
