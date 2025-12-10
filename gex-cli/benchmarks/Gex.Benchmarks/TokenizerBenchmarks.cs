using BenchmarkDotNet.Attributes;
using Gex.Core;

namespace Gex.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 5)]
public class TokenizerBenchmarks
{
    private const string SimpleExpression = "1 + 2 * 3";
    private const string ComplexExpression = "myVar + (foo * bar) - baz / qux + 42";
    private const string LongExpression = "a + b * c - d / e + f * g - h / i + j * k - l / m + n * o - p / q";

    [Benchmark]
    public void Tokenize_SimpleExpression()
    {
        var tokenizer = new Tokenizer(SimpleExpression);
        _ = tokenizer.Tokenize().ToList();
    }

    [Benchmark]
    public void Tokenize_ComplexExpression()
    {
        var tokenizer = new Tokenizer(ComplexExpression);
        _ = tokenizer.Tokenize().ToList();
    }

    [Benchmark]
    public void Tokenize_LongExpression()
    {
        var tokenizer = new Tokenizer(LongExpression);
        _ = tokenizer.Tokenize().ToList();
    }
}
